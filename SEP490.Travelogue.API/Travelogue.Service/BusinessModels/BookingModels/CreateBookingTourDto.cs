using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class CreateBookingTourDto
{
    [Required]
    public Guid TourId { get; set; }
    [Required]
    public Guid ScheduledId { get; set; }
    public string? PromotionCode { get; set; }

    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? ContactAddress { get; set; }

    public List<CreateBookingParticipantDto> Participants { get; set; } = new();
}

public class CreateBookingParticipantDto
{
    public ParticipantType Type { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
}
