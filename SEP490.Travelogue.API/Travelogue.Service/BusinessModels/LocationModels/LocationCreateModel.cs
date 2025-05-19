using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.LocationModels;
public class LocationCreateModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(0, 5)]
    public double Rating { get; set; } = 0;
    public Guid? TypeLocationId { get; set; }
    public Guid? DistrictId { get; set; }
    public HeritageRank HeritageRank { get; set; }
}
