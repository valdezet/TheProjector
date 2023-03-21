using TheProjector.Data.DTO;

namespace TheProjector.Data.ViewModels;

public class PersonViewViewModel
{
    public PersonBasicInfo BasicInfo { get; set; }


    public ICollection<ProjectIdName> AssignedProjects { get; set; }

}