using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeEventModels;
public class TypeEventCreateModel
{
    [Required, StringLength(100)]
    public string TypeName { get; set; } = string.Empty;
}
