using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.NewsModels;
public class NewsDataDetailModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }

    public Guid? EventId { get; set; }
    public string? EventName { get; set; }

    public Guid? NewsCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsHighlighted { get; set; }

    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();

    public List<RelatedNewsModel> RelatedNews { get; set; } = new();
}

public class RelatedNewsModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }

    public Guid? EventId { get; set; }
    public string? EventName { get; set; }

    public Guid? NewsCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
