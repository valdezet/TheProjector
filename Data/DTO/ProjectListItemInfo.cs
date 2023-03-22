using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class ProjectListItemInfo
{
    public long Id { get; set; }
    public string Name { get; set; }

    public string BudgetShorthand { get; set; }

    public string BudgetLocalized { get; set; }

}