using TheProjector.Data.DTO;
using TheProjector.Data.DTO.Form;
using TheProjector.Data.Persistence;

namespace TheProjector.Services;

public class ProjectService
{
    private TheProjectorDbContext _dbContext;
    public ProjectService(TheProjectorDbContext dbContext)
    {
        // avoiding manual resolution of Db Context for now.
        _dbContext = dbContext;

    }
    public async Task<CommandResult> CreateProject(ProjectForm form)
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
}