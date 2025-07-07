using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourSchedule : BaseEntity
{
    [Required]
    public Guid TourPlanVersionId { get; set; }

    [Required]
    public DateTime DepartureDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }  // Số người tối đa cho ngày này

    public int CurrentBooked { get; set; } = 0; // Số người đã đặt

    [Required]
    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    public TourPlanVersion TourPlanVersion { get; set; } = null!;
    public ICollection<TourScheduleGuide> TourScheduleGuides { get; set; } = new List<TourScheduleGuide>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
