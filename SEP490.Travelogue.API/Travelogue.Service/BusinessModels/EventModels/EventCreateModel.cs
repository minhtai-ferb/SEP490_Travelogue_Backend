using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.EventModels;
public class EventCreateModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid TypeEventId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? LunarStartDate { get; set; }
    public string? LunarEndDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsRecurring { get; set; } = false; // Hoạt động có lặp lại không?
    public string? RecurrencePattern { get; set; }
    public bool IsHighlighted { get; set; } = false;
}
