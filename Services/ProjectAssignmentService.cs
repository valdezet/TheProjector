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