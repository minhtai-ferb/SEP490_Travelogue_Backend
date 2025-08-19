using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationDataModel : BaseDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public string? Category { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
