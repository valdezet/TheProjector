using System.ComponentModel.DataAnnotations;


namespace TheProjector.Data.Persistence;

public class Person
{
    public long Id { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }


    public ICollection<Project> AssignedProjects { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    /* calculated properties */

    public string FullName => FirstName + " " + LastName;
}