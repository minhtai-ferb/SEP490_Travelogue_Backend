using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class RejectionRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid TourGuideId { get; set; }
    public RejectionRequestType RequestType { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? BookingId { get; set; }
    public string Reason { get; set; } = null!;
    public RejectionRequestStatus Status { get; set; }
    public string? StatusText { get; set; } = null!;
    public string? ModeratorComment { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}