using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Entities;

public sealed class TourMedia : BaseEntity
{
    public string MediaUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public float SizeInBytes { get; set; } = 0;
    public bool IsThumbnail { get; set; } = false;

    public Guid TourId { get; set; }
    // Navigation Properties
    public Tour Tour { get; set; } = null!;
}
