using TheProjector.Data.DTO;

namespace TheProjector.Data.DTO;

public class ProjectSearchCollection : PaginatedCollection<ProjectListItemInfo>
{
    // query strings
    public string? NameSearch { get; set; }
}