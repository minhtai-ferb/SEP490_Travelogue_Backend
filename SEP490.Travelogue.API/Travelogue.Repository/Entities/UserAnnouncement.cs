using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class UserAnnouncement : BaseEntity
{
    public Guid AnnouncementId { get; set; }
    public Guid UserId { get; set; }
    public bool IsReaded { get; set; } = false;
    public Announcement? Announcement { get; set; }
    public User? User { get; set; }
}
