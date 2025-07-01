using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourCreateModel
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    public int TotalDays { get; set; }

    [Required]
    public Guid TourTypeId { get; set; }
}
