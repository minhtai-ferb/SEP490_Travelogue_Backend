using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ExchangeModels;

namespace Travelogue.Service.BusinessModels.ExchangeSessionModels
{
    public class ExchangeSessionDataDetailModel
    {
        public Guid Id { get; set; }
        public Guid TourGuideId { get; set; }
        public string TourGuideName { get; set; } = string.Empty;
        public Guid TripPlanId { get; set; }
        public ExchangeSessionStatus FinalStatus { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public string TripPlanName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public ExchangeDataModel ExchangeData { get; set; } = new ExchangeDataModel();
    }
}