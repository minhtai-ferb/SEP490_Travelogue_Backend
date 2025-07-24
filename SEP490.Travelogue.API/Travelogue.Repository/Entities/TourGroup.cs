using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGroup : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Required]
    public Guid TourId { get; set; }

    public Tour? Tour { get; set; }
    public ICollection<TourGroupMember>? TourGroupMembers { get; set; }
}
