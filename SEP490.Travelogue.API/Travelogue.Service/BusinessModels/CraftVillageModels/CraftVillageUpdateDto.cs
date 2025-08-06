using System.ComponentModel.DataAnnotations;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageUpdateDto
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

    // Craft Village specific properties
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool WorkshopsAvailable { get; set; } = false;
    public string? SignatureProduct { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Số năm lịch sử phải là số không âm")]
    public int? YearsOfHistory { get; set; }

    public bool IsRecognizedByUnesco { get; set; } = false;
    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
