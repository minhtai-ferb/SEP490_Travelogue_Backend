using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ExchangeModels
{
    public class ExchangeDataModel
    {
        public Guid ExchangeId { get; set; }
        public Guid TripPlanVersionId { get; set; }

        public Guid? SuggestedTripPlanVersionId { get; set; }
        public ExchangeSessionStatus Status { get; set; }
        public string? ExchangeSessionStatus { get; set; }
        public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? UserRespondedAt { get; set; }
        public string? UserResponseMessage { get; set; }
        // public Guid UserId { get; set; }
        // public Guid TripPlanId { get; set; }
        // public Guid TourGuideId { get; set; }

        // public Guid SessionId { get; set; }
        // public DateTimeOffset StartDate { get; set; }
        // public DateTimeOffset EndDate { get; set; }
    }
}