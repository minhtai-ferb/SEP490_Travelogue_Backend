using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HistoricalLocationModels;
using Travelogue.Service.BusinessModels.HotelModels;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationCreateModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Required]
    public List<LocationType> Types { get; set; } = new();
    public Guid? DistrictId { get; set; }
    public HotelCreateModel? Hotel { get; set; }
    public CuisineCreateModel? Cuisine { get; set; }
    public CraftVillageCreateModel? CraftVillage { get; set; }
    public HistoricalLocationCreateModel? HistoricalLocation { get; set; }
}
