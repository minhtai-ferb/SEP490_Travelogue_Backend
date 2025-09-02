using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class BookingPriceRequestListItemDto
{
    public Guid Id { get; set; }

    public Guid TourGuideId { get; set; }
    public string TourGuideName { get; set; } = string.Empty;

    public decimal RequestedPrice { get; set; }

    public BookingPriceRequestStatus Status { get; set; }
    public string? StatusText { get; set; } = string.Empty;

    public string? RejectionReason { get; set; }

    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
}
