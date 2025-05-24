using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public class EntitySample
{

}
//public sealed class Order : BaseEntity
//{
//    [Required]
//    public Guid UserId { get; set; }

//    [Required]
//    public Guid OrderTypeId { get; set; }

//    [DataType(DataType.DateTime)]
//    public DateTime OrderDate { get; set; }

//    [Range(0, double.MaxValue)]
//    public decimal TotalAmount { get; set; }

//    [Required, MaxLength(50)]
//    public string Status { get; set; } = string.Empty;

//    [Required, MaxLength(50)]
//    public string PaymentMethod { get; set; } = string.Empty;

//    public User? User { get; set; }
//    public OrderType? OrderType { get; set; }
//    public ICollection<OrderDetail>? OrderDetails { get; set; }
//    public ICollection<Transaction>? Transactions { get; set; }
//}
//public sealed class OrderType : BaseEntity
//{
//    [Required, MaxLength(100)]
//    public string Name { get; set; } = string.Empty;

//    [MaxLength(500)]
//    public string? Description { get; set; }

//    public ICollection<Order>? Orders { get; set; }
//}
