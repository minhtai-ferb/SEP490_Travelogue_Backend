using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingWorkshopDto
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Guid WorkshopScheduleId { get; set; }
    public string? PromotionCode { get; set; }
    public int AdultCount { get; set; }
    public int ChildrenCount { get; set; }
}
