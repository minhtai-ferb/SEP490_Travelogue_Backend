using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourPlan : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    [Required]
    public Guid TourId { get; set; }

    public User? User { get; set; }
    public Tour? Tour { get; set; }
    public ICollection<TourPlanLocation>? TourPlanLocations { get; set; }
}
