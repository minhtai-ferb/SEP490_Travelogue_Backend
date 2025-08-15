using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class BookingPriceRequest : BaseEntity
{
    [Required]
    public Guid TourGuideId { get; set; }

    [Range(10000, double.MaxValue)]
    public decimal Price { get; set; }

    public BookingPriceRequestStatus Status { get; set; } = BookingPriceRequestStatus.Pending;

    public string? RejectionReason { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public TourGuide TourGuide { get; set; } = null!;
}
