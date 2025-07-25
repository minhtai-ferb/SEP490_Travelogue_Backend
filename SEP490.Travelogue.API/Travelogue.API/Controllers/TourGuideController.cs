using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    /// <summary>
    /// Lấy thông tin Tour Guide theo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetTourGuideByIdAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guide")
        ));
    }

    /// <summary>
    /// Lấy danh sách tất cả Tour Guides.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<IActionResult> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetAllTourGuidesAsync(cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guides")
        ));
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetTourGuidesByFilterAsync([FromQuery] TourGuideFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetTourGuidesByFilterAsync(filter, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guides")
        ));
    }

    /// <summary>
    /// Cấp quyền Tour Guide cho người dùng dựa trên email.
    /// </summary>
    /// <param name="emails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignToTourGuideAsync([FromBody] List<string> emails, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.AssignToTourGuideAsync(emails, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateTourGuide([FromBody] TourGuideUpdateModel model, CancellationToken cancellationToken)
    {
        var updatedData = await _tourGuideService.UpdateTourGuideAsync(model, cancellationToken);
        return Ok(ResponseModel<TourGuideDataModel>.OkResponseModel(
            data: updatedData,
            message: ResponseMessages.UPDATE_SUCCESS
        ));
    }

    [HttpPost("certification")]
    public async Task<IActionResult> AddCertification([FromBody] CertificationDto dto, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.AddCertificationAsync(dto, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpDelete("{certificationId:guid}")]
    public async Task<IActionResult> SoftDeleteCertification(Guid certificationId, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.SoftDeleteCertificationAsync(certificationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }
}