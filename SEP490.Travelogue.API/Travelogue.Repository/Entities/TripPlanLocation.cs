using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TripPlanLocation : BaseEntity
{
    [Required]
    public Guid TripPlanVersionId { get; set; }

    [Required]
    public Guid LocationId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime? StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }
    public int Order { get; set; } = 0;

    public TripPlanVersion? TripPlanVersion { get; set; }
    public Location? Location { get; set; }
}
