using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.ReviewModels;

public class CreateTourReviewRequestDto
{
    public Guid BookingId { get; set; }
    public Guid TourId { get; set; }
    public string? Comment { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
}
