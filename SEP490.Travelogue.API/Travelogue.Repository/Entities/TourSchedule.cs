using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TourSchedule : BaseEntity
{
    [Required]
    public required Guid TourId { get; set; }

    [Required]
    public DateTime DepartureDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }

    public int CurrentBooked { get; set; } = 0;

    public TourScheduleStatus Status { get; set; } = TourScheduleStatus.Active;
    public string? Reason { get; set; }

    // [Required]
    // [Range(1, int.MaxValue)]
    // public int TotalDays { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AdultPrice { get; set; }
    [Range(0, double.MaxValue)]
    public decimal ChildrenPrice { get; set; }

    public Tour Tour { get; set; } = null!;

    public ICollection<TourGuideSchedule> TourGuideSchedules { get; set; } = new List<TourGuideSchedule>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    // public ICollection<TourScheduleGuide> TourScheduleGuides { get; set; } = new List<TourScheduleGuide>();
}
