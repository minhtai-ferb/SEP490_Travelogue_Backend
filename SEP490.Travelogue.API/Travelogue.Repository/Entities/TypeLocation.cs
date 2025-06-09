using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TypeLocation : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Location>? Locations { get; set; }
}
