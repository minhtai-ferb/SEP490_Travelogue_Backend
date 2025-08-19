using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Announcement : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NotificationType Type { get; set; }

    public Guid? TourId { get; set; }
    public Tour? Tour { get; set; }

    public Guid? TourGuideId { get; set; }
    public TourGuide? TourGuide { get; set; }

    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }

    public Guid? ReviewId { get; set; }
    public Review? Review { get; set; }

    public Guid? ReportId { get; set; }
    public Report? Report { get; set; }

    public ICollection<UserAnnouncement> UserAnnouncements { get; set; } = new List<UserAnnouncement>();
}
