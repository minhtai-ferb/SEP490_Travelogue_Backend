using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class RoleDistrict : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid DistrictId { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;
    public District District { get; set; } = null!;
}
