using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.ExperienceModels;
public class ExperienceMediaResponse
{
    public Guid ExperienceId { get; set; }
    public string Title { get; set; }
    public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
}
