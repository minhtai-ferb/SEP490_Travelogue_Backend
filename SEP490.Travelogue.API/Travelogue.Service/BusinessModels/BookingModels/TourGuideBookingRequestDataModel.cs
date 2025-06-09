using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class TourGuideBookingRequestDataModel
{
    public Guid UserId { get; set; }
    public Guid TripPlanId { get; set; }
    public Guid TripPlanVersionId { get; set; }
    public Guid TourGuideId { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public BookingRequestStatus Status { get; set; }

    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? UserRespondedAt { get; set; }

    public string? UserResponseMessage { get; set; }
}
