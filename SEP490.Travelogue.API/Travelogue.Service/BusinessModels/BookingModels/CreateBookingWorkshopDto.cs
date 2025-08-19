using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingWorkshopDto
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Guid WorkshopScheduleId { get; set; }
    public string? PromotionCode { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? ContactAddress { get; set; }

    public List<CreateBookingParticipantDto> Participants { get; set; } = new();
}
