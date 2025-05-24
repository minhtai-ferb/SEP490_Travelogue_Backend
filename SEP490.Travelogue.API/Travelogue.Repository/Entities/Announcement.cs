using Travelogue.Repository.Bases.BaseEntitys;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;
public class Announcement
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? TourId { get; set; }
    public Tour? Tour { get; set; }

    public Guid? TourGuideId { get; set; }
    public TourGuide? TourGuide { get; set; }

    public ICollection<UserAnnouncement> UserAnnouncements { get; set; }
}


