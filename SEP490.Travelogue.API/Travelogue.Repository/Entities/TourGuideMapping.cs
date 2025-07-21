using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideMapping : BaseEntity
{
    [Required]
    public Guid TourId { get; set; }

    [Required]
    public Guid GuideId { get; set; }

    // Navigation properties
    [ForeignKey("TourId")]
    public Tour Tour { get; set; } = null!;

    [ForeignKey("GuideId")]
    public TourGuide TourGuide { get; set; } = null!;
}
