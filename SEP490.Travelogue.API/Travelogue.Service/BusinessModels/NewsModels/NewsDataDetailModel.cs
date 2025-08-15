using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.NewsModels;

public class NewsDataDetailModel : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public NewsCategory? NewsCategory { get; set; }
    public string? CategoryName { get; set; }
    public bool IsHighlighted { get; set; }
    public TypeExperience? TypeExperience { get; set; }
    public string? TypeExperienceText { get; set; } = string.Empty;

    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();

    public List<RelatedNewsModel> RelatedNews { get; set; } = new();
}

public class RelatedNewsModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }

    public NewsCategory NewsCategory { get; set; }
    public string? CategoryName { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
