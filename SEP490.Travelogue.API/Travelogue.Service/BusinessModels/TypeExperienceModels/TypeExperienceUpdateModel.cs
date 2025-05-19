using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeExperienceModels;
public class TypeExperienceUpdateModel
{
    [Required, StringLength(100)]
    public string TypeName { get; set; } = string.Empty;
}
