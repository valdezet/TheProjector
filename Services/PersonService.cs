using TheProjector.Data.DTO;

using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Services;


public class PersonService
{

    private TheProjectorDbContext _dbContext;

    public PersonService(TheProjectorDbContext dbContext)
    {
        // avoiding manual resolution of Db Context for now.
        _dbContext = dbContext;

    }

    public async Task<PersonSearchCollection> SearchPeople(PersonSearchRequest query)
    {
        int itemsPerPage = query.ItemsPerPage;
        int currentPage = query.Page;
        int skip = itemsPerPage * (currentPage - 1);
        string? nameSearch = query.Name;

        IQueryable<Person> getPeopleQuery = _dbContext.People;

        if (!String.IsNullOrEmpty(nameSearch))
        {
            getPeopleQuery = getPeopleQuery.Where(c => (c.FirstName + " " + c.LastName).Contains(nameSearch));
        }

        int projectCount = getPeopleQuery.Count();

        getPeopleQuery = getPeopleQuery.OrderBy(person => person.Id).Skip(skip).Take(itemsPerPage);

        ICollection<PersonListItemInfo> results = await getPeopleQuery
            .Select(p => new PersonListItemInfo { Id = p.Id, Name = p.FullName })
            .ToListAsync();

        return new PersonSearchCollection
        {
            NameSearch = nameSearch,
            CurrentPage = currentPage,
            ItemsPerPage = itemsPerPage,
            TotalCount = projectCount,
            Collection = results
        };
    }

    public async Task<PersonBasicInfo> GetPersonBasicInfo(long id)
    {
        Person person = await _dbContext.People.Where(p => p.Id == id).FirstAsync();

        return new PersonBasicInfo
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName
        };
    }

    public async Task<CommandResult> CreatePerson(PersonBasicInfo form)
    {
        try
        {
            Person newPerson = new Person
            {
                FirstName = form.FirstName,
                LastName = form.LastName
            };
            _dbContext.People.Add(newPerson);
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (Exception)
        {
            return CommandResult.Fail("There was an error in inserting to database.");
        }
    }
}