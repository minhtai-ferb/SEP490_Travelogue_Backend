using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideAvailability : BaseEntity
{
    [ForeignKey("TourGuide")]
    public Guid TourGuideId { get; set; }

    [MaxLength(255)]
    public string? Note { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation property
    public TourGuide TourGuide { get; set; } = null!;
}