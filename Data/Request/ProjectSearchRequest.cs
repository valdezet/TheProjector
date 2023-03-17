using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Request;
public class ProjectSearchRequest : PaginatedRequest
{
    [StringLength(128)]
    public string? Name { get; set; }
}