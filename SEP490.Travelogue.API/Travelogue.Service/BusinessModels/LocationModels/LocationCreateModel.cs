using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationCreateModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }

    public double MinPrice { get; set; } = 0;
    public double MaxPrice { get; set; } = 0;

    [Range(-180, 180)]
    public double Longitude { get; set; }

    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public Guid? DistrictId { get; set; }
    [Required]
    public LocationType LocationType { get; set; }
    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
