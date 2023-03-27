using TheProjector.Data.DTO;
using TheProjector.Data.Request;
using TheProjector.Data.Persistence;
using TheProjector.Data.Form;
using TheProjector.Extensions;
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

        IQueryable<Project> getProjectsQuery = _dbContext.Projects;
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

        ICollection<ProjectListItemInfo> results = await getProjectsQuery
            .Select(p => new ProjectListItemInfo
            {
                Id = p.Id,
                Name = p.Name,
                BudgetShorthand = $"{p.BudgetCurrencyCode} {p.Budget.Shorthand()}",
                BudgetLocalized = $"{p.BudgetCurrencyCode} {p.Budget.Localized()}"
            })
            .ToListAsync();

        int totalPageCount = (int)Math.Ceiling((double)projectCount / itemsPerPage);
        int pageButtonsShown = 10;
        int firstPageNumberDisplayed = Math.Max(1, currentPage - (int)((double)pageButtonsShown / 2));
        int lastPageNumberDisplayed = Math.Min(currentPage + (int)((double)pageButtonsShown / 2), totalPageCount);

        return new ProjectSearchCollection
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

    public async Task<ProjectBasicInfo> GetProjectBasicInfo(long id)
    {
        Project project = await _dbContext.Projects.Where(p => p.Id == id).FirstAsync();

        return new ProjectBasicInfo
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            BudgetShorthand = $"{project.BudgetCurrencyCode} {project.Budget.Shorthand()}",
            BudgetLocalized = $"{project.BudgetCurrencyCode} {project.Budget.Localized()}",
            Remarks = project.Remarks,
            IsArchived = project.DateArchivedUtc != null,
            DateArchivedUtc = project.DateArchivedUtc
        };
    }

    public async Task<ProjectForm?> GetProjectFormInfo(long projectId)
    {
        Project? projectInfo = await _dbContext.Projects.FindAsync(projectId);
        if (projectInfo == null)
        {
            return null;
        }
        return new ProjectForm
        {
            Id = projectInfo.Id,
            Code = projectInfo.Code,
            Name = projectInfo.Name,
            Budget = projectInfo.Budget,
            BudgetCurrencyCode = projectInfo.BudgetCurrencyCode,
            Remarks = projectInfo.Remarks,
            RowVersion = projectInfo.RowVersion
        };
    }

    public async Task<bool> CheckCodeExistence(string projectCode)
    {
        return await _dbContext.Projects.AnyAsync(project => project.Code == projectCode);
    }

    public async Task<bool> CheckCodeExistence(string projectCode, long exceptProjectId)
    {
        return await _dbContext.Projects.AnyAsync(project => (project.Code == projectCode) && (project.Id != exceptProjectId));
    }

    public async Task<CommandResult> CreateProject(ProjectForm form)
    {
        Project newProject = new Project
        {
            Name = form.Name,
            Budget = form.Budget,
            BudgetCurrencyCode = form.BudgetCurrencyCode,
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

    public async Task<CommandResult> EditProject(ProjectForm form)
    {
        Project? project = await _dbContext.Projects.FindAsync(form.Id);
        if (project == null)
        {
            return CommandResult.Fail("Project does not exist or has been deleted by another user.");
        }
        if (project.IsArchived)
        {
            return CommandResult.Fail("Can not update archived projects.");
        }
        project.Code = form.Code;
        project.Name = form.Name;
        project.Budget = form.Budget;
        project.BudgetCurrencyCode = form.BudgetCurrencyCode;
        project.Remarks = form.Remarks;

        _dbContext.Entry(project).OriginalValues["RowVersion"] = form.RowVersion!;


        try
        {
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
                return CommandResult.Fail("The project has been deleted by another user.");
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
                "This project has been recently changed by another user. Please reload the page and make the changes again.",
                concurrencyPropertyErrors);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Fail("There was an error in updating the Project.");
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

    public async Task<CommandResult> UnarchiveProject(long projectId)
    {
        try
        {
            Project? project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
            {
                return CommandResult.Fail("Project not found.");
            }
            if (!project.IsArchived)
            {
                return CommandResult.Fail("Project is not archived.");
            }
            project.DateArchivedUtc = null;
            await _dbContext.SaveChangesAsync();
            return CommandResult.Success();
        }
        catch (DbUpdateException)
        {
            return CommandResult.Fail("Error in unarchiving the project.");
        }
    }
}