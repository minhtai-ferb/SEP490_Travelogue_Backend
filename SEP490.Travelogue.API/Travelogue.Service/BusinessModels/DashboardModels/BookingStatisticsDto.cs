using Travelogue.Service.BusinessModels.BookingModels;

namespace Travelogue.Service.BusinessModels.DashboardModels;

public class BookingStatisticsDto
{
    // public List<BookingDataModel> Bookings { get; set; }
    public int TotalTours { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ActualRevenue { get; set; }
    public double CancelRate { get; set; }

    public Dictionary<string, int> StatusCount { get; set; }

    // Top dữ liệu
    public List<TopItemDto> TopTours { get; set; }
    public List<TopItemDto> TopGuides { get; set; }
}
