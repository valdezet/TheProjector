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

    public async Task<ProjectSearchCollection<ProjectIdName>> GetProjectsAssignedTo(long personId, ProjectSearchRequest query)
    {
        IQueryable<Project> getProjectsQuery = _dbContext.People.Where(person => person.Id == personId).SelectMany(person => person.AssignedProjects);
        string? nameSearch = query.Name;
        if (!String.IsNullOrEmpty(nameSearch))
        {
            getProjectsQuery = getProjectsQuery.Where(p => p.Name.Contains(nameSearch));
        }

        if (query.Archived)
        {
            getProjectsQuery = getProjectsQuery.Where(p => p.DateArchivedUtc != null);
        }
        else
        {
            getProjectsQuery = getProjectsQuery.Where(p => p.DateArchivedUtc == null);
        }

        int projectCount = getProjectsQuery.Count();

        int itemsPerPage = query.ItemsPerPage;
        int currentPage = query.Page;
        int skip = itemsPerPage * (currentPage - 1);
        getProjectsQuery = getProjectsQuery.OrderBy(project => project.Id).Skip(skip).Take(itemsPerPage);

        ICollection<ProjectIdName> results = await getProjectsQuery
            .Select(p => new ProjectIdName
            {
                Id = p.Id,
                Name = p.Name,
            })
            .ToListAsync();

        int totalPageCount = (int)Math.Ceiling((double)projectCount / itemsPerPage);
        int pageButtonsShown = 10;
        int firstPageNumberDisplayed = Math.Max(1, currentPage - (int)((double)pageButtonsShown / 2));
        int lastPageNumberDisplayed = Math.Min(currentPage + (int)((double)pageButtonsShown / 2), totalPageCount);

        return new ProjectSearchCollection<ProjectIdName>
        {
            NameSearch = nameSearch,
            CurrentPage = currentPage,
            ItemsPerPage = itemsPerPage,
            TotalCount = projectCount,
            Collection = results,
            Archived = query.Archived,
            TotalPageCount = totalPageCount,
            FirstPageNumberDisplayed = firstPageNumberDisplayed,
            LastPageNumberDisplayed = lastPageNumberDisplayed
        };
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

    public async Task<PersonSearchCollection> GetAssignedPeople(long projectId, PersonSearchRequest query)
    {

        int itemsPerPage = query.ItemsPerPage;
        int currentPage = query.Page;
        int skip = itemsPerPage * (currentPage - 1);
        string? nameSearch = query.Name;

        IQueryable<Person> getPeopleQuery = GetPeopleAssignedToProjectQuery(projectId);

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