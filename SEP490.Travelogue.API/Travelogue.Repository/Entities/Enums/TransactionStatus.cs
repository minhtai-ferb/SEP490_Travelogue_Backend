using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3
}
public enum TransactionDirection
{
    [Display(Name = "Cộng tiền")]
    Credit,
    [Display(Name = "Trừ tiền")]
    Debit
}