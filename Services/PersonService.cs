using TheProjector.Data.DTO;

using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using TheProjector.Data.Form;

namespace TheProjector.Services;


public class PersonService
{

    private TheProjectorDbContext _dbContext;

    private PasswordService _passwordService;

    public PersonService(
        TheProjectorDbContext dbContext,
        PasswordService passwordService
        )
    {
        // avoiding manual resolution of Db Context for now.
        _dbContext = dbContext;
        _passwordService = passwordService;

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

        int totalPageCount = (int)Math.Ceiling((double)projectCount / itemsPerPage);
        int pageButtonsShown = 10;
        int firstPageNumberDisplayed = Math.Max(1, currentPage - (int)((double)pageButtonsShown / 2));
        int lastPageNumberDisplayed = Math.Min(currentPage + (int)((double)pageButtonsShown / 2), totalPageCount);

        return new PersonSearchCollection
        {
            NameSearch = nameSearch,
            CurrentPage = currentPage,
            ItemsPerPage = itemsPerPage,
            TotalCount = projectCount,
            Collection = results,
            TotalPageCount = totalPageCount,
            FirstPageNumberDisplayed = firstPageNumberDisplayed,
            LastPageNumberDisplayed = lastPageNumberDisplayed
        };
    }

    public async Task<PersonBasicInfo> GetPersonBasicInfo(long id)
    {
        Person person = await _dbContext.People.Where(p => p.Id == id).FirstAsync();

        return new PersonBasicInfo
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            RowVersion = person.RowVersion
        };
    }

    public async Task<CommandResult> CreatePerson(CreatePersonForm form)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {

            try
            {
                User user = new User()
                {
                    Email = form.Email,
                    EmailNormalized = form.Email.ToUpper(),
                    PasswordHash = Convert.ToBase64String(_passwordService.MakeHashedPassword(form.Password))
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                Person newPerson = new Person()
                {
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    UserId = user.Id
                };
                _dbContext.People.Add(newPerson);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return CommandResult.Success();
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is DbUpdateException)
            {
                await transaction.RollbackAsync();
                return CommandResult.Fail("There was an error in inserting to database.");
            }
        }
    }

    public async Task<CommandResult> EditPerson(PersonBasicInfo form)
    {
        try
        {
            Person? person = await _dbContext.People.FindAsync(form.Id);
            if (person == null)
            {
                return CommandResult.Fail("Person not found.");
            }
            person.FirstName = form.FirstName;
            person.LastName = form.LastName;
            _dbContext.Entry(person).OriginalValues["RowVersion"] = form.RowVersion!;
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            Dictionary<string, string> concurrencyPropertyErrors = new Dictionary<string, string>();
            var currentValues = entry.CurrentValues;
            var databaseValues = entry.GetDatabaseValues();


            if (databaseValues == null)
            {
                return CommandResult.Fail("The person's records has been deleted by another user.");
            }

            foreach (var property in currentValues.Properties)
            {
                var fieldName = property.Name;
                var currentValue = currentValues[property];
                var databaseValue = databaseValues[property];


                if (currentValue == null)
                {
                    if (databaseValue != null)
                    {
                        concurrencyPropertyErrors.Add(
                            fieldName,
                            $"The {fieldName} field value and the stored data do not match."
                        );
                    }
                }
                else if (!currentValue.Equals(databaseValue))
                {
                    concurrencyPropertyErrors.Add(
                        fieldName,
                        $"The {fieldName} field value and the stored data do not match."
                    );
                }
            }
            return CommandResult.Fail(
                "This person's profile has been recently changed by another user. Please reload the page and make the changes again.",
                concurrencyPropertyErrors);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Fail("There was an error in inserting to database.");
        }
    }
}