using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.ExchangeModels
{
    public class ExchangeCreateModel
    {
        public Guid TripPlanId { get; set; }
        public Guid TripPlanVersionId { get; set; }

        public Guid? SuggestedTripPlanVersionId { get; set; }
        public Guid TourGuideId { get; set; }

        public Guid SessionId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public ExchangeSessionStatus Status { get; set; }
        public string? UserResponseMessage { get; set; }
    }
}