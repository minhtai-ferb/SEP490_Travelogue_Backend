using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class FilterWorkshop
{
    public string? Name { get; set; }

    public WorkshopStatus? Status { get; set; }

    public Guid? CraftVillageId { get; set; }
}