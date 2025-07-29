using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class RejectionRequestCreateDto
{
    public RejectionRequestType RequestType { get; set; }
    public Guid? TourScheduleId { get; set; }
    public Guid? BookingId { get; set; }
    public string Reason { get; set; } = null!;
}
