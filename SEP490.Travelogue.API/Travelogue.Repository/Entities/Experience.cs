using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Experience : BaseEntity
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? TypeExperienceId { get; set; }
    public Guid? DistrictId { get; set; }

    // Navigation Properties
    public Location? Location { get; set; }
    public Event? Event { get; set; }
    public District? District { get; set; }
    public TypeExperience? TypeExperience { get; set; }
    public ICollection<ExperienceMedia>? ExperienceMedias { get; set; }
}
