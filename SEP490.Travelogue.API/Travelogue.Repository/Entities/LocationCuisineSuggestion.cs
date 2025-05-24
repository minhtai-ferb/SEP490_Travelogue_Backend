using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class LocationCuisineSuggestion : BaseEntity
{
    public Guid LocationId { get; set; }
    public Guid CuisineId { get; set; }
    public string? Note { get; set; }
    public Location Location { get; set; } = null!;
    public Cuisine Cuisine { get; set; } = null!;
}
