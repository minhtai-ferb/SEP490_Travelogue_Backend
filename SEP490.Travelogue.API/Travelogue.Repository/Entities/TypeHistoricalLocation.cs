using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TypeHistoricalLocation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<HistoricalLocation>? HistoricalLocations { get; set; }
}
