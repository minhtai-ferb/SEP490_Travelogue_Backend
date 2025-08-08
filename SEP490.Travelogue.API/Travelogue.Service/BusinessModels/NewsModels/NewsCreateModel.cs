using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.NewsModels;

public class NewsCreateModel
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public NewsCategory NewsCategory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsHighlighted { get; set; }
    public TypeExperience? TypeExperience { get; set; }
    public List<MediaDto>? MediaDtos { get; set; } = new List<MediaDto>();
}
