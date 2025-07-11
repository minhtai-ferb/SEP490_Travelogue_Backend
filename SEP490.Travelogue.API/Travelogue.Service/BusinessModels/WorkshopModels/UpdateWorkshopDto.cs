namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class UpdateWorkshopDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid CraftVillageId { get; set; }
}
