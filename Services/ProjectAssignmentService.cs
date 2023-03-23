using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Utilities;

public class ProjectAssignmentService
{
    TheProjectorDbContext _dbContext;
    public ProjectAssignmentService(TheProjectorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<ProjectIdName>> GetProjectsAssignedTo(long personId)
    {
        return await _dbContext.People.Where(person => person.Id == personId)
            .SelectMany(person => person.AssignedProjects)
            .Select(project => new ProjectIdName { Id = project.Id, Name = project.Name })
            .ToListAsync();
    }

    // separate method for DRY
    private IQueryable<Person> GetPeopleAssignedToProjectQuery(long projectId)
    {
        return _dbContext.Projects
            .Where(project => project.Id == projectId)
            .SelectMany(project => project.AssignedPeople);
    }

    public async Task<ICollection<PersonListItemInfo>> GetAssignablePeople(long projectId)
    {
        var assignedPeopleQuery = GetPeopleAssignedToProjectQuery(projectId);
        ICollection<PersonListItemInfo> people = await _dbContext.People
            .Where(person => !assignedPeopleQuery.Select(assigned => assigned.Id).Contains(person.Id))
            .Select(assignable => new PersonListItemInfo { Id = assignable.Id, Name = assignable.FullName })
            .ToListAsync();
        return people;
    }

    public async Task<ICollection<PersonListItemInfo>> GetAssignedPeople(long projectId)
    {
        ICollection<PersonListItemInfo> people = await GetPeopleAssignedToProjectQuery(projectId)
            .Select(assigned => new PersonListItemInfo { Id = assigned.Id, Name = assigned.FullName })
            .ToListAsync();
        return people;
    }

    public async Task<CommandResult> AssignPerson(ProjectPersonPairRequest pair)
    {
        try
        {
            Project? project = await _dbContext.Projects.FindAsync(pair.ProjectId);
            if (project == null)
            {
                return CommandResult.Fail("Project not found.");
            }
            if (project.IsArchived)
            {
                return CommandResult.Fail("Can't assign new people on archived projects.");
            }

            Person? person = await _dbContext.People.FindAsync(pair.PersonId);
            if (person == null)
            {
                return CommandResult.Fail("Person not found.");
            }
            // instantiate with empty list to avoid querying.
            project.AssignedPeople = new List<Person>();
            project.AssignedPeople.Add(person);
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            // throws when inserted rows == 0, which may mean they're already assigned to the project.
            return CommandResult.Fail("Error in assigning Person. The Person might not exist or is probably already assigned to the project.");
        }
    }

    public async Task<CommandResult> UnassignPerson(ProjectPersonPairRequest pair)
    {
        Project project;
        try
        {
            project = await _dbContext.Projects
                .Where(project => project.Id == pair.ProjectId)
                .Include(project => project.AssignedPeople)
                .FirstAsync();
            if (project.IsArchived)
            {
                return CommandResult.Fail("Can't unassign people on archived projects.");
            }
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