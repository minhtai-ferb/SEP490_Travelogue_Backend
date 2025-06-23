using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HotelModels;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationDataDetailModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; } = 0;
    public List<string>? Categories { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
    public HotelDataModel? Hotel { get; set; }
    public CuisineDataModel? Cuisine { get; set; }
    public CraftVillageDataModel? CraftVillage { get; set; }
    public HistoricalLocation? HistoricalLocation { get; set; } = null!;
}
