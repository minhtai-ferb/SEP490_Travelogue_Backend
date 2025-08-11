using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TourScheduleBookingsResponse
{
    public Guid ScheduleId { get; set; }
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public int MaxParticipant { get; set; }
    public int CurrentBooked { get; set; }
    public TourScheduleStatus Status { get; set; }
    public string? Reason { get; set; }
    public decimal AdultPrice { get; set; }
    public decimal ChildrenPrice { get; set; }

    public List<TourBookingItem> Bookings { get; set; } = new();
}