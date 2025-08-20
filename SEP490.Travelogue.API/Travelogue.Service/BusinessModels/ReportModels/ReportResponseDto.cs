using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ReportModels;

public class ReportResponseDto : BaseDataModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ReviewId { get; set; }
    public string? Reason { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
}
public class ProcessReportRequestDto
{
    public ReportStatus Status { get; set; }
    public string? Note { get; set; }
}