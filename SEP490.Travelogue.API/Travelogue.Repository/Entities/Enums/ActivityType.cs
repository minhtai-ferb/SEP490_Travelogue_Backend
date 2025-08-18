using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ActivityType
{
    [Display(Name = "Tham quan")]
    Sightseeing = 1,
    [Display(Name = "Ăn uống")]
    FoodAndDrink = 2,
}