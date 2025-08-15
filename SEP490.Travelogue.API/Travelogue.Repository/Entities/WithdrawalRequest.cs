using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class WithdrawalRequest : BaseEntity
{
    [Required]
    public Guid WalletId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public Guid BankAccountId { get; set; }
    public string? Note { get; set; }
    public string? ProofImageUrl { get; set; }

    public DateTime RequestTime { get; set; }

    [Required]
    public WithdrawalRequestStatus Status { get; set; } = WithdrawalRequestStatus.Pending;

    // Navigation
    public User User { get; set; } = null!;

    // Navigation properties
    [ForeignKey("WalletId")]
    public Wallet Wallet { get; set; }

    [ForeignKey("BankAccountId")]
    public BankAccount BankAccount { get; set; }
}
