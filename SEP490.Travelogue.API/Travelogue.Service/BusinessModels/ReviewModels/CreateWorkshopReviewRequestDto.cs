using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.ReviewModels;

public class CreateWorkshopReviewRequestDto
{
    public Guid BookingId { get; set; }
    public Guid WorkshopId { get; set; }
    public string? Comment { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
}