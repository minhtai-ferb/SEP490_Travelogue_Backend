using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum PaymentChannel
{
    Unknown = 0,
    [Display(Name = "Ngân hàng")]
    Bank = 1,
    [Display(Name = "Ví nội bộ")]
    Wallet = 2,     // ví 
}
