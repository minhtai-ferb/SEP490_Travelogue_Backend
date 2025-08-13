using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum WorkshopStatus
{
    [Display(Name = "Bản nháp")]
    Draft = 1,

    [Display(Name = "Chờ duyệt")]
    Pending = 2,

    // [Display(Name = "Cần chỉnh sửa")]
    // NeedRevision = 3,

    [Display(Name = "Đã phê duyệt")]
    Approved = 4,

    [Display(Name = "Bị từ chối")]
    Rejected = 5
}
