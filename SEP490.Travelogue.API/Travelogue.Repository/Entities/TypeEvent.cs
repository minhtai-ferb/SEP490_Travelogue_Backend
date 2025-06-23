using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TypeEvent : BaseEntity
{
    public string TypeName { get; set; } = string.Empty;
    public ICollection<Event>? Events { get; set; }
}
