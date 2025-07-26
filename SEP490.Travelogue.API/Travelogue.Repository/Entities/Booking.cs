using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? WorkshopScheduleId { get; set; }
    public Guid? WorkshopId { get; set; }
    public string? PaymentLinkId { get; set; }

    // public DateTimeOffset? ScheduledStartDate { get; set; }
    // public DateTimeOffset? ScheduledEndDate { get; set; }

    [Required]
    [EnumDataType(typeof(BookingStatus))]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    [Required]
    [EnumDataType(typeof(BookingType))]
    public BookingType BookingType { get; set; }

    [Required]
    public DateTimeOffset BookingDate { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }

    public Guid? PromotionId { get; set; }

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
    public TourSchedule? TourSchedule { get; set; }
    public TourGuide? TourGuide { get; set; }
    public Workshop? Workshop { get; set; }
    public WorkshopSchedule? WorkshopSchedule { get; set; }
    public TripPlan? TripPlan { get; set; }

    [ForeignKey("PromotionId")]
    public Promotion? Promotion { get; set; }
    public ICollection<TransactionEntry> Transactions { get; set; } = new List<TransactionEntry>();
    public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
}
