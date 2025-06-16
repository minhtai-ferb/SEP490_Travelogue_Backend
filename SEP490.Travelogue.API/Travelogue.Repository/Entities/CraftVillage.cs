using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CraftVillage : BaseEntity
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
    public Guid? LocationId { get; set; }

    public bool WorkshopsAvailable { get; set; } = false;

    // Navigation Properties
    public Location? Location { get; set; } = null!;
    public ICollection<LocationCraftVillageSuggestion> LocationCraftVillageSuggestions { get; set; } = new List<LocationCraftVillageSuggestion>();
    // public ICollection<TripPlanCraftVillage> TripPlanCraftVillages { get; set; } = new List<TripPlanCraftVillage>();
}
