using System.ComponentModel.DataAnnotations;

namespace TheProjector.Data.DTO;

public class ProjectBasicInfo
{
    public long Id { get; set; } = 0;

    [StringLength(128)]
    [RegularExpression(@"^[a-z-]+$", ErrorMessage = "The Code can only contain lowercase letters a-z and dashes(-)")]
    public string Code { get; set; }

    [StringLength(128)]
    [RegularExpression(@"^[A-Za-z\s-]+[^-\s]$")]
    public string Name { get; set; }


    public decimal Budget { get; set; }

    [StringLength(2048)]
    public string? Remarks { get; set; }
}