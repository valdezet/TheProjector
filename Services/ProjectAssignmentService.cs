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

    public async Task<CommandResult> UnassignPerson(ProjectPersonPairRequest pair)
    {
        Project project;
        try
        {
            // I don't know yet if there's a way to remove a person from relationship
            // without having to make these queries and not using the intermediate table which is to be removed in a future refactor            
            // https://learn.microsoft.com/en-us/ef/core/saving/related-data#removing-relationships
            // technical spike if I go with raw SQL (fromSql): input santitaion, updated rows == 0 problem, 
            // what if the ProjectPersonAssignments table name change when I refactor?
            project = await _dbContext.Projects
                .Where(project => project.Id == pair.ProjectId)
                .Include(project => project.AssignedPeople)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            return CommandResult.Fail("Project not found");
        }

        try
        {
            Person personToRemove = project.AssignedPeople.Where(person => person.Id == pair.PersonId).First();
            project.AssignedPeople.Remove(personToRemove);
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (InvalidOperationException)
        {
            return CommandResult.Fail("Person to remove is not found among assigned people in project.");
        }
    }
}