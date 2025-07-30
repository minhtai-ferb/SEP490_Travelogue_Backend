using Microsoft.AspNetCore.Http;

namespace Travelogue.Service.BusinessModels.MediaModel;

public class UploadMediasDto
{
    public List<IFormFile> Files { get; set; }
}