using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class RefundRequest : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid BookingId { get; set; }
    public string? Reason { get; set; }

    [Required]
    public RefundRequestStatus Status { get; set; } = RefundRequestStatus.Pending;
    public string? RejectionReason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RefundAmount { get; set; }
    public Booking Booking { get; set; } = null!;
    public User User { get; set; } = null!;
}
