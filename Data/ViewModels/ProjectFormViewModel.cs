using TheProjector.Data.Form;
using TheProjector.Data.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TheProjector.Data.ViewModels;


public class ProjectFormViewModel
{
    public ProjectForm Form { get; set; }

    [ValidateNever]
    public IEnumerable<CurrencyInfo> Currencies { get; set; }
}