using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanLocation : BaseEntity
{
    [Required]
    public Guid TourPlanId { get; set; }

    [Required]
    public Guid LocationId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    public TourPlan? TourPlan { get; set; }
    public Location? Location { get; set; }
}
