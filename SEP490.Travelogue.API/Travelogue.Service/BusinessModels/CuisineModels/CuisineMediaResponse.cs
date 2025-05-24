using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CuisineModels;
public class CuisineMediaResponse
{
    public Guid CuisineId { get; set; }
    public string CuisineName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
