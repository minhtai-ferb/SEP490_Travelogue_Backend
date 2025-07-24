using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.LocationModels;
public class LocationUpdateWithMediaFileModel
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
    [FromForm]
    public List<IFormFile>? ImageUploads { get; set; }
}
