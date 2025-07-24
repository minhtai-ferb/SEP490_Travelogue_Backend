using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourScheduleGuide : BaseEntity
{
    [Required]
    public Guid TourScheduleId { get; set; }

    [Required]
    public Guid TourGuideId { get; set; }

    public TourSchedule TourSchedule { get; set; } = null!;
    public TourGuide TourGuide { get; set; } = null!;
}