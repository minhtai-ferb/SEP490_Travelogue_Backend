using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopResponseDto
{
    public Guid WorkshopId { get; set; }
    public WorkshopStatus Status { get; set; }
    public string StatusText
    {
        get
        {
            return Status switch
            {
                WorkshopStatus.Draft => "Draft",
                WorkshopStatus.Confirmed => "Confirmed",
                WorkshopStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
