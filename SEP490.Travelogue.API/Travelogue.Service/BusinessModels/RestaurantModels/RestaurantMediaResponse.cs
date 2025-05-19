using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.RestaurantModels;
public class RestaurantMediaResponse
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
