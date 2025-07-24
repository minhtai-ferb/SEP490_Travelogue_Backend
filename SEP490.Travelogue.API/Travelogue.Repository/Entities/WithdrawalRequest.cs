using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WithdrawalRequest : BaseEntity
{
    [Required]
    public Guid CraftVillageId { get; set; }

    [Required]
    public Guid WalletId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime RequestTime { get; set; }

    [Required]
    public string Status { get; set; } = null!;

    public Guid? ProcessedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation
    public CraftVillage CraftVillage { get; set; } = null!;
    public User User { get; set; } = null!;

    // Navigation properties
    [ForeignKey("WalletId")]
    public Wallet Wallet { get; set; }
}
