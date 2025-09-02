using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourModels;

public class UpdateTourPlanLocationDto
{
    public Guid? TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    [Range(1, int.MaxValue)]
    public int DayOrder { get; set; }

    public ActivityType ActivityType { get; set; } = ActivityType.Sightseeing;

    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    public float TravelTimeFromPrev { get; set; }
    public float DistanceFromPrev { get; set; }

    // public Guid? WorkshopId { get; set; }
    public Guid? WorkshopTicketTypeId { get; set; }
    public Guid? WorkshopSessionRuleId { get; set; }
}
