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

    public int TotalPageCount => (int)Math.Ceiling((double)TotalCount / ItemsPerPage);

    public int FirstPageNumberDisplayed => Math.Max(1, CurrentPage - (int)((double)PageButtonsShown / 2));

    public int LastPageNumberDisplayed => Math.Min(CurrentPage + (int)((double)PageButtonsShown / 2), TotalPageCount);

    // search query
    public string? NameSearch { get; set; }
}