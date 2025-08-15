using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum BookingType
{
    [Display(Name = "Tour")]
    Tour = 1,
    [Display(Name = "Workshop")]
    Workshop = 2,
    [Display(Name = "Tour Guide")]
    TourGuide = 3,
}
