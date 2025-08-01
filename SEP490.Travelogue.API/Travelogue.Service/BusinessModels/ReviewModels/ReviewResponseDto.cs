namespace Travelogue.Service.BusinessModels.ReviewModels;

public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? WorkshopId { get; set; }
    public Guid? TourGuideId { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}