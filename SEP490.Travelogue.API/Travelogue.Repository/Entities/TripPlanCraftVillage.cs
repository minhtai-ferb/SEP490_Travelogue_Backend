using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;

public sealed class TripPlanCraftVillage : BaseEntity
{
    [Required]
    public Guid TripPlanId { get; set; }

    [Required]
    public Guid CraftVillageId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? StartTime { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }
    public int Order { get; set; } = 0;

    public TripPlan? TripPlan { get; set; }
    public CraftVillage? CraftVillage { get; set; }
}
