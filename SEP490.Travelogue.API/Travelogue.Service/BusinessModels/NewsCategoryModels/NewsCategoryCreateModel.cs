using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.NewsCategoryModels;

public class NewsCategoryCreateModel
{
    [Required, StringLength(100)]
    public required string Category { get; set; }
}
