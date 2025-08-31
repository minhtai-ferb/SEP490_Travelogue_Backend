using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum NewsCategory
{
    [Display(Name = "Tin tức")]
    News = 1,

    [Display(Name = "Sự kiện")]
    Event = 2,

    [Display(Name = "Trải nghiệm")]
    Experience = 3,
}
