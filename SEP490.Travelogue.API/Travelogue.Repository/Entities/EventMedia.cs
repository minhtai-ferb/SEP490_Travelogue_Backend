using Travelogue.Repository.Bases.BaseEntitys;

namespace Travelogue.Repository.Entities;
public sealed class EventMedia : BaseEntity
{
    //public string Url { get; set; } = string.Empty;
    //public string FileKey { get; set; } = string.Empty;
    public string MediaUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string FileType { get; set; }
    public float SizeInBytes { get; set; }
    public bool IsThumbnail { get; set; }
    public Guid EventId { get; set; }

    // Navigation Properties
    public Event Event { get; set; } = null!;
    //public Guid EntityId { get; set; }
    //public EntityType EntityType { get; set; }
}
