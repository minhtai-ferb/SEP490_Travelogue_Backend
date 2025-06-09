using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public class TripPlanVersion : BaseEntity
{
    public Guid TripPlanId { get; set; }
    public TripPlan? TripPlan { get; set; }

    public DateTimeOffset VersionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;

    public int VersionNumber { get; set; } = 1;
    public string? Notes { get; set; } = null;
    public string Status { get; set; } = "Draft";

    public ICollection<TripPlanCraftVillage>? TripPlanCraftVillages { get; set; }
    public ICollection<TripPlanCuisine>? TripPlanCuisines { get; set; }
    public ICollection<TripPlanLocation>? TripPlanLocations { get; set; }
}
