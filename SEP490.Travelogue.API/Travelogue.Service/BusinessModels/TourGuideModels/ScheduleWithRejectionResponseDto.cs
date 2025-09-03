namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class ScheduleWithRejectionResponseDto
{
    public Guid Id { get; set; }
    public Guid TourGuideId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? BookingId { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? Note { get; set; }
    public string? TourName { get; set; }
    public string? CustomerName { get; set; }
    public decimal? Price { get; set; }
    public string ScheduleType { get; set; } = null!;
    public RejectionRequestResponseDto? RejectionRequest { get; set; }
}