using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingTourGuideDto
{
    public Guid TourGuideId { get; set; }

    public Guid? TripPlanId { get; set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public string? PromotionCode { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? ContactAddress { get; set; }

    // Danh sách hành khách
    public List<CreateBookingParticipantDto> Participants { get; set; } = new();
}
