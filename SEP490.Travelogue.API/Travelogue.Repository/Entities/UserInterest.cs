using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class UserInterest : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid InterestId { get; set; }

    public User? User { get; set; }
    public Interest? Interest { get; set; }
}
