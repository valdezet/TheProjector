using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Request;
public class PersonSearchRequest
{
    [Range(1, Int32.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(5, 100)]
    public int ItemsPerPage { get; set; } = 10;

    [StringLength(128)]
    public string? Name { get; set; }
}