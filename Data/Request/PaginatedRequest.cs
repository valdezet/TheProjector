using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Request;

public class PaginatedRequest
{
    [Range(1, Int32.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(5, 100)]
    public int ItemsPerPage { get; set; } = 10;
}