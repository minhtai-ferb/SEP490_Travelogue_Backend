using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ReportModels;

public class ReviewReportSummaryDto
{
    public Guid ReviewId { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public int TotalReports { get; set; }
    public int UniqueReporters { get; set; }
    public DateTimeOffset? LastReportAt { get; set; }
    public ReportStatus? FinalReportStatus { get; set; }
}