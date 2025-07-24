using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum LanguageEnum
{
    [Display(Name = "Tiếng Việt")]
    Vietnamese = 1,

    [Display(Name = "Tiếng Anh")]
    English = 2,

    [Display(Name = "Tiếng Nhật")]
    Japanese = 3,

    [Display(Name = "Tiếng Trung")]
    Chinese = 4,

    [Display(Name = "Tiếng Hàn")]
    Korean = 5
}
