using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public class PasswordResetToken : BaseEntity
{
    public string? Email { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiryTime { get; set; }
    public bool IsUsed { get; set; }
}
