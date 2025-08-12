using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum SystemSettingKey
{
    [Display(Name = "Tỷ lệ hoa hồng")]
    BookingCommissionPercent = 1,
}