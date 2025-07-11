namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingRequest
{
    public Guid TourId { get; set; }
    public Guid? TourScheduleId { get; set; } // Null for custom trip plans
    public Guid? TripPlanVersionId { get; set; }
    public Guid TourGuideId { get; set; }
    public int AdultCount { get; set; }
    public int ChildCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}