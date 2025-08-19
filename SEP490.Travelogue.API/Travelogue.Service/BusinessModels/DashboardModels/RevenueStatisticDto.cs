namespace Travelogue.Service.BusinessModels.DashboardModels;

public class RevenueStatisticDto
{
    public RevenueDetailDto GrossRevenue { get; set; }
    public RevenueDetailDto NetRevenue { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class RevenueDetailDto
{
    public decimal Total { get; set; }
    public RevenueByCategoryDto ByCategory { get; set; }
    public List<DailyRevenueStatDto> DailyStats { get; set; }
}

public class RevenueByCategoryDto
{
    public decimal Tour { get; set; }
    public decimal BookingTourGuide { get; set; }
    public decimal BookingWorkshop { get; set; }
}

public class DailyRevenueStatDto
{
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public decimal Tour { get; set; }
    public decimal BookingTourGuide { get; set; }
    public decimal BookingWorkshop { get; set; }
}
