using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourJoinRequest : BaseEntity
{

    [Required]
    public Guid FromOrderId { get; set; }

    [Required]
    public Guid ToOrderId { get; set; }

    public string? RequestNote { get; set; }
    public string? ResponseNote { get; set; }

    [Required]
    public string Status { get; set; } = null!;

    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    [ForeignKey("FromOrderId")]
    public Order FromOrder { get; set; } = null!;

    [ForeignKey("ToOrderId")]
    public Order ToOrder { get; set; } = null!;
}
