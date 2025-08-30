using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TripPlan : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string? ImageUrl { get; set; }
    public TripPlanStatus Status { get; set; } = TripPlanStatus.Draft;

    public string? PickupAddress { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }
    public ICollection<TripPlanLocation> TripPlanLocations { get; set; } = new List<TripPlanLocation>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
