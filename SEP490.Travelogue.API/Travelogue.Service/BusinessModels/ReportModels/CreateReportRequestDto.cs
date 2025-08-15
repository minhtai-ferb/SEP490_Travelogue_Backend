namespace Travelogue.Service.BusinessModels.ReportModels;

public class CreateReportRequestDto
{
    public Guid ReviewId { get; set; }
    public string? Reason { get; set; }
}
