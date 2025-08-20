using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class SystemSetting : BaseEntity
{
    [Required]
    public SystemSettingKey Key { get; set; }

    [MaxLength(1000)]
    public string? Value { get; set; }

    [MaxLength(100)]
    public string? Unit { get; set; }
}
