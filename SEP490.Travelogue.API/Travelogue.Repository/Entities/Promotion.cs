using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Promotion : BaseEntity
{
    [Required]
    [StringLength(100)]
    public required string PromotionName { get; set; }

    [Required]
    [StringLength(50)]
    public required string PromotionCode { get; set; }

    [Required]
    [EnumDataType(typeof(DiscountType))]
    public DiscountType DiscountType { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountValue { get; set; }

    [Required]
    public required DateTime StartDate { get; set; }

    [Required]
    public required DateTime EndDate { get; set; }

    [Required]
    [EnumDataType(typeof(ServiceOption))]
    public required ServiceOption ApplicableType { get; set; }

    // Navigation
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("TourId")]
    public Tour Tour { get; set; } = null!;

    // Navigation properties
    public ICollection<PromotionApplicable> PromotionApplicables { get; set; } = new List<PromotionApplicable>();
    // public ICollection<Booking> Bookings { get; set; }
}
