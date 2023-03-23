using System.ComponentModel.DataAnnotations;
using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectBasicInfo
{
    public long Id { get; set; } = 0;

    public string Code { get; set; }

    public string Name { get; set; }

    public string? Remarks { get; set; }

    public DateTime? DateArchivedUtc { get; set; }

    public bool IsArchived { get; set; }

    public string BudgetShorthand { get; set; }

    public string BudgetLocalized { get; set; }
}