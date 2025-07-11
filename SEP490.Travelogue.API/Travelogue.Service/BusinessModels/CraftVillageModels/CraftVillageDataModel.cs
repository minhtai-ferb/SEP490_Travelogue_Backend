using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CraftVillageModels;

public class CraftVillageDataModel : BaseDataModel
{
    public Guid CraftVillageId { get; set; }
    // public string Name { get; set; } = string.Empty;
    // public string? Description { get; set; }
    // public string? Content { get; set; }
    // public Guid LocationId { get; set; }
    // public string? LocationName { get; set; }
    // public string? Address { get; set; }
    public decimal? StarRating { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? SignatureProduct { get; set; }
    public int? YearsOfHistory { get; set; }

    public bool IsRecognizedByUNESCO { get; set; } = false;
    // public double Latitude { get; set; }
    // public double Longitude { get; set; }
    // public Guid? DistrictId { get; set; }
    public List<string>? Categories { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
