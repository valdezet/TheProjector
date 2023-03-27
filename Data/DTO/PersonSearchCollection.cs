namespace TheProjector.Data.DTO;


public class PersonSearchCollection
{
    public ICollection<PersonListItemInfo> Collection { get; set; } = new List<PersonListItemInfo>();

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; }

    public int ItemsPerPage { get; set; }

    public int PageButtonsShown { get; set; } = 10;

    /* calculated properties */

    public int TotalPageCount { get; set; }

    public int FirstPageNumberDisplayed { get; set; }

    public int LastPageNumberDisplayed { get; set; }

    // query strings
    public string? NameSearch { get; set; }
}