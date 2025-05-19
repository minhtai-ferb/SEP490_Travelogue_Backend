using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Travelogue.Service.BusinessModels.DistrictModels;
public class DistrictCreateWithMediaFileModel
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public float? Area { get; set; }
    [FromForm]
    public IFormFile? ImageUpload { get; set; }
}
