using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.NewsCategoryModels;
public class NewsCategoryUpdateModel
{
    [Required, StringLength(100)]
    public string Category { get; set; }
}
