using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum SystemTransactionKind
{
    [Display(Name = "Khách thanh toán")]
    IncomingTourPayment = 1, // khách thanh toán tour (tiền vào hệ thống)

    [Display(Name = "Hoàn tiền")]
    RefundToUser = 2,        // hệ thống hoàn tiền cho khách

    [Display(Name = "Rút tiền")]
    Withdrawal = 3,          // rút tiền 

    [Display(Name = "Chi cho tour guide")]
    PayoutToGuide = 4,       // chi cho tour guide

    [Display(Name = "Chi cho làng nghề")]
    PayoutToCraftVillage = 5,// chi cho làng nghề

    [Display(Name = "Thu hoa hồng")]
    CommissionIncome = 6,    // thu hoa hồng
}