using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.NewsCategoryModels;
public class NewsCategoryCreateModel
{
    [Required, StringLength(100)]
    public string Category { get; set; }
}
