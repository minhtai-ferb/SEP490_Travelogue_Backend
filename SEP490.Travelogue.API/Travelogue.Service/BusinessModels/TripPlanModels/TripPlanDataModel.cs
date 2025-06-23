namespace Travelogue.Service.BusinessModels.TripPlanModels;

public class TripPlanDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid? TripPlanVersionId { get; set; }
    public bool IsFromTourGuide { get; set; } = false;
    public Guid UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
}
