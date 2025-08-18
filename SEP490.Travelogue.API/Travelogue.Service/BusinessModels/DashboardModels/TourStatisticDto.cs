using Travelogue.Service.BusinessModels.BookingModels;

namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TourStatisticDto
{
    // public Guid TourId { get; set; }
    // public string TourName { get; set; }
    // public string TourStatus { get; set; }
    // public DateTime StartDate { get; set; }
    // public DateTime EndDate { get; set; }

    // thống kê booking
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int ExpiredBookings { get; set; }
    public int CancelledByProviderBookings { get; set; }
    public int CompletedBookings { get; set; }
    public double CompletionRate { get; set; }

    // doanh thu
    // doanh thu từ booking đã hoàn thành r
    public decimal TotalRevenue { get; set; }
    public decimal ConfirmedRevenue { get; set; }
    public decimal CompletedRevenue { get; set; }
    // doanh thu mất do đơn bị hủy
    public decimal LostRevenue { get; set; }

    public List<BookingDataModel> Bookings { get; set; }
}