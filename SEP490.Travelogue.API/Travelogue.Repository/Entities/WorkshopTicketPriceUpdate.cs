using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopTicketPriceUpdate : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    [Required]
    public Guid TicketTypeId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal OldPrice { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal NewPrice { get; set; }

    [Required]
    public PriceUpdateStatus Status { get; set; } = PriceUpdateStatus.Pending;

    public string? RequestReason { get; set; }
    public string? ModeratorNote { get; set; }

    [Required]
    public Guid RequestedBy { get; set; }
    public Guid? DecidedBy { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
}
