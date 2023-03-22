using System.ComponentModel.DataAnnotations;
using TheProjector.Validation;

namespace TheProjector.Data.Form;

public class ProjectForm
{
    public long Id { get; set; } = 0;

    [StringLength(128)]
    [RegularExpression(@"^[a-z-0-9]+$", ErrorMessage = "The Code can only contain lowercase letters a-z and dashes(-)")]
    public string Code { get; set; }

    [StringLength(128)]
    [RegularExpression(@"^[A-Za-z0-9\s-]+[^-\s]$", ErrorMessage = "The Code can only contain letters a-z, numbers, spaces and dashes(-). It should also not end with spaces.")]
    [DisplayFormat(DataFormatString = "#,##.00")]
    public string Name { get; set; }

    [Range(0, 999999999999999.9999)]
    [DataType(DataType.Currency)]
    public decimal Budget { get; set; }

    [CurrencyCode]
    public string BudgetCurrencyCode { get; set; } = System.Globalization.RegionInfo.CurrentRegion.ISOCurrencySymbol;

    [StringLength(2048)]
    public string? Remarks { get; set; }
}