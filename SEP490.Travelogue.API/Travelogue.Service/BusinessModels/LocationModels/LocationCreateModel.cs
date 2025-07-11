using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

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

    public Guid? DistrictId { get; set; }
    [Required]
    public LocationType LocationType { get; set; }

    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    // [Required]
    // public List<LocationType> Types { get; set; } = new();
    // public CuisineCreateModel? Cuisine { get; set; }
    // public CraftVillageCreateModel? CraftVillage { get; set; }
    // public HistoricalLocationCreateModel? HistoricalLocation { get; set; }
}
