using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopRecurringRule : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    public ICollection<WorkshopSessionRule> Sessions { get; set; } = new List<WorkshopSessionRule>();
}