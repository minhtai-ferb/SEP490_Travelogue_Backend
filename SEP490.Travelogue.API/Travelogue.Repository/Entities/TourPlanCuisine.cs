using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlanCuisine : BaseEntity
{
    [Required]
    public Guid TourPlanId { get; set; }

    [Required]
    public Guid CuisineId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    public TourPlan? TourPlan { get; set; }
    public Cuisine? Cuisine { get; set; }
}
