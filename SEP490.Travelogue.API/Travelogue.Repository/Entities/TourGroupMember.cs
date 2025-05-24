using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TourGroupMember : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid OrderId { get; set; }
    [Required]
    public Guid TourGroupId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime JoinDate { get; set; }

    public User User { get; set; } = default!;
    public Order Orders { get; set; } = default!;
    public TourGroup TourGroups { get; set; } = default!;
}
