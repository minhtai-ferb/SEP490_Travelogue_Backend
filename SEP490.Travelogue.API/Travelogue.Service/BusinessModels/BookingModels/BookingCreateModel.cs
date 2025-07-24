using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class BookingCreateModel
{
    [Required]
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? VersionId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    public DateTime? ScheduledDate { get; set; }
    public bool IsOpenToJoin { get; set; } = false;
}
