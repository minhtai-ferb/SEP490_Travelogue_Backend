namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class CreateScheduleDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipant { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public string? Notes { get; set; }
}
