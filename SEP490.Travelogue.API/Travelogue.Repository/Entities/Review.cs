using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public class Review : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    public Guid BookingId { get; set; }

    public Guid? TourId { get; set; }
    public Guid? WorkshopId { get; set; }
    public Guid? TourGuideId { get; set; }

    public string? Comment { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public User User { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
    public Tour? Tour { get; set; }
    public Workshop? Workshop { get; set; }
    public TourGuide? TourGuide { get; set; }

    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
