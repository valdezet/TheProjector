using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectListItemInfo
{
    public long Id { get; set; }
    public string Name { get; set; }

    public decimal Budget { get; set; }

    public string BudgetCurrencyCode { get; set; }

    /* calculated properties */

    public string BudgetShorthand => $"{BudgetCurrencyCode} {Budget.Shorthand()}";

    public string BudgetLocalized => $"{BudgetCurrencyCode} {Budget.Localized()}";

}