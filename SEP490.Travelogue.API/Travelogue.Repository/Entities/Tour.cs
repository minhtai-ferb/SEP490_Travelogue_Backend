using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Tour : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    public int TotalDays { get; set; }

    [Required]
    public Guid TourTypeId { get; set; }

    public Guid? CurrentVersionId { get; set; }

    [ForeignKey(nameof(CurrentVersionId))]
    public TourPlanVersion? CurrentVersion { get; set; }


    public TourType TourType { get; set; } = default!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();
    public ICollection<TourSchedule> TourSchedules { get; set; } = new List<TourSchedule>();
    // public ICollection<TourPlanLocation> TourPlanLocations { get; set; } = new List<TourPlanLocation>();
    public ICollection<TourPlanVersion> TourPlanVersions { get; set; } = new List<TourPlanVersion>();


    public ICollection<TourGuideMapping> TourGuideMappings { get; set; } = new List<TourGuideMapping>();
    public ICollection<PromotionApplicable> PromotionApplicables { get; set; } = new List<PromotionApplicable>();
}
