
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Persistence;


[Index("Email", IsUnique = true)]
[Index("EmailNormalized", IsUnique = true)]
public class User
{
    public long Id { get; set; }

    [MaxLength(100)]
    [MinLength(5)]
    public string Email { get; set; }

    [MaxLength(100)]
    [MinLength(5)]
    public string EmailNormalized { get; set; }

    public string PasswordHash { get; set; }

}