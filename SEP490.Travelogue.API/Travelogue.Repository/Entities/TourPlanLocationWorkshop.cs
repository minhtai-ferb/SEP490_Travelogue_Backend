using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanLocationWorkshop : BaseEntity
{
    [Required]
    public Guid TourPlanLocationId { get; set; }
    public TourPlanLocation TourPlanLocation { get; set; } = null!;

    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public Guid? WorkshopTicketTypeId { get; set; }
    public WorkshopTicketType? WorkshopTicketType { get; set; }

    public Guid? WorkshopSessionRuleId { get; set; }
    public WorkshopSessionRule? WorkshopSessionRule { get; set; }

    public string? Notes { get; set; }
}
