namespace Travelogue.Service.BusinessModels.WithdrawalRequestModels;

public class WithdrawalRequestCreateDto
{
    public decimal Amount { get; set; }
    public Guid BankAccountId { get; set; }
}
