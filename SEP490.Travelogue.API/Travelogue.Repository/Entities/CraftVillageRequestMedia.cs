using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class CraftVillageRequestMedia : BaseEntity
{
    public string MediaUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public float SizeInBytes { get; set; }
    public bool IsThumbnail { get; set; }
    public Guid LocationId { get; set; }

    // Navigation Properties
    public CraftVillageRequest CraftVillageRequest { get; set; } = null!;
}