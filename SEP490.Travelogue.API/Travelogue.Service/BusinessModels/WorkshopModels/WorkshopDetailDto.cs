using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public WorkshopStatus Status { get; set; }

    public Guid CraftVillageId { get; set; }

    public Guid LocationId { get; set; }
    public string? CraftVillageName { get; set; } = string.Empty;
    public List<WorkshopTicketTypeDto> TicketTypes { get; set; } = new();
    public List<WorkshopScheduleDto> Schedules { get; set; } = new();
    public List<WorkshopRecurringRuleDto> RecurringRules { get; set; } = new();
    public List<WorkshopExceptionDto> Exceptions { get; set; } = new();
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}

public class WorkshopTicketTypeDto
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }
    public TicketType Type { get; set; } = TicketType.Visit;

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsCombo { get; set; }
    public int DurationMinutes { get; set; }
    public string? Content { get; set; }

    public List<WorkshopActivityDto> Activities { get; set; } = new();
}

public class WorkshopActivityDto
{
    public Guid Id { get; set; }

    public Guid WorkshopTicketTypeId { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public int ActivityOrder { get; set; }
}

public class WorkshopScheduleDto
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public int CurrentBooked { get; set; }
    public string? Notes { get; set; }
    public ScheduleStatus Status { get; set; }
}

public class WorkshopRecurringRuleDto
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }

    public ICollection<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek>();
    public List<string> DaysOfWeekText { get; set; } = new();

    public string DaysOfWeekDisplay { get; set; } = string.Empty;

    public List<WorkshopSessionRuleDto> Sessions { get; set; } = new();
}

public class WorkshopSessionRuleDto
{
    public Guid Id { get; set; }

    public Guid RecurringRuleId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Capacity { get; set; }
}

public class WorkshopExceptionDto
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }
    public DateTime Date { get; set; }
    public string? Reason { get; set; }
}
