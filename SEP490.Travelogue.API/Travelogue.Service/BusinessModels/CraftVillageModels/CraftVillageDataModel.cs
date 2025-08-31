using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.WorkshopModels;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageDataModel // : BaseDataModel
{
    // public Guid CraftVillageId { get; set; }
    // public string Name { get; set; } = string.Empty;
    // public string? Description { get; set; }
    // public string? Content { get; set; }
    // public Guid LocationId { get; set; }
    // public string? LocationName { get; set; }
    // public string? Address { get; set; }
    public Guid OwnerId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? SignatureProduct { get; set; }
    public int? YearsOfHistory { get; set; }
    public bool WorkshopsAvailable { get; set; }

    public bool IsRecognizedByUnesco { get; set; } = false;
    public WorkshopDetailDto? Workshop { get; set; } = new();
    // public double Latitude { get; set; }
    // public double Longitude { get; set; }
    // public Guid? DistrictId { get; set; }
    // public string Category { get; set; }
    // public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
