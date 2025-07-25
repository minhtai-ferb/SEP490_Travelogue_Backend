using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ParticipantType
{
    [Display(Name = "Người lớn")]
    Adult = 1,

    [Display(Name = "Trẻ em")]
    Child = 2
}