using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Location : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }

    // [Range(0, 5)]
    // public double Rating { get; set; } = 0;
    public Guid? DistrictId { get; set; }

    // Navigation Properties
    public CraftVillage? CraftVillage { get; set; }
    public HistoricalLocation? HistoricalLocation { get; set; }
    public Cuisine? Cuisine { get; set; }
    public District? District { get; set; }
    public LocationType LocationType { get; set; }
    // public ICollection<LocationTypeMapping> LocationTypes { get; set; } = new List<LocationTypeMapping>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public ICollection<Event> Activities { get; set; } = new List<Event>();
    public ICollection<LocationMedia> LocationMedias { get; set; } = new List<LocationMedia>();
    public ICollection<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; } = new List<LocationCraftVillageSuggestion>();
    public ICollection<LocationCuisineSuggestion> LocationCuisineSuggestions { get; set; } = new List<LocationCuisineSuggestion>();
    public ICollection<TripPlanLocation> TripPlanLocations { get; set; } = new List<TripPlanLocation>();
    public ICollection<FavoriteLocation> FavoriteLocations { get; set; } = new List<FavoriteLocation>();
    public ICollection<LocationInterest> LocationInterests { get; set; } = new List<LocationInterest>();
}
