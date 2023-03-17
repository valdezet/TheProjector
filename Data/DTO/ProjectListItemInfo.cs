using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectListItemInfo
{
    public long Id { get; set; }
    public string Name { get; set; }

    public decimal Budget { get; set; }

    /* calculated properties */

    public string BudgetShorthand => Budget.Shorthand();

}