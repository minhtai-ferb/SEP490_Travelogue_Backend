using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TripPlanModels;
public class TripPlanCreateModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
}
