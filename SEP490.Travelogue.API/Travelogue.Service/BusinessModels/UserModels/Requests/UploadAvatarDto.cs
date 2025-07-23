using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Travelogue.Service.BusinessModels.UserModels.Requests;

public class UploadAvatarDto
{
    [Required]
    public IFormFile File { get; set; }
}