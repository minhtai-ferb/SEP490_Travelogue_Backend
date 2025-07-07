using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.DistrictModels;

public class DistrictMediaResponse
{
    public Guid DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
