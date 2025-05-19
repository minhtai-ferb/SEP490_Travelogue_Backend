using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeLocationModels;
public class TypeLocationCreateModel
{
    [Required, StringLength(100)]
    public string Name { get; set; }
}
