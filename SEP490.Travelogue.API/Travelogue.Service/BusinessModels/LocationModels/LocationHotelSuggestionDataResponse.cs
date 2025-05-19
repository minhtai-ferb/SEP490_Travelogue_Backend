using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;
public class LocationHotelSuggestionDataResponse
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; }
    public List<HotelResponse> RecommendedHotels { get; set; } = new();
}

public class HotelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
