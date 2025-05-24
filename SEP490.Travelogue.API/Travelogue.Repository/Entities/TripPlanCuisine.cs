using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TripPlanCuisine : BaseEntity
{
    [Required]
    public Guid TripPlanId { get; set; }

    [Required]
    public Guid CuisineId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    public TripPlan? TripPlan { get; set; }
    public Cuisine? Cuisine { get; set; }
}
