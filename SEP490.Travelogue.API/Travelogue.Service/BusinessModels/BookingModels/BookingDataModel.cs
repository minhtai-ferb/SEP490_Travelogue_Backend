using Newtonsoft.Json;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class BookingDataModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? TripPlanId { get; set; }
    public Guid? WorkshopId { get; set; }
    public Guid? WorkshopScheduleId { get; set; }
    public string? PaymentLinkId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? StatusText { get; set; }
    // {
    //     return Status switch
    //     {
    //         BookingStatus.Pending => "Draft",
    //         BookingStatus.Confirmed => "Confirmed",
    //         BookingStatus.Cancelled => "Cancelled",
    //         _ => "Unknown"
    //     };
    // }

    public BookingType BookingType { get; set; }
    public string? BookingTypeText { get; set; }

    public DateTimeOffset BookingDate { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public Guid? PromotionId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
}
