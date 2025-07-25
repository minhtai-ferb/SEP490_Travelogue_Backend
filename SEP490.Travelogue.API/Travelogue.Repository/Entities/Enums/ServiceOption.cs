using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ServiceOption
{
    [Display(Name = "Tour")]
    Tour = 1,

    [Display(Name = "Hướng dẫn viên")]
    TourGuide = 2,

    [Display(Name = "Cả hai")]
    Both = 3
}