using Microsoft.AspNetCore.Mvc;
using Travelogue.Service.Commons.Interfaces;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Repository.Bases.Responses;

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

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
    {
        var response = await _mediaService.UploadImageAsync(image);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "image")
        ));
    }

    [HttpPost("upload-multiple-images")]
    public async Task<IActionResult> UploadMultipleImages([FromForm] List<IFormFile> images)
    {
        var response = await _mediaService.UploadMultipleImagesAsync(images);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "images")
        ));
    }

    [HttpPost("upload-certification")]
    public async Task<IActionResult> UploadCertification([FromForm] IFormFile certification)
    {
        var response = await _mediaService.UploadDocumentAsync(certification);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "certification")
        ));
    }

    [HttpPost("upload-multiple-certifications")]
    public async Task<IActionResult> UploadMultipleCertifications([FromForm] List<IFormFile> certifications)
    {
        var response = await _mediaService.UploadMultipleDocumentsAsync(certifications);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "certifications")
        ));
    }


    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> DeleteImage(string fileName)
    {
        var response = await _mediaService.DeleteImageAsync(fileName);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }

    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<string> fileNames)
    {
        var response = await _mediaService.DeleteImagesAsync(fileNames);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllImages()
    {
        var response = await _mediaService.GetAllImagesAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }
}
