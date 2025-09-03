using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;
public enum TicketType
{
    [Display(Name = "Tham quan")]
    Visit = 1,
    [Display(Name = "Tham quan kèm trải nghiệm")]
    Experience = 2,
}