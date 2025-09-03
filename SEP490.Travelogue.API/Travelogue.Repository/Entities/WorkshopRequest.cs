using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopRequest : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid CraftVillageId { get; set; }
    public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;

    // Một workshop có nhiều loại vé
    public ICollection<WorkshopTicketTypeRequest> TicketTypes { get; set; } = new List<WorkshopTicketTypeRequest>();

    public ICollection<WorkshopRecurringRuleRequest> RecurringRules { get; set; } = new List<WorkshopRecurringRuleRequest>();
    public ICollection<WorkshopExceptionRequest> Exceptions { get; set; } = new List<WorkshopExceptionRequest>();
    public List<MediaRequest> Medias { get; set; } = new();
}