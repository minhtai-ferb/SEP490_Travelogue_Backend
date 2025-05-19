using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Travelogue.Service.BusinessModels.ExperienceModels;
public class ExperienceCreateWithMediaFileModel
{
    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? TypeExperienceId { get; set; }
    [FromForm]
    public List<IFormFile>? ImageUploads { get; set; }
}
