using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class News : BaseEntity
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    // Navigation Properties
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    public NewsCategory? NewsCategory { get; set; }

    public bool IsHighlighted { get; set; } = false;
}
