using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CuisineModels;

public class CuisineDataModel //: BaseDataModel
{
    // public Guid CuisineId { get; set; }
    // public string Name { get; set; } = string.Empty;
    // public string? Description { get; set; }
    // public string? Content { get; set; }
    // public Guid LocationId { get; set; }
    // public string? LocationName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? CuisineType { get; set; } // Loại âm thực 
    public string? SignatureProduct { get; set; }
    public string? CookingMethod { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    //public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
