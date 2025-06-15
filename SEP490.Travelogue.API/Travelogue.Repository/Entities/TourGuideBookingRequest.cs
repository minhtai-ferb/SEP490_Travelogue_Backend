using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public class TripPlanExchange : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TripPlanId { get; set; }

    [Required]
    public Guid TripPlanVersionId { get; set; }

    public Guid? SuggestedTripPlanVersionId { get; set; }

    [Required]
    public Guid TourGuideId { get; set; }

    public Guid SessionId { get; set; }
    public TripPlanExchangeSession Session { get; set; } = null!;

    [Required]
    public DateTimeOffset StartDate { get; set; }

    [Required]
    public DateTimeOffset EndDate { get; set; }

    [Required]
    public ExchangeSessionStatus Status { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset RequestedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTimeOffset? UserRespondedAt { get; set; }

    [StringLength(1000)]
    public string? UserResponseMessage { get; set; }

    // Navigation Properties

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(TripPlanId))]
    public TripPlan TripPlan { get; set; } = null!;

    [ForeignKey(nameof(TripPlanVersionId))]
    public TripPlanVersion TripPlanVersion { get; set; } = null!;

    [ForeignKey(nameof(SuggestedTripPlanVersionId))]
    public TripPlanVersion? SuggestedTripPlanVersion { get; set; }

    [ForeignKey(nameof(TourGuideId))]
    public TourGuide TourGuide { get; set; } = null!;
}
