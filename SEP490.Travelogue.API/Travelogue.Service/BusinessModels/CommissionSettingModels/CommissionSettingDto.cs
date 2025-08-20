namespace Travelogue.Service.BusinessModels.CommissionSettingModels;

public class CommissionSettingDto
{
    public Guid Id { get; set; }
    public decimal TourGuideCommissionRate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public class UpdateCommissionSettingRequest
{
    public decimal TourGuideCommissionRate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime EffectiveDate { get; set; }
}