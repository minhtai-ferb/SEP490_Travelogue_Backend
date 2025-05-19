using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeEventModels;
public class TypeEventUpdateModel
{
    [Required, StringLength(100)]
    public string TypeName { get; set; } = string.Empty;
}
