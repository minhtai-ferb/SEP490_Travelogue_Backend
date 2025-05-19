using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;
public class LocationMediaResponse
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
