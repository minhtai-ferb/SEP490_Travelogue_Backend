using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopResponseDtoOLD
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public WorkshopStatus Status { get; set; }

    public Guid CraftVillageId { get; set; }
    public string? CraftVillageName { get; set; }
    public string? StatusText { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
