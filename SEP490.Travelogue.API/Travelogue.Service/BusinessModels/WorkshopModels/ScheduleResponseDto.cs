namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class ScheduleResponseDto
{
    public Guid ScheduleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipant { get; set; }
    public int CurrentBooked { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public string? Notes { get; set; }
}
