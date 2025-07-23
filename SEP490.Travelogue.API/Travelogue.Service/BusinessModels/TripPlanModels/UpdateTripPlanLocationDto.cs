namespace Travelogue.Service.BusinessModels.TripPlanModels;

public class UpdateTripPlanLocationDto
{
    public Guid? TripPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public int Order { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
    public float? TravelTimeFromPrev { get; set; }
    public float? DistanceFromPrev { get; set; }
    public float? EstimatedStartTime { get; set; }
    public float? EstimatedEndTime { get; set; }
}
