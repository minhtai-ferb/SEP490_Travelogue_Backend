using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public Role? Role { get; set; }
}
