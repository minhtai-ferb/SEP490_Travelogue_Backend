using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ParticipantType
{
    [Display(Name = "Người lớn")]
    Adult,

    [Display(Name = "Trẻ em")]
    Child
}