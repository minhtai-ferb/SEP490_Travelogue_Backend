using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TypeExperience : BaseEntity
{
    public string TypeName { get; set; } = string.Empty;
    public ICollection<Experience>? Experiences { get; set; }
}
