using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class LocationHotelSuggestion : BaseEntity
{
    public Guid LocationId { get; set; }
    public Guid HotelId { get; set; }
    public string? Note { get; set; }
    public Location Location { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
}
