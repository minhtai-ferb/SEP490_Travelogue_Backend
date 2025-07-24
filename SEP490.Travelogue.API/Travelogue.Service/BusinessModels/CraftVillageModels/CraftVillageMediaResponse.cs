using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageMediaResponse
{
    public Guid CraftVillageId { get; set; }
    public string? CraftVillageName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
