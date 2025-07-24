using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Travelogue.Service.BusinessModels.CuisineModels;
public class CuisineCreateWithMediaFileModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public Guid LocationId { get; set; }
    public string? Address { get; set; }
    public string? CuisineType { get; set; } // Loại âm thực 
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    [FromForm]
    public List<IFormFile>? ImageUploads { get; set; }
}
