using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Transaction : BaseEntity
{
    public Guid? BookingId { get; set; }

    [Required]
    public Guid WalletId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }

    [Required]
    [EnumDataType(typeof(TransactionStatus))]
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }

    // Navigation properties
    [ForeignKey("WalletId")]
    public Wallet Wallet { get; set; }

    [ForeignKey("BookingId")]
    public Booking Booking { get; set; }
}
