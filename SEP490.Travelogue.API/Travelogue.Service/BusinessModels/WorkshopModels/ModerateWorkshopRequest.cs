using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class ModerateWorkshopRequest
{
    public Guid WorkshopId { get; set; }
    public WorkshopStatus Status { get; set; }
    public string? Comment { get; set; }
}