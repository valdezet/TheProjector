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
}