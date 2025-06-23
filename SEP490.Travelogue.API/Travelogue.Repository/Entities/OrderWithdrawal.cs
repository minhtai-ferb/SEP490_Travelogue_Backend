using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class OrderWithdrawal : BaseEntity
{

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid WithdrawalRequestId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [ForeignKey("WithdrawalRequestId")]
    public WithdrawalRequest WithdrawalRequest { get; set; } = null!;

    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;
}
