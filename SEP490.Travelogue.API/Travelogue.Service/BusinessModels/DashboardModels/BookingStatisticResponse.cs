namespace Travelogue.Service.BusinessModels.DashboardModels;

public class BookingStatisticResponse
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<BookingStatisticItem> Data { get; set; } = new();
}

public class BookingStatisticItem
{
    public DateTime Day { get; set; }
    public int BookingSchedule { get; set; }
    public int BookingTourGuide { get; set; }
    public int BookingWorkshop { get; set; }
}