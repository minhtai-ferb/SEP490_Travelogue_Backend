using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.TourGuideModels;

namespace Travelogue.Service.BusinessModels.TourModels;

public class CreateTourDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    [Required]
    public Guid TourTypeId { get; set; }
}

public class TourResponseDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int TotalDays { get; set; }
    public Guid TourTypeId { get; set; }
    public string TourTypeText { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public TourStatus Status { get; set; }
    public string StatusText
    {
        get
        {
            return Status switch
            {
                TourStatus.Draft => "Draft",
                TourStatus.Confirmed => "Confirmed",
                TourStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}

public class UpdateTourDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    [Required]
    public Guid TourTypeId { get; set; }
}

public class UpdateTourPlanLocationDto
{
    public Guid? TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    [Range(1, int.MaxValue)]
    public int DayOrder { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}
public class TourPlanLocationResponseDto
{
    public Guid TourPlanLocationId { get; set; }
    public Guid LocationId { get; set; }
    public int DayOrder { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}

public class CreateTourScheduleDto
{
    [Required]
    public DateTime DepartureDate { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxParticipant { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AdultPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ChildrenPrice { get; set; }
}

public class TourScheduleResponseDto
{
    public Guid ScheduleId { get; set; }
    public DateTime DepartureDate { get; set; }
    public int MaxParticipant { get; set; }
    public int CurrentBooked { get; set; }
    public int TotalDays { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
}

public class TourDetailsResponseDto
{
    public Guid TourId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int TotalDays { get; set; }
    public Guid TourTypeId { get; set; }
    public string TourTypeName { get; set; }
    public string? TotalDaysText { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsDiscount { get; set; }
    public TourStatus Status { get; set; }
    public string StatusText
    {
        get
        {
            return Status switch
            {
                TourStatus.Draft => "Draft",
                TourStatus.Confirmed => "Confirmed",
                TourStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
    // public List<TourPlanLocationResponseDto> Locations { get; set; }
    public List<TourScheduleResponseDto> Schedules { get; set; }
    public TourGuideDataModel? TourGuide { get; set; }
    public List<PromotionDto>? Promotions { get; set; } = new List<PromotionDto>();
    public List<TourDayDetail> Days { get; set; } = new List<TourDayDetail>();
}

public class ConfirmTourDto
{
    public string? Notes { get; set; }
}