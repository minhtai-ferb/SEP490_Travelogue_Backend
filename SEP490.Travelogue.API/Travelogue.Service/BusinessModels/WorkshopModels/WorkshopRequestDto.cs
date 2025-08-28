using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopRequestDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public WorkshopStatus Status { get; set; } = WorkshopStatus.Pending;

    public List<TicketTypeRequestDto> TicketTypes { get; set; } = new();
    public List<RecurringRuleRequestDto> RecurringRules { get; set; } = new();
    public List<WorkshopExceptionRequestDto> Exceptions { get; set; } = new();
}

public class TicketTypeRequestDto
{
    [Required, MaxLength(50)]
    public TicketType Type { get; set; } = TicketType.Visit; // Visit / Experience

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsCombo { get; set; } = false;

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public string? Content { get; set; }

    // Nếu là Experience mới có Activities
    public List<WorkshopActivityRequestDto>? WorkshopActivities { get; set; }
}

public class RecurringRuleRequestDto
{
    [Required]
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public List<SessionRequestDto> Sessions { get; set; } = new();
}

public class SessionRequestDto
{
    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
}

public class WorkshopExceptionRequestDto
{
    [Required]
    public DateTime Date { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;
}

public class WorkshopActivityRequestDto
{
    [Required, MaxLength(200)]
    public string Activity { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Giờ bắt đầu tính theo số giờ từ lúc workshop khởi động.
    /// Ví dụ: 0 = ngay khi bắt đầu, 1 = sau 1 tiếng.
    /// </summary>
    [Range(0, 24)]
    public double StartHour { get; set; }

    /// <summary>
    /// Giờ kết thúc tính theo số giờ từ lúc workshop khởi động.
    /// </summary>
    [Range(0, 24)]
    public double EndHour { get; set; }

    /// <summary>
    /// Thứ tự hoạt động trong quy trình (step 1, step 2, ...)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int ActivityOrder { get; set; }
}