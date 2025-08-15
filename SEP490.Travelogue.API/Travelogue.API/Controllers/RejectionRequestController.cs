using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RejectionRequestController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public RejectionRequestController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    /// <summary>
    /// tour guide muốn hủy 1 lịch trình của 1 schedule hoặc booking
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateRejectionRequest([FromBody] RejectionRequestCreateDto dto)
    {
        var response = await _tourGuideService.CreateRejectionRequestAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    [HttpPut("{requestId}/approve")]
    public async Task<IActionResult> ApproveRejectionRequest(Guid requestId, Guid newTourGuideId)
    {
        var response = await _tourGuideService.ApproveRejectionRequestAsync(requestId, newTourGuideId);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    [HttpPut("{requestId}/reject")]
    public async Task<IActionResult> RejectRejectionRequest(Guid requestId, [FromBody] RejectRejectionRequestDto dto)
    {
        var response = await _tourGuideService.RejectRejectionRequestAsync(requestId, dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }
}
