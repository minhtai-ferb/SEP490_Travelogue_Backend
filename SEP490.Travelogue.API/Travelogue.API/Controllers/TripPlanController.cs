using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TripPlanModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripPlansController : ControllerBase
{
    private readonly ITripPlanService _tripPlanService;

    public TripPlansController(ITripPlanService tripPlanService)
    {
        _tripPlanService = tripPlanService;
    }

    /// <summary>
    /// Tạo mới tripPlan
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTripPlan([FromBody] TripPlanCreateModel model)
    {
        var result = await _tripPlanService.AddTripPlanAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "trip plan")
        ));
    }

    /// <summary>
    /// Cập nhật thông tin trip plan
    /// </summary>
    /// <param name="tripPlanId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("trip-plan/{tripPlanId}")]
    public async Task<IActionResult> UpdateTripPlan(Guid tripPlanId, [FromBody] TripPlanUpdateDto model)
    {
        var result = await _tripPlanService.UpdateTripPlanAsync(tripPlanId, model);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "trip plan")
        ));
    }

    /// <summary>
    /// Cập nhật tripPlan
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    // [HttpPut("old-update/{id}")]
    // public async Task<IActionResult> OldUpdateTripPlan(Guid id, [FromBody] TripPlanUpdateModel model)
    // {
    //     await _tripPlanService.UpdateTripPlanAsync(id, model, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: true,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "news")
    //     ));
    // }

    [HttpPut("trip-plan-location/{id}")]
    public async Task<IActionResult> UpdateTripPlan(Guid id, [FromBody] List<UpdateTripPlanLocationDto> model)
    {
        await _tripPlanService.UpdateTripPlanLocationsAsync(id, model);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy chi tiết trip plan theo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTripPlanDetail(Guid id)
    {
        var result = await _tripPlanService.GetTripPlanByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TripPlanDetailResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan")
        ));
    }

    /// <summary>
    /// Lấy danh sách trip plan với phân trang và tìm kiếm theo tiêu đề của người đang đăng nhập
    /// </summary>
    /// <param name="title"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetTripPlan(string? title, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _tripPlanService.GetPagedTripPlanWithSearchAsync(title, pageNumber, pageSize, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan")
        ));
    }

    /// <summary>
    /// Lấy danh sách trip plan với phân trang và tìm kiếm theo tiêu đề 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<IActionResult> GetTripPlanPage(string? title)
    {
        var result = await _tripPlanService.GetPagedTripPlanPageAsync(title, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan")
        ));
    }

    [HttpPut("{tripPlanId}/image-url")]
    public async Task<IActionResult> UpdateTripPlanImageUrl(Guid tripPlanId, [FromBody] UpdateImageUrlRequest request, CancellationToken cancellationToken)
    {
        var result = await _tripPlanService.UpdateTripPlanImageUrlAsync(tripPlanId, request.ImageUrl, cancellationToken);
        if (!result)
        {
            return NotFound("Trip plan not found or update failed.");
        }

        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "trip plan")
        ));
    }

    /// <summary>
    /// Xóa trip plan
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deletedImages"></param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id)
    {
        await _tripPlanService.DeleteTripPlanAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }
}