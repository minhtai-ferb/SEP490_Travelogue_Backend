using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;

    public ToursController(ITourService tripPlanService)
    {
        _tourService = tripPlanService;
    }

    /// <summary>
    /// Tạo mới tripPlan
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTour([FromBody] TourCreateModel model)
    {
        var result = await _tourService.AddTourAsync(model, new CancellationToken());
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
    public async Task<IActionResult> UpdateTour(Guid id, [FromBody] TourUpdateModel model)
    {
        await _tourService.UpdateTourAsync(id, model, new CancellationToken());
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
    public async Task<IActionResult> GetTourDetail(Guid id)
    {
        var result = await _tourService.GetTourByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TourDetailResponse>.OkResponseModel(
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
    public async Task<IActionResult> GetTour(string? title, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _tourService.GetPagedTourWithSearchAsync(title, pageNumber, pageSize, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan")
        ));
    }
}
