using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TransactionEntry : BaseEntity
{
    public Guid? WalletId { get; set; }
    public Guid? UserId { get; set; }

    public string? Description { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }

    [Required]
    [EnumDataType(typeof(TransactionStatus))]
    public TransactionStatus Status { get; set; }

    [EnumDataType(typeof(TransactionType))]
    public TransactionType Type { get; set; }

    [EnumDataType(typeof(TransactionType))]
    public TransactionDirection TransactionDirection { get; set; }

    public string? Reason { get; set; }
    public string? Method { get; set; }

    // Navigation properties
    [ForeignKey("WalletId")]
    public Wallet Wallet { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    // payos
    public string? AccountNumber { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal? PaidAmount { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public string? CounterAccountBankId { get; set; }
    public string? CounterAccountName { get; set; }
    public string? CounterAccountNumber { get; set; }
    public string? Currency { get; set; }
    public string? PaymentLinkId { get; set; }
}
