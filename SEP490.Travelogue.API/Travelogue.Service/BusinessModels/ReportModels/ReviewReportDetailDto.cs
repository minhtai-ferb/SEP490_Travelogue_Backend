using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ReportModels;

public class ReviewReportDetailDto
{
    public Guid ReviewId { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ReportStatus? FinalReportStatus { get; set; }
    public Guid? HandledBy { get; set; }
    public DateTimeOffset? HandledAt { get; set; }
    public string? ModeratorNote { get; set; }

    public List<ReportResponseDto> Reports { get; set; } = new();
}