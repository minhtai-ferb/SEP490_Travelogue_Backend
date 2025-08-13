using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Entities;

public sealed class CraftVillageRequest : BaseEntity
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public Guid? DistrictId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // public Guid LocationId { get; set; }
    public Guid OwnerId { get; set; }

    public bool WorkshopsAvailable { get; set; } = false;
    public string? SignatureProduct { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số năm lịch sử phải là số không âm")]
    public int? YearsOfHistory { get; set; }

    public bool IsRecognizedByUnesco { get; set; } = false;

    public CraftVillageRequestStatus Status { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public List<MediaRequest> Medias { get; set; } = new();
    // public ICollection<CraftVillageRequestMedia> CraftVillageRequestMedias { get; set; } = new List<CraftVillageRequestMedia>();
}

public class MediaRequest
{
    public string MediaUrl { get; set; } = string.Empty;
    public bool IsThumbnail { get; set; }
}