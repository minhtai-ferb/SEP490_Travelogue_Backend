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
    /// Cập nhật tripPlan
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTripPlan(Guid id, [FromBody] TripPlanUpdateModel model)
    {
        await _tripPlanService.UpdateTripPlanAsync(id, model, new CancellationToken());
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
        return Ok(ResponseModel<TripPlanDetailResponse>.OkResponseModel(
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
    /// Người dùng gửi request --> tạo phiên bản mới từ phiên bản gốc --> gán vào trip plan version trong request để biết được version nào là phiên bản mà họ gửi cho tour guide
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("version/{id}")]
    public async Task<IActionResult> GetTripPlanVersion(Guid id)
    {
        // var result = await _tripPlanService.CreateVersionFromTripPlanAsync(id, string.Empty);
        var result = await _tripPlanService.CreateVersionFromTripPlanAsync(id, string.Empty);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan version")
        ));
    }
}
