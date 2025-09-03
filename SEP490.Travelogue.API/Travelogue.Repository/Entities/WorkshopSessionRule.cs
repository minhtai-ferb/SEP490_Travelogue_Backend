using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopSessionRule : BaseEntity
{
    [Required]
    public Guid RecurringRuleId { get; set; }
    public WorkshopRecurringRule RecurringRule { get; set; } = null!;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Capacity { get; set; }
}