using Travelogue.Service.BusinessModels.TourGuideModels;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourScheduleResponseDto
{
    public Guid ScheduleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipant { get; set; }
    public int CurrentBooked { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public string? Notes { get; set; }
    public TourGuideDataModel? TourGuide { get; set; }
}
