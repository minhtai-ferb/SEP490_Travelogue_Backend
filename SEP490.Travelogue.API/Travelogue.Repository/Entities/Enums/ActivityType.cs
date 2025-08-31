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
}