namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageWorkshopDashboardDto
{
    public Guid CraftVillageId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal GrossTotal { get; set; }
    public decimal NetTotal { get; set; }
    public decimal GrossDirectTotal { get; set; }
    public decimal NetDirectTotal { get; set; }
    public decimal GrossFromToursTotal { get; set; }
    public decimal NetFromToursTotal { get; set; }
    public List<DailyRevenueDto> Daily { get; set; } = new();
}

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Gross { get; set; }
    public decimal Net { get; set; }
    public decimal GrossDirect { get; set; }
    public decimal NetDirect { get; set; }
    public decimal GrossFromTours { get; set; }
    public decimal NetFromTours { get; set; }
}
