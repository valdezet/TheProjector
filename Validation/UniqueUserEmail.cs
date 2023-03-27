using TheProjector.Utilities;
using System.ComponentModel.DataAnnotations;
using TheProjector.Data.Persistence;

namespace TheProjector.Validation;

public class UniqueUserEmailAttribute : ValidationAttribute
{
    public string GetErrorMessage() => "The email is already taken by another user.";

    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        TheProjectorDbContext dbContext = (TheProjectorDbContext)validationContext.GetRequiredService(typeof(TheProjectorDbContext));
        if (value == null)
        {
            return new ValidationResult(GetErrorMessage());
        }
        string val = value.ToString()!;

        bool exists = dbContext.Users.Any(user => user.EmailNormalized == val.ToUpper());
        if (!exists)
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult(GetErrorMessage());
        }
    }
}