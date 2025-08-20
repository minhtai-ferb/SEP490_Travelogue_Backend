using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CommissionSettings : BaseEntity
{
    public decimal TourGuideCommissionRate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime EffectiveDate { get; set; }
}