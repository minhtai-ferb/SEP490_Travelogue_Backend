using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelogue.Service.BusinessModels.ReportModels;
public class ReviewReportDetailDto
{
    public Guid ReviewId { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public List<ReportResponseDto> Reports { get; set; } = new();
}