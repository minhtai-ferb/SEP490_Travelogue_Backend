namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class CreateActivityDto
{
    public string Activity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Notes { get; set; }
    public int DayOrder { get; set; }
}
