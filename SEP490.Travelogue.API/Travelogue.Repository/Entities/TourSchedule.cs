using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourSchedule : BaseEntity
{
    [Required]
    public Guid TourId { get; set; }

    [Required]
    public DateTime DepartureDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }  // Số người tối đa cho ngày này

    public int CurrentBooked { get; set; } = 0; // Số người đã đặt

    public Tour Tour { get; set; } = null!;
}