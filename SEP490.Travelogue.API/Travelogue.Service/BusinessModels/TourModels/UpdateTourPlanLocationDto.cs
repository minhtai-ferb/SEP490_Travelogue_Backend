using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TourModels;

public class UpdateTourPlanLocationDto
{
    public Guid? TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    [Range(1, int.MaxValue)]
    public int DayOrder { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}
