using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideMapping : BaseEntity
{
    [Required]
    public Guid TourScheduleId { get; set; }

    [Required]
    public Guid TourGuideId { get; set; }

    // Navigation properties
    [ForeignKey("TourScheduleId")]
    public TourSchedule TourSchedule { get; set; } = null!;

    [ForeignKey("TourGuideId")]
    public TourGuide TourGuide { get; set; } = null!;
}
