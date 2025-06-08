using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TourGuide : BaseEntity
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public string? Introduction { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }
    public ICollection<TourGuideAvailability>? Availabilities { get; set; }
}
