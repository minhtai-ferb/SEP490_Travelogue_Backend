using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class UserAnnouncement : BaseEntity
{
    public Guid AnnouncementId { get; set; }
    public Guid UserId { get; set; }
    public bool IsReaded { get; set; } = false;
    public Announcement? Notification { get; set; }
    public User? User { get; set; }
}
