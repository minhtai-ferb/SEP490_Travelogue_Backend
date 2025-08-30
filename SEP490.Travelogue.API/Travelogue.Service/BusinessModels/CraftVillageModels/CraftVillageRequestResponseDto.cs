using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.WorkshopModels;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerEmail { get; set; }
    public string OwnerFullName { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public Guid? DistrictId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    // public Guid LocationId { get; set; }
    public bool WorkshopsAvailable { get; set; }
    public string? SignatureProduct { get; set; }
    public int? YearsOfHistory { get; set; }
    public bool IsRecognizedByUnesco { get; set; }
    public CraftVillageRequestStatus Status { get; set; }
    public string? StatusText { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public List<MediaDto> Medias { get; set; } = new();
    public WorkshopRequestResponseDto? Workshop { get; set; }
}
