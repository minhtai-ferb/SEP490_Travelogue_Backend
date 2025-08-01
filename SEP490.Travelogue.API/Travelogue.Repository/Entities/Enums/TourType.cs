using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TourType
{
    [Display(Name = "Du lịch trong nước")]
    International = 1,

    [Display(Name = "Du lịch nghỉ dưỡng")]
    Leisure = 2,

    [Display(Name = "Du lịch khám phá")]
    Adventure = 3,

    [Display(Name = "Du lịch sinh thái")]
    Ecotourism = 4,

    [Display(Name = "Du lịch văn hóa")]
    Cultural = 5,

    [Display(Name = "Du lịch tâm linh")]
    Spiritual = 6,

    [Display(Name = "Du lịch ẩm thực")]
    Culinary = 7,

    [Display(Name = "Du lịch mạo hiểm")]
    Extreme = 8
}
