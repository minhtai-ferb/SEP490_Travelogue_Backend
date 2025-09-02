namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class RequestPriceChangeDto
{
    public decimal NewPrice { get; set; }
    public string? Reason { get; set; }
}
