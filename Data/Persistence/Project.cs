using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TheProjector.Data.Persistence;

public class Project
{
    public long Id { get; set; }

    [StringLength(128)]
    public string Code { get; set; }

    [StringLength(128)]
    public string Name { get; set; }

    [Column(TypeName = "decimal(19,4)")]
    public decimal Budget { get; set; }

    [StringLength(2048)]
    public string? Remarks { get; set; }
}