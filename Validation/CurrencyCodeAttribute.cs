using TheProjector.Services;
using System.ComponentModel.DataAnnotations;
namespace TheProjector.Validation;

public class CurrencyCodeAttribute : ValidationAttribute
{

    public string GetErrorMessage() => "The currency provided is either not valid or not supported.";

    protected override ValidationResult? IsValid(
        object value, ValidationContext validationContext)
    {
        string val = value.ToString()!;

        if (val.Length != 3)
        {
            return new ValidationResult(GetErrorMessage());
        }
        else
        {
            CurrencyService currencyService = validationContext.GetService<CurrencyService>()!;
            bool existingCurrency = currencyService.CheckCurrencyCodeSupport(val);
            if (!existingCurrency)
            {
                return new ValidationResult(GetErrorMessage());
            }
        }

        return ValidationResult.Success;
    }
}