using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.SystemSettingModels;

public class SystemSettingUpdateDto
{
    [Required]
    public SystemSettingKey Key { get; set; }
    [MaxLength(1000)]
    public string? Value { get; set; }
    // [MaxLength(100)]
    // public string? Unit { get; set; }
}
