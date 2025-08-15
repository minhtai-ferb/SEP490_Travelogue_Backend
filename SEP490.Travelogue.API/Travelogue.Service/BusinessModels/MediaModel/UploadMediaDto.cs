using Microsoft.AspNetCore.Http;

namespace Travelogue.Service.BusinessModels.MediaModel;

public class UploadMediaDto
{
    public IFormFile File { get; set; }
}