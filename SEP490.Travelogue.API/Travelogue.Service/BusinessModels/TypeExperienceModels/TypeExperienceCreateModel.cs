using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeExperienceModels;
public class TypeExperienceCreateModel
{
    [Required, StringLength(100)]
    public string TypeName { get; set; } = string.Empty;
}
