using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TripPlan : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public Guid? TripPlanVersionId { get; set; }

    public User? User { get; set; }
    public ICollection<TripPlanVersion> TripPlanVersions { get; set; } = new List<TripPlanVersion>();
    public ICollection<TripPlanShare> Shares { get; set; } = new List<TripPlanShare>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
