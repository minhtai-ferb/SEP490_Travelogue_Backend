using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntitys;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Order : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    public DateTime? ScheduledDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    public DateTime? CancelledAt { get; set; }
    public bool IsOpenToJoin { get; set; } = false;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPaid { get; set; }

    public User? User { get; set; }
    public Tour? Tour { get; set; }
    public TourGuide? TourGuide { get; set; }
}
