using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class Tour : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public Guid TourTypeId { get; set; }

    public TourType TourType { get; set; } = default!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();
    public ICollection<TourPlan> TourPlans { get; set; } = new List<TourPlan>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
