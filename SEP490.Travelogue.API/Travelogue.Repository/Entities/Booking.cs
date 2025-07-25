using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Booking : BaseEntity
{
    public Guid TourId { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid UserId { get; set; }
    public Guid TourVersionId { get; set; }
    public string? PaymentLinkId { get; set; }

    [Required]
    public DateTimeOffset BookingDate { get; set; }

    // public DateTimeOffset? ScheduledStartDate { get; set; }
    // public DateTimeOffset? ScheduledEndDate { get; set; }

    [Required]
    [EnumDataType(typeof(BookingStatus))]
    public BookingStatus Status { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }
    public bool IsOpenToJoin { get; set; } = false;

    public Guid? PromotionId { get; set; }
    public Guid? WorkshopId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal OriginalPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal FinalPrice { get; set; }

    public User? User { get; set; }
    public Tour? Tour { get; set; }
    public TourGuide? TourGuide { get; set; }
    public Workshop? Workshop { get; set; }
    public TripPlan? TripPlan { get; set; }
    public TourSchedule? TourSchedule { get; set; }

    [ForeignKey("PromotionId")]
    public Promotion? Promotion { get; set; }
    public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
}
