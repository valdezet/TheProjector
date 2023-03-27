using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Form;

public class LoginForm
{
    [EmailAddress]
    [StringLength(200, MinimumLength = 5)]
    public string Email { get; set; }


    [StringLength(32, MinimumLength = 7)]
    public string Password { get; set; }
}