using System.ComponentModel.DataAnnotations;
using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectBasicInfo
{
    public long Id { get; set; } = 0;

    [StringLength(128)]
    [RegularExpression(@"^[a-z-0-9]+$", ErrorMessage = "The Code can only contain lowercase letters a-z and dashes(-)")]
    public string Code { get; set; }

    [StringLength(128)]
    [RegularExpression(@"^[A-Za-z0-9\s-]+[^-\s]$", ErrorMessage = "The Code can only contain letters a-z, numbers, spaces and dashes(-). It should also not end with spaces.")]
    public string Name { get; set; }

    [Range(0, 999999999999999.9999)]
    [RegularExpression(@"^\d{1,15}(\.\d{1,4})?$", ErrorMessage = "The Budget field should be a whole number or have up to at most 4 decimal places.")]
    public decimal Budget { get; set; }

    [StringLength(2048)]
    public string? Remarks { get; set; }

    public DateTime? DateArchivedUtc { get; set; }

    // Computed Properties

    public bool IsArchived => DateArchivedUtc != null;

    public string BudgetShortHand => Budget.Shorthand();
}