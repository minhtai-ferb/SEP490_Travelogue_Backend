using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class CraftVillage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    //public decimal? StarRating { get; set; }
    public decimal? PricePerNight { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
    //public Guid? LocationId { get; set; }

    // Navigation Properties
    //public Location? Location { get; set; }
    public ICollection<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; } = new List<LocationCraftVillageSuggestion>();
    public ICollection<TripPlanCraftVillage> TripPlanCraftVillages { get; set; } = new List<TripPlanCraftVillage>();
}
