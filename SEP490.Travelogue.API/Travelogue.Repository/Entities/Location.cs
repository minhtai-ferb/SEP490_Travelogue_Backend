using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Location : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    //[Range(0, 5)]
    //public double Rating { get; set; } = 0;
    public HeritageRank HeritageRank { get; set; }
    public Guid? TypeLocationId { get; set; }
    public Guid? DistrictId { get; set; }

    // Navigation Properties
    public District? District { get; set; }
    public TypeLocation? TypeLocation { get; set; }
    public ICollection<Experience>? Experiences { get; set; }
    public ICollection<Event>? Activities { get; set; }
    public ICollection<LocationMedia>? LocationMedias { get; set; }
    public ICollection<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; } = new List<LocationCraftVillageSuggestion>();
    public ICollection<LocationCuisineSuggestion> LocationCuisineSuggestions { get; set; } = new List<LocationCuisineSuggestion>();
    public ICollection<TripPlanLocation> TripPlanLocations { get; set; } = new List<TripPlanLocation>();
    public ICollection<FavoriteLocation> FavoriteLocations { get; set; } = new List<FavoriteLocation>();
    public ICollection<LocationInterest> LocationInterests { get; set; } = new List<LocationInterest>();
    public ICollection<TourPlan> TourPlans { get; set; } = new List<TourPlan>();
}
