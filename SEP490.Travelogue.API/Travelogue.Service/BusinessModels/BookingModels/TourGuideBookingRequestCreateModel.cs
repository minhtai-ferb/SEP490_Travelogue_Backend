using System;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class TourGuideBookingRequestCreateModel
{
    public Guid TripPlanId { get; set; }
    public Guid TripPlanVersionId { get; set; }
    public Guid TourGuideId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? UserResponseMessage { get; set; }
}
