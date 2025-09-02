using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopTicketPriceUpdateListItemDto
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }
    public string WorkshopName { get; set; } = string.Empty;

    public Guid CraftVillageId { get; set; }
    public string CraftVillageName { get; set; } = string.Empty;

    public Guid TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;

    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }

    public PriceUpdateStatus Status { get; set; }
    public string? RequestReason { get; set; }
    public string? ModeratorNote { get; set; }

    public Guid RequestedBy { get; set; }
    public string? RequestedByName { get; set; }

    public Guid? DecidedBy { get; set; }
    public string? DecidedByName { get; set; }

    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
}