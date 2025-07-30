using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourMediaResponse
{
    public Guid TourId { get; set; }
    public string? TourName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
