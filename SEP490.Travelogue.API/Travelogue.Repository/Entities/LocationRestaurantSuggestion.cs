using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class LocationRestaurantSuggestion : BaseEntity
{
    public Guid LocationId { get; set; }
    public Guid RestaurantId { get; set; }
    public string? Note { get; set; }
    public Location Location { get; set; } = null!;
    public Restaurant Restaurant { get; set; } = null!;
}
