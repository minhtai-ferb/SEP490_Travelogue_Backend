using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourPlanLocationResponseDto
{
    public Guid TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public ActivityType ActivityType { get; set; } = ActivityType.Sightseeing;
    public string? ActivityTypeText { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    public float TravelTimeFromPrev { get; set; }
    public float DistanceFromPrev { get; set; }
}
