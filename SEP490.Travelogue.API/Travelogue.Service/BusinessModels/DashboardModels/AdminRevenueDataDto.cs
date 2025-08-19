namespace Travelogue.Service.BusinessModels.DashboardModels;

public class AdminRevenueDataDto
{
    public AdminRevenueDto GrossRevenue { get; set; }
    public AdminRevenueDto NetRevenue { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class AdminRevenueDto
{
    public decimal Total { get; set; }
    public AdminRevenueByCategoryDto ByCategory { get; set; }
    public List<AdminDailyStatDto> DailyStats { get; set; }
}

public class AdminRevenueByCategoryDto
{
    public decimal Tour { get; set; }
    public decimal CommissionTourGuide { get; set; }
    public decimal CommissionWorkshop { get; set; }
}

public class AdminDailyStatDto
{
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public decimal Tour { get; set; }
    public decimal CommissionTourGuide { get; set; }
    public decimal CommissionWorkshop { get; set; }
}
