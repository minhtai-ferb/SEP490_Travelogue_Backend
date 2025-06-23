using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class LocationCategory : BaseEntity
{
    public Guid LocationId { get; set; }
    public Location Location { get; set; }

    public Guid CategoryId { get; set; }
    public TypeLocation Category { get; set; }
}