using TheProjector.Data.DTO;

namespace TheProjector.Data.DTO;

public class ProjectSearchCollection
{

    public ICollection<ProjectListItemInfo> Collection { get; set; } = new List<ProjectListItemInfo>();

    // pagination

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; }

    public int ItemsPerPage { get; set; }

    public int PageButtonsShown { get; set; } = 10;

    /* calculated properties */

    public int TotalPageCount { get; set; }

    public int FirstPageNumberDisplayed { get; set; }

    public int LastPageNumberDisplayed { get; set; }

    // search query
    public string? NameSearch { get; set; }

    public bool Archived { get; set; } = false;
}