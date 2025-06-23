using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public class TripPlanShare : BaseEntity
{
    public Guid TripPlanId { get; set; }
    public Guid UserId { get; set; }
    public PermissionTripPlan Permission { get; set; } = PermissionTripPlan.View;
    public TripPlan TripPlan { get; set; } = null!;
    public User User { get; set; } = null!;
}
