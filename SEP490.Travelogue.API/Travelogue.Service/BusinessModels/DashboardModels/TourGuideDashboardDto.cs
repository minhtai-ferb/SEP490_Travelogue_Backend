namespace Travelogue.Service.BusinessModels.DashboardModels;

public class TourGuideDashboardDto
{
    public Guid TourGuideId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public decimal GrossRevenueDirect { get; set; }
    public decimal NetRevenueDirect { get; set; }
    public int BookingsDirectCount { get; set; }
    public int BookingsFromToursCount { get; set; }
    public int BookingsAllCount { get; set; }

    public List<TourGuideDailyStatDto> DailyStats { get; set; } = new();
}

public class TourGuideDailyStatDto
{
    public DateTime Date { get; set; }

    public decimal RevenueDirectGross { get; set; }
    public decimal RevenueDirectNet { get; set; }
    public int BookingsDirect { get; set; }
    public int BookingsFromTours { get; set; }
    public int BookingsAll { get; set; }
}
