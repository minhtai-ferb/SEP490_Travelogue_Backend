namespace Travelogue.Service.BusinessModels.CommissionSettingModels;

public class CommissionSettingDto
{
    public Guid Id { get; set; }

    public decimal TourGuideCommissionRate { get; set; }
    public DateTime TourGuideEffectiveDate { get; set; }
    public DateTime? TourGuideEndDate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime CraftVillageEffectiveDate { get; set; }
    public DateTime? CraftVillageEndDate { get; set; }
}

public class CreateCommissionSettingRequest
{
    public decimal TourGuideCommissionRate { get; set; }
    public DateTime TourGuideEffectiveDate { get; set; }
    public DateTime? TourGuideEndDate { get; set; }

    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime CraftVillageEffectiveDate { get; set; }
    public DateTime? CraftVillageEndDate { get; set; }
}

public class TourGuideCommissionDto
{
    public Guid Id { get; set; }
    public decimal TourGuideCommissionRate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CraftVillageCommissionDto
{
    public Guid Id { get; set; }
    public decimal CraftVillageCommissionRate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
