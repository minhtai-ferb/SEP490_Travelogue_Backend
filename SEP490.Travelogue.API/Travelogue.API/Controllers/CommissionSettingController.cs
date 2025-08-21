using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CommissionSettingModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommissionSettingController : ControllerBase
{
    private readonly ICommissionRateService _commissionRateService;

    public CommissionSettingController(ICommissionRateService commissionRateService)
    {
        _commissionRateService = commissionRateService;
    }

    /// <summary>
    /// Lấy tất cả commission rates
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _commissionRateService.GetAllAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("group")]
    public async Task<IActionResult> GetAllGroup()
    {
        var result = await _commissionRateService.GetAllGroupAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Lấy commission theo Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _commissionRateService.GetByIdAsync(id);

        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Lấy commission hiện tại 
    /// </summary>
    [HttpGet("current/{type}")]
    public async Task<IActionResult> GetCurrent([FromRoute] CommissionType type, [FromQuery] DateTime? onDate = null)
    {
        var result = await _commissionRateService.GetCurrentAsync(type, onDate);

        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Tạo mới commission
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommissionRateCreateDto dto)
    {
        var result = await _commissionRateService.CreateAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Cập nhật giá trị rate
    /// </summary>
    // [HttpPut("{id:guid}")]
    // public async Task<IActionResult> Update(Guid id, [FromBody] decimal newRateValue)
    // {
    //     await _commissionRateService.UpdateAsync(id, newRateValue);
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: true,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
    //     ));
    // }

    /// <summary>
    /// Xóa commission
    /// </summary>
    // [HttpDelete("{id:guid}")]
    // public async Task<IActionResult> Delete(Guid id)
    // {
    //     await _commissionRateService.DeleteAsync(id);
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: true,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
    //     ));
    // }
}
