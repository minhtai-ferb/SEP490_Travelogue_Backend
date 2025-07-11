using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopActivity : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }

    [Required, MaxLength(200)]
    public string Activity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
    public int DayOrder { get; set; } = 0;

    public Workshop? Workshop { get; set; }
}
