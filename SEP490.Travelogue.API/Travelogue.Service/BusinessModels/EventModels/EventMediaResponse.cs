using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.EventModels;
public class EventMediaResponse
{
    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
