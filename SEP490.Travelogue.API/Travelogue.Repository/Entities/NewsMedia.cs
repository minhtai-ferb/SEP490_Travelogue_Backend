using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class NewsMedia : BaseEntity
{
    public string MediaUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public float SizeInBytes { get; set; } = 0;
    public bool IsThumbnail { get; set; } = false;

    public Guid NewsId { get; set; }
    // Navigation Properties
    public News News { get; set; } = null!;
}
