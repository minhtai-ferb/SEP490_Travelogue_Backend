using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TourTypeModels;

public class TourTypeUpdateModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
