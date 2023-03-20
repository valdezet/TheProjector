namespace TheProjector.Data.DTO;


public class PersonSearchCollection : PaginatedCollection<PersonListItemInfo>
{
    // query strings
    public string? NameSearch { get; set; }
}