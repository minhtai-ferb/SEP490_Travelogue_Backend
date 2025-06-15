using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.ExchangeSessionModels;

public class CreateExchangeSessionRequest
{
    [Required]
    public Guid TourGuideId { get; set; }

    [Required]
    public Guid TripPlanVersionId { get; set; }
}

