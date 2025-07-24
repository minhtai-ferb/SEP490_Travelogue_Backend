using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TripPlanLocation : BaseEntity
{
    [Required]
    public Guid TripPlanId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }
    public int Order { get; set; } = 0;
    public float? TravelTimeFromPrev { get; set; } = 0;
    public float? DistanceFromPrev { get; set; } = 0;
    public float? EstimatedStartTime { get; set; } = 0;
    public float? EstimatedEndTime { get; set; } = 0;

    public TripPlan? TripPlan { get; set; }
    public Location? Location { get; set; }
}
