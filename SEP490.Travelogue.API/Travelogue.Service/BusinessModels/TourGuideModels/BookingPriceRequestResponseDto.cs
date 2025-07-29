using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class BookingPriceRequestResponseDto
{
    public Guid TourGuideId { get; set; }
    public decimal Price { get; set; }

    public BookingPriceRequestStatus Status { get; set; } = BookingPriceRequestStatus.Pending;
    public string? StatusText { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}
