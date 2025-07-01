using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class Order : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    public Guid? TourId { get; set; }
    public Guid? TourGuideId { get; set; }
    public Guid? TripPlanVersionId { get; set; }
    public Guid TourVersionId { get; set; }

    [Required]
    public DateTimeOffset OrderDate { get; set; }

    public DateTimeOffset? ScheduledStartDate { get; set; }
    public DateTimeOffset? ScheduledEndDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }
    public bool IsOpenToJoin { get; set; } = false;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPaid { get; set; }

    public User? User { get; set; }
    public Tour? Tour { get; set; }
    public TourGuide? TourGuide { get; set; }
    public TripPlan? TripPlan { get; set; }
    public TripPlanVersion? TripPlanVersion { get; set; }
    public TourPlanVersion TourPlanVersion { get; set; } = default!;
}
