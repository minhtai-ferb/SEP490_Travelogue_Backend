namespace Travelogue.Service.BusinessModels.ReviewModels;

public class RatingDetailsDto
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
}
