using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanLocation : BaseEntity
{
    [Required]
    public Guid TourId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public int DayOrder { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    public float TravelTimeFromPrev { get; set; }
    public float DistanceFromPrev { get; set; }
    public float EstimatedStartTime { get; set; }
    public float EstimatedEndTime { get; set; }

    public Tour Tour { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
