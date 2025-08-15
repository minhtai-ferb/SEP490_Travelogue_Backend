using System.Transactions;
using Travelogue.Repository.Entities.Enums;
using TransactionStatus = Travelogue.Repository.Entities.Enums.TransactionStatus;

namespace Travelogue.Service.BusinessModels.TransactionModels;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid? WalletId { get; set; }
    public Guid? UserId { get; set; }

    public string? AccountNumber { get; set; }

    public decimal? PaidAmount { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public string? CounterAccountBankId { get; set; }
    public string? CounterAccountName { get; set; }
    public string? CounterAccountNumber { get; set; }
    public string? Currency { get; set; }
    public string? PaymentLinkId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? PaymentStatusText { get; set; }
    public TransactionStatus Status { get; set; }
    public string? StatusText { get; set; }
    public TransactionType Type { get; set; }
    public string? TypeText { get; set; }
}