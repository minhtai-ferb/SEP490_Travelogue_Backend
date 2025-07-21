namespace Travelogue.Service.BusinessModels.TourGuideModels;

public class CreateTourGuideRequestDto
{
    public string Introduction { get; set; }
    public decimal Price { get; set; }
    public List<CertificationDto> Certifications { get; set; } = new List<CertificationDto>();
}
