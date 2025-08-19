using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum NotificationType
{
    // Hệ thống -> Admin
    [Display(Name = "Yêu cầu rút tiền")]
    WithdrawRequest = 1,

    [Display(Name = "Yêu cầu hoàn tiền")]
    RefundRequest = 2,

    [Display(Name = "Báo cáo đánh giá")]
    ReportRequest = 3,

    // Hệ thống -> User
    [Display(Name = "Phản hồi rút tiền")]
    WithdrawResponse = 10,

    [Display(Name = "Phản hồi hoàn tiền")]
    RefundResponse = 11,

    [Display(Name = "Đánh giá bị tố cáo")]
    AccountReported = 12,

    // Chung
    [Display(Name = "Thông báo hệ thống")]
    System = 20,

    [Display(Name = "Thông báo sự kiện")]
    Event = 21,

    [Display(Name = "Thông báo đặt chỗ")]
    Booking = 22,

    [Display(Name = "Thông báo tour")]
    Tour = 23
}