using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class LocationInterest : BaseEntity
{
    public Guid LocationId { get; set; }
    public Guid InterestId { get; set; }

    public Interest? Interest { get; set; }
    public Location? Location { get; set; }
}

