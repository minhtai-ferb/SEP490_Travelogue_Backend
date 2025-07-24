using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.NewsModels;

public class NewsDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }

    public NewsCategory NewsCategory { get; set; }
    public string? CategoryName { get; set; }
    public bool IsHighlighted { get; set; }

    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
