namespace Travelogue.Service.BusinessModels.DashboardModels;

public class RevenueStatisticResponse
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal GrossRevenue { get; set; }
    public List<RevenueDataItem> RevenueDataItem { get; set; } = new();
}

public class RevenueDataItem
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Commission { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal GrossRevenue { get; set; }
}
