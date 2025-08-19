using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TransactionType
{
    [Display(Name = "Đặt tour")]
    Booking = 1,

    [Display(Name = "Rút tiền")]
    Withdraw = 2,

    [Display(Name = "Hoàn tiền")]
    Refund = 3,
}