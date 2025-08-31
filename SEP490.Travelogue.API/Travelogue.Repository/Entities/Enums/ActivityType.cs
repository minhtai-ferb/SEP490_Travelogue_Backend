using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ActivityType
{
    [Display(Name = "Tham quan")]
    Sightseeing = 1,

    [Display(Name = "Ăn uống")]
    FoodAndDrink = 2,

    [Display(Name = "Trải nghiệm làng nghề")]
    Workshop = 3,

    [Display(Name = "Nghỉ ngơi")]
    Rest = 4,

    [Display(Name = "Mua sắm")]
    Shopping = 5,

    [Display(Name = "Giải trí")]
    Entertainment = 6,

    [Display(Name = "Trải nghiệm")]
    Experience = 7
}
