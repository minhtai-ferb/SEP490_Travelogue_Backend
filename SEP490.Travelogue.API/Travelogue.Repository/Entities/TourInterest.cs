using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TourInterest : BaseEntity
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Interest Interest { get; set; }
}