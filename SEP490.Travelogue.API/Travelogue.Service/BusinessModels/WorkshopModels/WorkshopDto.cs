using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public WorkshopStatus Status { get; set; } = WorkshopStatus.Pending;

    public List<TicketTypeRequestDto> TicketTypes { get; set; } = new();
    public List<WorkshopScheduleResponseDto> Schedules { get; set; } = new();
    public List<RecurringRuleRequestDto> RecurringRules { get; set; } = new();
    public List<WorkshopExceptionRequestDto> Exceptions { get; set; } = new();
}

public class WorkshopScheduleResponseDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public int CurrentBooked { get; set; }
    public ScheduleStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class TicketTypeRequestDto
{
    [Required]
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

    public TimeSpan StartHour { get; set; }

    public TimeSpan EndHour { get; set; }

    [Range(1, int.MaxValue)]
    public int ActivityOrder { get; set; }
}