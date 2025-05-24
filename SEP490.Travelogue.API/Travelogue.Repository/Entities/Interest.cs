using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class Interest : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<TourInterest>? TourInterests { get; set; }
    public ICollection<LocationInterest>? LocationInterests { get; set; }
    public ICollection<CraftVillageInterest>? CraftVillageInterests { get; set; }
    public ICollection<CuisineInterest>? CuisineInterests { get; set; }
    public ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
}

