using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.ExperienceModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ExperienceController : ControllerBase
{
    private readonly IExperienceService _experienceService;

    public ExperienceController(IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }

    /// <summary>
    /// Tạo mới experience
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateExperience([FromBody] ExperienceCreateModel model)
    {
        var result = await _experienceService.AddExperienceAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "experience")
        ));
    }

    /// <summary>
    /// Xóa experience theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExperience(Guid id)
    {
        await _experienceService.DeleteExperienceAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "experience")
        ));
    }

    /// <summary>
    /// Lấy tất cả experience
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllExperiences()
    {
        var experiences = await _experienceService.GetAllExperiencesAsync(new CancellationToken());
        return Ok(ResponseModel<List<ExperienceDataModel>>.OkResponseModel(
            data: experiences,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experience")
        ));
    }

    /// <summary>
    /// Lấy experience theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetExperienceById(Guid id)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<ExperienceDataModel>.OkResponseModel(
            data: experience,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experience")
        ));
    }
    /// <summary>
    /// Cập nhật experience
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] ExperienceUpdateModel model)
    {
        await _experienceService.UpdateExperienceAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "experience")
        ));
    }

    /// <summary>
    /// Lấy danh sách experience phân trang theo tiêu đề
    /// </summary>
    /// <param name="title"></param>
    /// <param name="typeExperienceId"></param>
    /// <param name="locationId"></param>
    /// <param name="eventId"></param>
    /// <param name="districtId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedExperienceWithSearch(string? title, Guid? typeExperienceId, Guid? locationId, Guid? eventId, Guid? districtId, int pageNumber = 1, int pageSize = 10)
    {
        var experiences = await _experienceService.GetPagedExperiencesWithSearchAsync(title, typeExperienceId, locationId, eventId, districtId, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: experiences.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experience"),
            totalCount: experiences.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    {
        var result = await _experienceService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpGet("admin-get")]
    public async Task<IActionResult> GetAllExperiencesAdmin()
    {
        var experiences = await _experienceService.GetAllExperienceAdminAsync();
        return Ok(ResponseModel<List<ExperienceDataModel>>.OkResponseModel(
            data: experiences,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experience")
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedExperience(int pageNumber = 1, int pageSize = 10)
    //{
    //    var experiences = await _experienceService.GetPagedExperiencesAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: experiences.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experience"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
