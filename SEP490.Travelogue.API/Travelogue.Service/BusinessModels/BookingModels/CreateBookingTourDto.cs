using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingTourDto
{
    [Required]
    public Guid TourId { get; set; }
    [Required]
    public Guid ScheduledId { get; set; }
    public string? PromotionCode { get; set; }
    public int AdultCount { get; set; }
    public int ChildrenCount { get; set; }
}
