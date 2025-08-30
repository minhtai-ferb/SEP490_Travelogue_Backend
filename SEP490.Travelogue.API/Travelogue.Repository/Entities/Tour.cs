using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Tour : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? TransportType { get; set; }
    [MaxLength(300)]
    public string? PickupAddress { get; set; }
    [MaxLength(500)]
    public string? StayInfo { get; set; }

    public int TotalDays { get; set; }

    public TourStatus Status { get; set; } = TourStatus.Draft;
    public TourType? TourType { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TourPlanLocation> TourPlanLocations { get; set; } = new List<TourPlanLocation>();
    // public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<TourSchedule> TourSchedules { get; set; } = new List<TourSchedule>();
    public ICollection<PromotionApplicable> PromotionApplicables { get; set; } = new List<PromotionApplicable>();
    public ICollection<TourInterest> TourInterests { get; set; } = new List<TourInterest>();
    // public ICollection<TourPlanLocation> TourPlanLocations { get; set; } = new List<TourPlanLocation>();
    // public ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();
}
