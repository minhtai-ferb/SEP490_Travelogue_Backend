using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.SystemSettingModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemSettingController : ControllerBase
{
    private readonly ISystemSettingService _systemSettingService;

    public SystemSettingController(ISystemSettingService systemSettingService)
    {
        _systemSettingService = systemSettingService;
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> Update([FromBody] SystemSettingUpdateDto dto)
    {
        var result = await _systemSettingService.UpdateAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour schedule")
        ));
    }

    [HttpGet]
    public async Task<ActionResult<List<SystemSettingDto>>> GetAll()
    {
        var result = await _systemSettingService.GetAllAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour schedule")
        ));
    }
}