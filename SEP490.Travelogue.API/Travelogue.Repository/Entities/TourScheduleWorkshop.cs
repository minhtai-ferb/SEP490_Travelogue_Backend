using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourScheduleWorkshop : BaseEntity
{
    [Required]
    public Guid TourScheduleId { get; set; }
    public TourSchedule TourSchedule { get; set; } = null!;

    [Required]
    public Guid TourPlanLocationId { get; set; }
    public TourPlanLocation TourPlanLocation { get; set; } = null!;

    [Required] public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    [Required] public Guid WorkshopScheduleId { get; set; }
    public WorkshopSchedule WorkshopSchedule { get; set; } = null!;

    public Guid? WorkshopTicketTypeId { get; set; }
    public WorkshopTicketType? WorkshopTicketType { get; set; }
}
