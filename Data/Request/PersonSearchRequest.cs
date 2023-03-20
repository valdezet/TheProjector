using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Request;
public class PersonSearchRequest : PaginatedRequest
{
    [StringLength(128)]
    public string? Name { get; set; }
}