using System.ComponentModel.DataAnnotations;
using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectBasicInfo
{
    public long Id { get; set; } = 0;

    public string Code { get; set; }

    public string Name { get; set; }

    public decimal Budget { get; set; }

    public string? Remarks { get; set; }

    public DateTime? DateArchivedUtc { get; set; }

    // Computed Properties

    public bool IsArchived => DateArchivedUtc != null;

    public string BudgetShortHand => Budget.Shorthand();
}