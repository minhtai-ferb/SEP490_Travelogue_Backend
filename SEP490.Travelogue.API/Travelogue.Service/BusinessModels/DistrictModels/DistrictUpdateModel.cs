using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.DistrictModels;
public class DistrictUpdateModel
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public float? Area { get; set; }
}
