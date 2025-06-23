using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class HistoricalLocation : BaseEntity
{
    public HeritageRank HeritageRank { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; } = null!;
}
