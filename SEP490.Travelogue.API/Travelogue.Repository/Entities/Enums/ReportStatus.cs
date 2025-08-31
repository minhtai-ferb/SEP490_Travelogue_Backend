using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ReportStatus
{
    [Display(Name = "Đang xử lý")]
    Pending = 1,

    [Display(Name = "Đã xử lý")]
    Processed = 2,

    [Display(Name = "Bị từ chối")]
    Rejected = 3
}
