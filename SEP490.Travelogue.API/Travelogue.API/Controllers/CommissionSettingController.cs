using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.CommissionSettingModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommissionSettingController : ControllerBase
{
    private readonly ICommissionSettingService _commissionSettingService;

    public CommissionSettingController(ICommissionSettingService commissionSettingService)
    {
        _commissionSettingService = commissionSettingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _commissionSettingService.GetAllAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var result = await _commissionSettingService.GetCurrentAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var result = await _commissionSettingService.GetByDateAsync(date);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommissionSettingRequest dto)
    {
        var result = await _commissionSettingService.UpdateAsync(id, dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }
}
