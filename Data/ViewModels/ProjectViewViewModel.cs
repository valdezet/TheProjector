using TheProjector.Data.DTO;

namespace TheProjector.Data.ViewModels;

public class ProjectViewViewModel
{
    public ProjectBasicInfo BasicInfo { get; set; }


    // TODO: add info for assigned people, assignable
    public ICollection<PersonListItemInfo> AssignablePeople { get; set; }

    public ICollection<PersonListItemInfo> AssignedPeople { get; set; } = new List<PersonListItemInfo>();

}