using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideSchedule : BaseEntity
{
    public Guid? TourScheduleId { get; set; }

    [Required]
    public Guid TourGuideId { get; set; }

    public DateTimeOffset Date { get; set; }
    public string? Note { get; set; }
    public Guid? BookingId { get; set; }

    // Navigation properties
    [ForeignKey("TourScheduleId")]
    public TourSchedule TourSchedule { get; set; } = null!;

    [ForeignKey("TourGuideId")]
    public TourGuide TourGuide { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
