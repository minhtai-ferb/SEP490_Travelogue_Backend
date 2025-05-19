using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.NewsModels;
public class NewsMediaResponse
{
    public Guid NewsId { get; set; }
    public string Title { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
