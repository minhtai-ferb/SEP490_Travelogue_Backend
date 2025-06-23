using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class TripPlanExchangeSession : BaseEntity
{
    [Required]
    public Guid TripPlanId { get; set; }

    [Required]
    public Guid TourGuideId { get; set; }

    [Required]
    public Guid CreatedByUserId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    public ExchangeSessionStatus FinalStatus { get; set; }

    public DateTimeOffset? ClosedAt { get; set; }

    // Navigation Properties

    [ForeignKey(nameof(TripPlanId))]
    public TripPlan TripPlan { get; set; } = null!;

    [ForeignKey(nameof(TourGuideId))]
    public TourGuide TourGuide { get; set; } = null!;

    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; } = null!;

    public ICollection<TripPlanExchange> Exchanges { get; set; } = new List<TripPlanExchange>();
}
