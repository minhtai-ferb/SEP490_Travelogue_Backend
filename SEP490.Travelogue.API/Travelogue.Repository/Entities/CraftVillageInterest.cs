using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class CraftVillageInterest : BaseEntity
{
    public Guid CraftVillageId { get; set; }
    public Guid InterestId { get; set; }

    public Interest? Interest { get; set; }
    public CraftVillage? CraftVillage { get; set; }
}

