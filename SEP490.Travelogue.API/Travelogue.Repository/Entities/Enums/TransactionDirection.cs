using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TransactionDirection
{
    [Display(Name = "Cộng tiền")]
    Credit,
    [Display(Name = "Trừ tiền")]
    Debit
}