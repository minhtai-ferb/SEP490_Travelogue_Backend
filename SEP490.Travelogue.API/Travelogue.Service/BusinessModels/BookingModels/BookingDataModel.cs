using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.BookingModels;

public class BookingDataModel
{
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? VersionId { get; set; }
    public DateTime OrderDate { get; set; }

    public DateTime? ScheduledDate { get; set; }
    public BookingStatus Status { get; set; }

    public DateTime? CancelledAt { get; set; }
    public bool IsOpenToJoin { get; set; }
    public decimal TotalPaid { get; set; }
}
