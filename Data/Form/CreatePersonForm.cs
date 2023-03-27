using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Form;

public class CreatePersonForm
{

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s,'-]+$", ErrorMessage = "Only English Alphabet letters, dashes (-), spaces, and comma is allowed.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s,'-]+$", ErrorMessage = "Only English Alphabet letters, dashes (-), spaces, and comma is allowed.")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [EmailAddress]
    [StringLength(200, MinimumLength = 5)]
    public string Email { get; set; }


    [StringLength(32, MinimumLength = 7)]
    public string Password { get; set; }
}