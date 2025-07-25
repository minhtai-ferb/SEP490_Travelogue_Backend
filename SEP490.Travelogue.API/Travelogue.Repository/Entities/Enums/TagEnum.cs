using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TagEnum
{
    [Display(Name = "Thân thiện")]
    Friendly = 1,

    [Display(Name = "Hài hước")]
    Funny = 2,

    [Display(Name = "Hiểu biết")]
    Knowledgeable = 3,

    [Display(Name = "Hoạt ngôn")]
    Talkative = 4,

    [Display(Name = "Kinh nghiệm")]
    Experienced = 5
}