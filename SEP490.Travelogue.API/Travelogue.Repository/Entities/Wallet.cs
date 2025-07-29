using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Wallet : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Balance { get; set; } = 0.00m;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; }
    public ICollection<TransactionEntry> Transactions { get; set; } = new List<TransactionEntry>();
    public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
}