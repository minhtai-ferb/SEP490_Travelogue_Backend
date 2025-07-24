using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum DiscountType
{
    [Display(Name = "Cố định")]
    Fixed = 1,

    [Display(Name = "Phần trăm")]
    Percentage = 2
}
