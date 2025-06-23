using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public class TripPlanVersion : BaseEntity
{
    public Guid TripPlanId { get; set; }
    public TripPlan? TripPlan { get; set; }

    public DateTimeOffset VersionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public bool IsFromTourGuide { get; set; } = false;
    public int VersionNumber { get; set; } = 1;
    public string? Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";

    public ICollection<TripPlanLocation>? TripPlanLocations { get; set; }
}
