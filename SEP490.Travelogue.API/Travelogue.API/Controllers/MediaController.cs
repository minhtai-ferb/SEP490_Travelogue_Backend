using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Service.Commons.Interfaces;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
    {
        var url = await _mediaService.UploadImageAsync(image);
        return Ok(new { imageUrl = url });
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> images)
    {
        var urls = await _mediaService.UploadMultipleImagesAsync(images);
        return Ok(urls);
    }

    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> DeleteImage(string fileName)
    {
        var result = await _mediaService.DeleteImageAsync(fileName);
        return Ok(new { deleted = result });
    }

    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<string> fileNames)
    {
        var result = await _mediaService.DeleteImagesAsync(fileNames);
        return Ok(new { deleted = result });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllImages()
    {
        var result = await _mediaService.GetAllImagesAsync();
        return Ok(result);
    }
}
