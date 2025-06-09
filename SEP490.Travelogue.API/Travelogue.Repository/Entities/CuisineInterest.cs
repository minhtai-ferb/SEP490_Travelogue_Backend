using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CuisineInterest : BaseEntity
{
    public Guid CuisineId { get; set; }
    public Guid InterestId { get; set; }

    public Interest? Interest { get; set; }
    public Cuisine? Cuisine { get; set; }
}

