using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.ExperienceModels;
public class ExperienceDataModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid? EventId { get; set; }
    public string? EventName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public Guid? TypeExperienceId { get; set; }
    public string? TypeExperienceName { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
