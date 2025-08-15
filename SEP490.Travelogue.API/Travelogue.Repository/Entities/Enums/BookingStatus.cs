using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum BookingStatus
{
    [Display(Name = "Đang chờ xác nhận")]
    Pending = 0,

    [Display(Name = "Đã xác nhận")]
    Confirmed = 1,

    [Display(Name = "Bị hủy")]
    Cancelled = 2,

    [Display(Name = "Hết hạn")]
    Expired = 3,

    [Display(Name = "Bị hủy bởi nhà cung cấp")]
    CancelledByProvider = 4
}
