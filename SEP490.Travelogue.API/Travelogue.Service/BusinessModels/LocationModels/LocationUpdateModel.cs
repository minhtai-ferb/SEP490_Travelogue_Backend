using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HistoricalLocationModels;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationUpdateModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    public double MinPrice { get; set; } = 0;
    public double MaxPrice { get; set; } = 0;

    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(0, 5)]
    public double Rating { get; set; } = 0;

    [Required]
    public List<LocationType> Types { get; set; } = new();
    public Guid? DistrictId { get; set; }
    public HeritageRank HeritageRank { get; set; }
    public CuisineCreateModel? Cuisine { get; set; }
    public CraftVillageCreateModel? CraftVillage { get; set; }
    public HistoricalLocationCreateModel? HistoricalLocation { get; set; }
}
