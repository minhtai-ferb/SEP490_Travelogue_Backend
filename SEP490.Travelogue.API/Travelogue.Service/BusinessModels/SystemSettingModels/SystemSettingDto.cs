using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.SystemSettingModels;

public class SystemSettingDto
{
    public SystemSettingKey Key { get; set; }
    public string KeyText { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Unit { get; set; }
}
