using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum WorkshopStatus
{
    [Display(Name = "Bản nháp")]
    Draft = 1,

    [Display(Name = "Chờ duyệt")]
    Pending = 2,

    [Display(Name = "Sẵn sàng")] // làng nghề sửa xong, có sschedule đầy đủ rồi, có thể nộp cho moderator
    Confirmed = 3,

    [Display(Name = "Đã phê duyệt")]
    Approved = 4,

    [Display(Name = "Bị từ chối")]
    Rejected = 5
}
