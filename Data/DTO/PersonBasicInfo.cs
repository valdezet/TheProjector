using System.ComponentModel.DataAnnotations;
using TheProjector.Extensions;

namespace TheProjector.Data.DTO;

public class PersonBasicInfo
{
    public long Id { get; set; } = 0;

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s,'-]+$", ErrorMessage = "Only English Alphabet letters, dashes (-), spaces, and comma is allowed.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [StringLength(50)]
    [RegularExpression(@"^[a-zA-Z\s,'-]+$", ErrorMessage = "Only English Alphabet letters, dashes (-), spaces, and comma is allowed.")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    /* computed properties, do not handle in CRUD. */

    public string FullName => FirstName + " " + LastName;
}