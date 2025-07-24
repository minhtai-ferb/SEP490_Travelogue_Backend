using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Transaction : BaseEntity
{
    [Required]
    public Guid BookingId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime TransactionDate { get; set; }

    [Required]
    [EnumDataType(typeof(TransactionStatus))]
    public TransactionStatus Status { get; set; }

    public Booking? Booking { get; set; }
}
