using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TourGuideRequest : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public required string Introduction { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    public TourGuideRequestStatus Status { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }

    public ICollection<TourGuideRequestCertification> Certifications { get; set; } = new List<TourGuideRequestCertification>();
}
