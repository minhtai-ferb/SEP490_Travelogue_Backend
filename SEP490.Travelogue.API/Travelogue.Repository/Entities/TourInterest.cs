using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourInterest : BaseEntity
{
    public Guid TourId { get; set; }
    public Guid InterestId { get; set; }

    public Interest? Interest { get; set; }
    public Tour? Tour { get; set; }
}
