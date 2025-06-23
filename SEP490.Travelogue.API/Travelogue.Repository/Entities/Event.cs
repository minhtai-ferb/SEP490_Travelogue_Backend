using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Event : BaseEntity
{
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
    public string? RecurrencePattern { get; set; } // Mô tả chu kỳ lặp lại 
    public bool IsHighlighted { get; set; } = false;

    // Navigation Properties
    public TypeEvent TypeEvent { get; set; } = null!;
    public Location? Location { get; set; } = null!;
    public District? District { get; set; } = null!;
    public ICollection<Experience>? Experiences { get; set; }
    public ICollection<EventMedia>? EventMedias { get; set; }
}
