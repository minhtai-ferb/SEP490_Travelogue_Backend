using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class LocationCraftVillageSuggestion : BaseEntity
{
    public Guid LocationId { get; set; }
    public Guid CraftVillageId { get; set; }
    public string? Note { get; set; }
    public Location Location { get; set; } = null!;
    public CraftVillage CraftVillage { get; set; } = null!;
}
