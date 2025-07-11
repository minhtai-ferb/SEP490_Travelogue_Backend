using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ServiceOption
{
    [Display(Name = "Tour")]
    Tour,

    [Display(Name = "Hướng dẫn viên")]
    TourGuide,

    [Display(Name = "Cả hai")]
    Both
}