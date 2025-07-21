using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class Certification : BaseEntity
{
    public Guid TourGuideId { get; set; }
    public TourGuide TourGuide { get; set; }

    public string Name { get; set; }

    public string? CertificateUrl { get; set; }
}
