using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class LocationInterest : BaseEntity
{
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public Interest Interest { get; set; }
}
