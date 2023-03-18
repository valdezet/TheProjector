using System.ComponentModel.DataAnnotations;


namespace TheProjector.Data.Persistence;

public class Person
{
    public long Id { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }


    // TODO user relation
}