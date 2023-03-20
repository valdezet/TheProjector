using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Services;

public class ProjectAssignmentService
{
    TheProjectorDbContext _dbContext;
    public ProjectAssignmentService(TheProjectorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<PersonListItemInfo>> GetAssignablePeople(long projectId)
    {
        ICollection<PersonListItemInfo> people = await _dbContext.People
                .GroupJoin(
                    _dbContext.PersonProjectAssignments.Where(ppa => ppa.ProjectId == projectId),
                    person => person.Id,
                    ppa => ppa.PersonId,
                    (person, ppa) => new { person, ppa }
                )
                .SelectMany(
                    joined => joined.ppa.DefaultIfEmpty(),
                    (joined, ppa) => new { person = joined.person, ppa = ppa }
                )
                .Where(joined => joined.ppa == null)
                .Select(joined => new PersonListItemInfo
                {
                    Id = joined.person.Id,
                    Name = String.Join(" ", joined.person.FirstName, joined.person.LastName)
                })
                .ToListAsync();
        return people;
    }

    public async Task<ICollection<PersonListItemInfo>> GetAssignedPeople(long projectId)
    {
        ICollection<PersonListItemInfo> people = await (
            from person in _dbContext.People
            join ppa in _dbContext.PersonProjectAssignments on person.Id equals ppa.PersonId
            where ppa.ProjectId == projectId
            select new PersonListItemInfo { Name = person.FullName, Id = person.Id }
        ).ToListAsync();

        return people;
    }

    public async Task<CommandResult> AssignPerson(ProjectPersonPairRequest pair)
    {
        try
        {

            PersonProjectAssignment ppa = new PersonProjectAssignment
            {
                ProjectId = pair.ProjectId,
                PersonId = pair.PersonId
            };
            _dbContext.Add(ppa);
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            return CommandResult.Fail("There was an error in assigning the person");
        }

    }
}