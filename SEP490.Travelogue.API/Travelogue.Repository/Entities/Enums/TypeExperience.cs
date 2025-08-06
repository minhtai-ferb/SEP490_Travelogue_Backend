using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TypeExperience
{
    [Display(Name = "Phiêu lưu")]
    Adventure = 1,

    [Display(Name = "Văn hóa")]
    Cultural = 2,

    [Display(Name = "Ẩm thực")]
    Culinary = 3,

    [Display(Name = "Du lịch sinh thái")]
    Ecotourism = 4,

    [Display(Name = "Giải trí")]
    Leisure = 5,

    [Display(Name = "Tâm linh")]
    Spiritual = 6,

    [Display(Name = "Mạo hiểm")]
    Extreme = 7
}