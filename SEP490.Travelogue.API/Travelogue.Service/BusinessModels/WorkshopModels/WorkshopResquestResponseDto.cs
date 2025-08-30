using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

// Workshop (request) trả về cho màn duyệt / xem chi tiết
public class WorkshopRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid CraftVillageRequestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public WorkshopStatus Status { get; set; } = WorkshopStatus.Pending;

    public List<TicketTypeRequestResponseDto> TicketTypes { get; set; } = new();
    public List<RecurringRuleRequestResponseDto> RecurringRules { get; set; } = new();
    public List<WorkshopExceptionRequestResponseDto> Exceptions { get; set; } = new();

    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastUpdatedBy { get; set; }
}

public class TicketTypeRequestResponseDto
{
    public Guid Id { get; set; }
    public TicketType Type { get; set; } = TicketType.Visit;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsCombo { get; set; } = false;
    public int DurationMinutes { get; set; }
    public string? Content { get; set; }
    public List<WorkshopActivityRequestResponseDto> WorkshopActivities { get; set; } = new();
}

public class WorkshopActivityRequestResponseDto
{
    public Guid Id { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
    public int ActivityOrder { get; set; }
}

public class RecurringRuleRequestResponseDto
{
    public Guid Id { get; set; }
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<SessionRequestResponseDto> Sessions { get; set; } = new();
}

public class SessionRequestResponseDto
{
    public Guid Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Capacity { get; set; }
}

public class WorkshopExceptionRequestResponseDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
    public bool IsActive { get; set; } = true;
}
