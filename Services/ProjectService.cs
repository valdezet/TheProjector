using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TheProjector.Services;

public class ProjectService
{
    private TheProjectorDbContext _dbContext;
    public ProjectService(TheProjectorDbContext dbContext)
    {
        // avoiding manual resolution of Db Context for now.
        _dbContext = dbContext;

    }

    public async Task<ProjectSearchCollection> SearchProject(ProjectSearchRequest query)
    {
        int itemsPerPage = query.ItemsPerPage;
        int currentPage = query.Page;
        int skip = itemsPerPage * (currentPage - 1);
        string? nameSearch = query.Name;

        IQueryable<Project> getProjectsQuery = _dbContext.Projects;

        if (!String.IsNullOrEmpty(nameSearch))
        {
            getProjectsQuery = getProjectsQuery.Where(c => c.Name.Contains(nameSearch));
        }

        int projectCount = getProjectsQuery.Count();

        getProjectsQuery = getProjectsQuery.OrderBy(project => project.Id).Skip(skip).Take(itemsPerPage);

        ICollection<ProjectListItemInfo> results = await getProjectsQuery
            .Select(p => new ProjectListItemInfo { Id = p.Id, Name = p.Name, Budget = p.Budget })
            .ToListAsync();

        return new ProjectSearchCollection
        {
            NameSearch = nameSearch,
            CurrentPage = currentPage,
            ItemsPerPage = itemsPerPage,
            TotalCount = projectCount,
            Collection = results
        };
    }

    public async Task<ProjectBasicInfo> GetProjectBasicInfo(long id)
    {
        Project project = await _dbContext.Projects.Where(p => p.Id == id).FirstAsync();

        return new ProjectBasicInfo
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            Budget = project.Budget,
            Remarks = project.Remarks,
            DateArchivedUtc = project.DateArchivedUtc
        };
    }

    public async Task<bool> CheckCodeExistence(string projectCode)
    {
        return await _dbContext.Projects.AnyAsync(project => project.Code == projectCode);
    }

    public async Task<CommandResult> CreateProject(ProjectBasicInfo form)
    {
        Project newProject = new Project
        {
            Name = form.Name,
            Budget = form.Budget,
            Code = form.Code,
            Remarks = form.Remarks
        };
        _dbContext.Add(newProject);
        try
        {

            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException DbUpdateException)
        {
            return CommandResult.Fail(DbUpdateException.Message);
        }
    }

    public async Task<CommandResult> ArchiveProject(long projectId)
    {
        try
        {
            Project? project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
                return CommandResult.Fail("Project not found.");
            if (project.IsArchived)
                return CommandResult.Fail("Project is already archived");
            project.DateArchivedUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            return CommandResult.Fail("Error in archiving the project.");
        }
    }
}