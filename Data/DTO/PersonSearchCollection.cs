namespace TheProjector.Data.DTO;


public class PersonSearchCollection
{
    public ICollection<PersonListItemInfo> Collection { get; set; } = new List<PersonListItemInfo>();

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; }

    public int ItemsPerPage { get; set; }

    public int PageButtonsShown { get; set; } = 10;

    public int TotalPageCount { get; set; }

    public int FirstPageNumberDisplayed { get; set; }

    public int LastPageNumberDisplayed { get; set; }

    public int[] ItemsPerPageSelection = new int[] { 1, 2, 5, 10, 25, 50 };

    // query strings
    public string? NameSearch { get; set; }
}