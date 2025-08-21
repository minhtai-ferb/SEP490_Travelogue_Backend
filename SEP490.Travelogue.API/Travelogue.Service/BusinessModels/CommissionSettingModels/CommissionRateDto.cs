using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.CommissionSettingModels;

public class CommissionRateDto
{
    public Guid Id { get; set; }
    public CommissionType Type { get; set; }
    public string? CommissionTypeText { get; set; }
    public decimal RateValue { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class CommissionRateGroupDto
{
    public CommissionType Type { get; set; }
    public string? CommissionTypeText { get; set; } = string.Empty;
    public List<CommissionRateDto> Rates { get; set; } = new();
}

public sealed class CommissionRateCreateDto
{
    public CommissionType Type { get; set; }
    public decimal RateValue { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public sealed class CommissionRateUpdateDto
{
    public decimal RateValue { get; set; }
}
