using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public class WorkshopTicketTypeRequest : BaseEntity
{
    public TicketType Type { get; set; } = TicketType.Visit;   // Visit / Experience
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsCombo { get; set; }
    public int DurationMinutes { get; set; }
    public string? Content { get; set; }

    // Chỉ Experience mới có
    public ICollection<WorkshopActivityRequest>? WorkshopActivities { get; set; }
}