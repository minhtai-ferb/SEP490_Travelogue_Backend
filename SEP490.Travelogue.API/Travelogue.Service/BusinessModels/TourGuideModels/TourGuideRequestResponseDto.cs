using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class TourGuideRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Introduction { get; set; }
    public decimal Price { get; set; }
    public TourGuideRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public List<CertificationDto> Certifications { get; set; }
}