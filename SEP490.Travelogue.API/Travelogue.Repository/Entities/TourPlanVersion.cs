using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanVersion : BaseEntity
{
    public Guid TourId { get; set; }
    public Tour? Tour { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public DateTimeOffset VersionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    // public bool IsFromTourGuide { get; set; } = false;
    public int VersionNumber { get; set; } = 1;
    public string? Notes { get; set; } = string.Empty;
    // public string Status { get; set; } = "Draft";

    public ICollection<TourPlanLocation> TourPlanLocations { get; set; } = new List<TourPlanLocation>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}