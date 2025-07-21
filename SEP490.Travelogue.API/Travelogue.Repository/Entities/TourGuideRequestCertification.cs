using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideRequestCertification : BaseEntity
{
    [Required]
    public Guid TourGuideRequestId { get; set; }
    public TourGuideRequest TourGuideRequest { get; set; } = null!;

    [Required]
    public string Name { get; set; }

    public string? CertificateUrl { get; set; }
}