using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TourBookingResponse
{
    public Guid TourId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }

    public int TotalDays { get; set; }

    public TourStatus Status { get; set; }
    public string? StatusText { get; set; }
    public TourType? TourType { get; set; }
    public string? TourTypeText { get; set; }
    public List<TourBookingItem> Bookings { get; set; } = new();
}

public class TourBookingItem
{
    public Guid BookingId { get; set; }
    public Guid ScheduleId { get; set; }

    public DateTime DepartureDate { get; set; }

    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid TourId { get; set; }
    public string? TourName { get; set; }
    public BookingStatus Status { get; set; }
    public string? StatusText { get; set; }

    public DateTimeOffset BookingDate { get; set; }

    public BookingType BookingType { get; set; }
    public string? BookingTypeText { get; set; }
    public decimal FinalPrice { get; set; }
}

