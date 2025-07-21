using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class ReviewTourGuideRequestDto
{
    public TourGuideRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}
