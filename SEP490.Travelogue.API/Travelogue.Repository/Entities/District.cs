using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class District : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? FileKey { get; set; }
    public string? Description { get; set; }
    public float? Area { get; set; }
    public ICollection<Event>? Events { get; set; }
    public ICollection<Experience>? Experiences { get; set; }
    public ICollection<Location>? Locations { get; set; }
}
