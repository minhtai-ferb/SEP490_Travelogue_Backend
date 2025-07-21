using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Announcement : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public Guid? TourId { get; set; }
    public Tour? Tour { get; set; }

    public Guid? TourGuideId { get; set; }
    public TourGuide? TourGuide { get; set; }

    public ICollection<UserAnnouncement> UserAnnouncements { get; set; } = new List<UserAnnouncement>();
}
