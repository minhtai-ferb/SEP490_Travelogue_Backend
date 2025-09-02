using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopTicketPriceUpdateDto
{
    public Guid Id { get; set; }
    public Guid WorkshopId { get; set; }
    public Guid TicketTypeId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public PriceUpdateStatus Status { get; set; }
    public string? RequestReason { get; set; }
    public string? ModeratorNote { get; set; }
    public Guid RequestedBy { get; set; }
    public Guid? DecidedBy { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
}
