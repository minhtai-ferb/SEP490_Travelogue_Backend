using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

// public sealed class TripPlanCuisine : BaseEntity
// {
//     [Required]
//     public Guid TripPlanVersionId { get; set; }

//     [Required]
//     public Guid CuisineId { get; set; }
//     [DataType(DataType.DateTime)]
//     public DateTime? StartTime { get; set; }

//     [DataType(DataType.DateTime)]
//     public DateTime? EndTime { get; set; }

//     public string? Notes { get; set; }
//     public int Order { get; set; } = 0;

//     public TripPlanVersion? TripPlanVersion { get; set; }
//     public Cuisine? Cuisine { get; set; }
// }
