using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.OrderModels;

public class BookingDataModel
{
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? VersionId { get; set; }
    public DateTimeOffset BookingDate { get; set; }

    public DateTimeOffset? ScheduledStartDate { get; set; }
    public DateTimeOffset? ScheduledEndDate { get; set; }
    public BookingStatus Status { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }
    public bool IsOpenToJoin { get; set; } = false;
    public decimal TotalPaid { get; set; }
}
