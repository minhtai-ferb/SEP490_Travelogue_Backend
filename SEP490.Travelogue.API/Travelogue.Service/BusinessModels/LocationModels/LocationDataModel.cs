using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationDataModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; } = 0;
    public Guid? TypeLocationId { get; set; }
    public string? TypeLocationName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public HeritageRank HeritageRank { get; set; }
    public string HeritageRankName { get; set; } = string.Empty;
    public List<MediaResponse> Medias { get; set; } = new List<MediaResponse>();
}
