using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TripPlanModels;

public class TripPlanResponseDto : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PickupAddress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ImageUrl { get; set; }
    public Guid UserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public TripPlanStatus? Status { get; set; }
    public string? StatusText { get; set; }
}
