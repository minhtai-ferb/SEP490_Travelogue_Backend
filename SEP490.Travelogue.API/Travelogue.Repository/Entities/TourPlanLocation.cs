using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanLocation : BaseEntity
{
    [Required]
    public Guid TourPlanVersionId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public int DayOrder { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }

    public TourPlanVersion TourPlanVersion { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
