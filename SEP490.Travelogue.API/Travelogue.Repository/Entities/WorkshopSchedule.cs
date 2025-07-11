using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class WorkshopSchedule : BaseEntity
{
    [Required]
    public Guid WorkshopId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }  // Số người tối đa cho ngày này

    public int CurrentBooked { get; set; } = 0; // Số người đã đặt
    public string? Notes { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AdultPrice { get; set; }
    [Range(0, double.MaxValue)]
    public decimal ChildrenPrice { get; set; }

    public Workshop? Workshop { get; set; }
}