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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTripPlan(Guid id, [FromBody] TripPlanUpdateModel model)
    {
        await _tripPlanService.UpdateTripPlanAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "news")
        ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTripPlanDetail(Guid id)
    {
        var result = await _tripPlanService.GetTripPlanByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TripPlanDetailResponse>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "trip plan")
        ));
    }
}
