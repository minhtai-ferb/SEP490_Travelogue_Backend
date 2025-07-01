using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TypeHistoricalLocationModels;

public class TypeHistoricalLocationCreateModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
