using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum NotificationType
{
    [Display(Name = "Tài khoản")]
    Account = 1,

    [Display(Name = "Tương tác")]
    Interaction = 2,

    [Display(Name = "Đặt chỗ")]
    Booking = 3,

    [Display(Name = "Hệ thống")]
    System = 4
}