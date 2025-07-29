using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class RejectionRequest : BaseEntity
{
    [Required]
    public Guid TourGuideId { get; set; }
    public TourGuide TourGuide { get; set; } = null!;

    [Required]
    public RejectionRequestType RequestType { get; set; }

    public Guid? TourScheduleId { get; set; }
    public TourSchedule? TourSchedule { get; set; }

    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }

    [Required]
    public string Reason { get; set; } = null!;

    public RejectionRequestStatus Status { get; set; } = RejectionRequestStatus.Pending;

    public string? ModeratorComment { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}