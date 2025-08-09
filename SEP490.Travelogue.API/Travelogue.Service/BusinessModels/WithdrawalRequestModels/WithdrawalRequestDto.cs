using Travelogue.Service.BusinessModels.BankAccountModels;

namespace Travelogue.Service.BusinessModels.WithdrawalRequestModels;

public class WithdrawalRequestDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public Guid BankAccountId { get; set; }
    public BankAccountDto? BankAccount { get; set; }
    public DateTime RequestTime { get; set; }
}
