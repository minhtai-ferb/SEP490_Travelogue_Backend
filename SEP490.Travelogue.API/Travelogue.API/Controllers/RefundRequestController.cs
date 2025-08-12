using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.RefundRequestModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundRequestController : ControllerBase
{
    private readonly IRefundRequestService _refundRequestService;

    public RefundRequestController(IRefundRequestService refundRequestService)
    {
        _refundRequestService = refundRequestService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<object>))]
    public async Task<IActionResult> CreateRefundRequest([FromBody] RefundRequestCreateDto dto)
    {
        var result = await _refundRequestService.CreateRefundRequestAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS));
    }

    [HttpPut("{refundRequestId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<object>))]
    public async Task<IActionResult> ApproveRefundRequest(Guid refundRequestId)
    {
        var result = await _refundRequestService.ApproveRefundRequestAsync(refundRequestId);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS));
    }

    [HttpPut("{refundRequestId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<object>))]
    public async Task<IActionResult> RejectRefundRequest(Guid refundRequestId, [FromQuery] string rejectionReason)
    {
        var result = await _refundRequestService.RejectRefundRequestAsync(refundRequestId, rejectionReason);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS));
    }

    [HttpGet("admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<RefundRequestDto>>))]
    public async Task<IActionResult> GetRefundRequestsForAdmin([FromQuery] RefundRequestAdminFilter filter)
    {
        var result = await _refundRequestService.GetRefundRequestsForAdminAsync(filter);
        return Ok(ResponseModel<List<RefundRequestDto>>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<List<RefundRequestDto>>))]
    public async Task<IActionResult> GetRefundRequestsForUser([FromQuery] RefundRequestUserFilter filter)
    {
        var result = await _refundRequestService.GetRefundRequestsForUserAsync(filter);
        return Ok(ResponseModel<List<RefundRequestDto>>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpGet("{refundRequestId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<RefundRequestDto>))]
    public async Task<IActionResult> GetRefundRequestDetail(Guid refundRequestId)
    {
        var result = await _refundRequestService.GetRefundRequestDetailAsync(refundRequestId);
        return Ok(ResponseModel<RefundRequestDto>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpPatch("{refundRequestId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<object>))]
    public async Task<IActionResult> DeleteRefundRequest(Guid refundRequestId)
    {
        await _refundRequestService.DeleteRefundRequestAsync(refundRequestId);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessages.SUCCESS
        ));
    }
}
