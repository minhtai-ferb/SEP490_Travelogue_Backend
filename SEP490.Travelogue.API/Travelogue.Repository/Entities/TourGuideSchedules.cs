using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideSchedule : BaseEntity
{
    [ForeignKey("TourGuide")]
    public Guid TourGuideId { get; set; }

    public Guid? BookingId { get; set; } // Foreign key to Booking
    [MaxLength(255)]
    public string? Note { get; set; }

    public DateTimeOffset Date { get; set; }

    // Navigation property
    public TourGuide TourGuide { get; set; } = null!;

    public Booking Booking { get; set; }
}