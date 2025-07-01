using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.TourModels;

public class TourUpdateModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public int TotalDays { get; set; }

    public List<TourPlanLocationModel>? Locations { get; set; } = new List<TourPlanLocationModel>();
    public List<TourScheduleModel>? TourSchedules { get; set; } = new List<TourScheduleModel>();
}

public class TourScheduleModel
{
    public DateTime DepartureDate { get; set; }
    public int MaxParticipants { get; set; }
}

public class TourPlanLocationModel
{
    public Guid? Id { get; set; }
    public Guid LocationId { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Notes { get; set; }
}
