using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.EventModels;
public class EventDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? TypeEventId { get; set; }
    public string? TypeEventName { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }

    public string? LunarStartDate { get; set; }
    public string? LunarEndDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsRecurring { get; set; } = false; // Hoạt động có lặp lại không?
    public string? RecurrencePattern { get; set; }
    public bool IsHighlighted { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}

public class ImageModel
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
}
