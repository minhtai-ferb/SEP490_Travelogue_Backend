using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class RefundRequest : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Reason { get; set; }

    [Required]
    public string Status { get; set; } = null!;

    public DateTime? ApprovedAt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RefundAmount { get; set; }
    public Order Order { get; set; } = null!;
    public User User { get; set; } = null!;
}
