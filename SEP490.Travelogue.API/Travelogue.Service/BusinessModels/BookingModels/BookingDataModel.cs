using Newtonsoft.Json;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class BookingDataModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }

    public Guid? TourId { get; set; }
    public string? TourName { get; set; }

    public Guid? TourScheduleId { get; set; }
    public DateTime DepartureDate { get; set; }

    public Guid? TourGuideId { get; set; }
    public string? TourGuideName { get; set; }

    public Guid? TripPlanId { get; set; }
    public string? TripPlanName { get; set; }

    public Guid? WorkshopId { get; set; }
    public string? WorkshopName { get; set; }

    public Guid? WorkshopScheduleId { get; set; }
    public string? PaymentLinkId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? StatusText { get; set; }

    public BookingType BookingType { get; set; }
    public string? BookingTypeText { get; set; }

    public DateTimeOffset BookingDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public Guid? PromotionId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
}
