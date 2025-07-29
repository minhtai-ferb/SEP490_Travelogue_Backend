using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingTourGuideDto
{
    public Guid TourGuideId { get; set; }

    public Guid? TripPlanId { get; set; }

    [Required]
    public DateTimeOffset Date { get; set; }

    public int AdultCount { get; set; }
    public int ChildrenCount { get; set; }

    public string? PromotionCode { get; set; }
    public string? Note { get; set; }
}
