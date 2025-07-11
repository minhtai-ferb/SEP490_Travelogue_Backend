namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class ActivityResponseDto
{
    public Guid ActivityId { get; set; }
    public string Activity { get; set; }
    public string Description { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Notes { get; set; }
    public int DayOrder { get; set; }
}
