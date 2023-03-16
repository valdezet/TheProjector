
using TheProjector.Data.DTO;
using TheProjector.Data.DTO.Form;

namespace TheProjector.Services;

public class ProjectService
{
    public CommandResult CreateProject(ProjectForm form)
    {
        return CommandResult.Success();
    }
}