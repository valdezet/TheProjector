using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.Persistence;

[Index("Name", IsUnique = true)]
public class Role
{
    public long Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    public ICollection<Person> People { get; set; }
}