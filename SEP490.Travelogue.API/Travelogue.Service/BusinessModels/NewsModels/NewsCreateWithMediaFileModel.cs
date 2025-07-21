using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.NewsModels;

public class NewsCreateWithMediaFileModel
{
    [Required, StringLength(100)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid? LocationId { get; set; }
    public NewsCategory NewsCategory { get; set; }
    public bool IsHighlighted { get; set; }
    [FromForm]
    public List<IFormFile>? ImageUploads { get; set; }
}
