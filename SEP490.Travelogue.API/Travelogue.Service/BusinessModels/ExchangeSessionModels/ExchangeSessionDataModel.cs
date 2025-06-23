using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ExchangeSessionModels;

public class ExchangeSessionDataModel
{
    public Guid Id { get; set; }
    public Guid TourGuideId { get; set; }
    public string TourGuideName { get; set; } = string.Empty;
    public Guid TripPlanVersionId { get; set; }
    public Guid TripPlanId { get; set; }
    public ExchangeSessionStatus FinalStatus { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}
