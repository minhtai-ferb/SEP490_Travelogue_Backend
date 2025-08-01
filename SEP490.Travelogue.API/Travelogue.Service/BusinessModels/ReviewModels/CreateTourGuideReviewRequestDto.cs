using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.ReviewModels;

public class CreateTourGuideReviewRequestDto
{
    public Guid BookingId { get; set; }
    public Guid TourGuideId { get; set; }
    public string? Comment { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
}
