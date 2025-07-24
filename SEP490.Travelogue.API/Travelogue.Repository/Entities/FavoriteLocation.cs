using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class FavoriteLocation : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("LocationId")]
    public Location? Location { get; set; }
}
