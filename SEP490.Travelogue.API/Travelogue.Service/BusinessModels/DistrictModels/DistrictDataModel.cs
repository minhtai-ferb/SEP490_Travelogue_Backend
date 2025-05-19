using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.DistrictModels;
public class DistrictDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
    public float? Area { get; set; }
}
