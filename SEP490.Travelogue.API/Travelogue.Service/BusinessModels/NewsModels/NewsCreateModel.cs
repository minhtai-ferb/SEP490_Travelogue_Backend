using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.NewsModels;

public class NewsCreateModel
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? NewsCategoryId { get; set; }
    public bool IsHighlighted { get; set; }
}
