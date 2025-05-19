using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;
public class LocationRestaurantSuggestionDataResponse
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; }
    public List<RestaurantResponse> RecommendedRestaurants { get; set; } = new();
}

public class RestaurantResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
