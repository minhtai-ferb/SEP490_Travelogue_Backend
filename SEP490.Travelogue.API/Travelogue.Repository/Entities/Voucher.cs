using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class Voucher : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TourId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; } = false;

    // Navigation
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("TourId")]
    public Tour Tour { get; set; } = null!;
}
