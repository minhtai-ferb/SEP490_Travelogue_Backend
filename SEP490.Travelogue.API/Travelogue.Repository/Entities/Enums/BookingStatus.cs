using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum BookingStatus
{
    [Display(Name = "Đang chờ thanh toán")]
    Pending = 0,

    [Display(Name = "Đã thanh toán")]
    Confirmed = 1,

    [Display(Name = "Bị hủy chưa thanh toán")]
    CancelledUnpaid = 2,

    [Display(Name = "Bị hủy đã thanh toán")]
    Cancelled = 3,

    [Display(Name = "Bị hủy bởi nhà cung cấp")]
    CancelledByProvider = 4,

    [Display(Name = "Đã hoàn thành")]
    Completed = 5,
    [Display(Name = "Hết hạn")]
    Expired = 6,
}