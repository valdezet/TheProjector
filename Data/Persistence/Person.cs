using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace TheProjector.Data.Persistence;

[Index("UserId", IsUnique = true)]
public class Person
{
    public long Id { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    public ICollection<Project> AssignedProjects { get; set; }

    public ICollection<Role> Roles { get; set; }

    public long UserId { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    /* calculated properties */

    public string FullName => FirstName + " " + LastName;
}