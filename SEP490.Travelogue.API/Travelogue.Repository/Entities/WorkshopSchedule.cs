using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

//public sealed class WorkshopSchedule : BaseEntity
//{
//    [Required]
//    public Guid WorkshopId { get; set; }

//    [DataType(DataType.DateTime)]
//    public DateTime StartTime { get; set; }

//    [DataType(DataType.DateTime)]
//    public DateTime EndTime { get; set; }

//    [Range(1, int.MaxValue)]
//    public int MaxParticipant { get; set; }

//    public int CurrentBooked { get; set; } = 0;
//    public string? Notes { get; set; }

//    [Range(0, double.MaxValue)]
//    public decimal AdultPrice { get; set; }
//    [Range(0, double.MaxValue)]
//    public decimal ChildrenPrice { get; set; }

//    public Workshop? Workshop { get; set; }
//    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
//}


public sealed class WorkshopSchedule : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public int CurrentBooked { get; set; } = 0;
    public string? Notes { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Active; 
}
