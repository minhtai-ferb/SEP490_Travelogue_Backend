namespace Travelogue.Service.BusinessModels.TourModels;

public class TourPlanLocationResponseDto
{
    public Guid TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    public float TravelTimeFromPrev { get; set; }
    public float DistanceFromPrev { get; set; }
    public float EstimatedStartTime { get; set; }
    public float EstimatedEndTime { get; set; }
}
