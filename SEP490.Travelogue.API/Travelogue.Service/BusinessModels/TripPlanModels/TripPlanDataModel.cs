namespace Travelogue.Service.BusinessModels.TripPlanModels;
public class TripPlanDataModel : BaseDataModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
