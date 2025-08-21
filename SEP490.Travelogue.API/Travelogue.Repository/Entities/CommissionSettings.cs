using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CommissionSettings : BaseEntity
{
    public decimal TourGuideCommissionRate { get; set; }
    public DateTime TourGuideEffectiveDate { get; set; }
    public DateTime? TourGuideEndDate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime CraftVillageEffectiveDate { get; set; }
    public DateTime? CraftVillageEndDate { get; set; }
}