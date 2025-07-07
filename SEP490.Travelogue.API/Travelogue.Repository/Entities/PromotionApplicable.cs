using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class PromotionApplicable : BaseEntity
{
    public Guid PromotionId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }

    [Required]
    [EnumDataType(typeof(ServiceOption))]
    public ServiceOption ServiceType { get; set; }

    // Navigation properties
    [ForeignKey("PromotionId")]
    public Promotion Promotion { get; set; } = null!;

    [ForeignKey("TourId")]
    public Tour? Tour { get; set; }

    [ForeignKey("GuideId")]
    public TourGuide? TourGuide { get; set; }
}