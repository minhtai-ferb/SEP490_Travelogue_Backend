using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.WorkshopModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkshopController : ControllerBase
{
    private readonly IWorkshopService _workshopService;
    public WorkshopController(IWorkshopService workshopService)
    {
        _workshopService = workshopService;
    }

    /// <summary>
    /// Tạo mới workshop
    /// </summary>
    /// <param name="model">Thông tin workshop cần tạo</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin workshop đã tạo</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWorkshop([FromBody] CreateWorkshopDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.CreateWorkshopAsync(model);
            return Ok(ResponseModel<WorkshopResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "workshop")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Cập nhật thông tin workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="model">Thông tin workshop cần cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin workshop đã cập nhật</returns>
    [HttpPut("{workshopId}")]
    [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateWorkshop(Guid workshopId, [FromBody] UpdateWorkshopDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.UpdateWorkshopAsync(workshopId, model);
            return Ok(ResponseModel<WorkshopResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Xác nhận workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="model">Thông tin xác nhận</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin workshop đã xác nhận</returns>
    [HttpPut("{workshopId}/confirm")]
    [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmWorkshop(Guid workshopId, [FromBody] ConfirmWorkshopDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.ConfirmWorkshopAsync(workshopId, model);
            return Ok(ResponseModel<WorkshopResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop status")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Lấy chi tiết workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Chi tiết workshop bao gồm hoạt động và lịch trình</returns>
    [HttpGet("{workshopId}")]
    [ProducesResponseType(typeof(ResponseModel<WorkshopDetailsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWorkshopDetails(Guid workshopId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.GetWorkshopDetailsAsync(workshopId);
            return Ok(ResponseModel<WorkshopDetailsResponseDto>.OkResponseModel(
                data: result,
                message: "Workshop details retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Cập nhật hàng loạt hoạt động của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="dtos">Danh sách các hoạt động cần thêm hoặc cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách các hoạt động không bị xóa</returns>
    /// <remarks>
    /// - Nếu ActivityId là null, hoạt động sẽ được thêm mới.
    /// - Nếu ActivityId được cung cấp, hoạt động sẽ được cập nhật.
    /// - Các hoạt động không xuất hiện trong danh sách dtos sẽ được đánh dấu IsDeleted = true.
    /// </remarks>
    [HttpPut("bulk")]
    [ProducesResponseType(typeof(ResponseModel<List<ActivityResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateActivities(Guid workshopId, [FromBody] List<UpdateActivityRequestDto> dtos, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.UpdateActivitiesAsync(workshopId, dtos);
            return Ok(ResponseModel<List<ActivityResponseDto>>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop activities")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Lấy danh sách hoạt động của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách các hoạt động không bị xóa</returns>
    [HttpGet("{workshopId}/activities")]
    [ProducesResponseType(typeof(ResponseModel<List<ActivityResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActivities(Guid workshopId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.GetActivitiesAsync(workshopId);
            return Ok(ResponseModel<List<ActivityResponseDto>>.OkResponseModel(
                data: result,
                message: "Workshop activities retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Tạo mới danh sách lịch trình cho workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="dtos">Danh sách lịch trình cần tạo</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách lịch trình đã tạo</returns>
    [HttpPost("{workshopId}/schedules")]
    [ProducesResponseType(typeof(ResponseModel<List<ScheduleResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSchedules(Guid workshopId, [FromBody] List<CreateScheduleDto> dtos, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.CreateSchedulesAsync(workshopId, dtos);
            return Ok(ResponseModel<List<ScheduleResponseDto>>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "workshop schedules")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Lấy danh sách lịch trình của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách lịch trình</returns>
    [HttpGet("{workshopId}/schedules")]
    [ProducesResponseType(typeof(ResponseModel<List<ScheduleResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSchedules(Guid workshopId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.GetSchedulesAsync(workshopId);
            return Ok(ResponseModel<List<ScheduleResponseDto>>.OkResponseModel(
                data: result,
                message: "Workshop schedules retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Cập nhật lịch trình của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="scheduleId">ID của lịch trình</param>
    /// <param name="dto">Thông tin lịch trình cần cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Lịch trình đã cập nhật</returns>
    [HttpPut("{scheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<ScheduleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSchedule(Guid workshopId, Guid scheduleId, [FromBody] CreateScheduleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.UpdateScheduleAsync(workshopId, scheduleId, dto);
            return Ok(ResponseModel<ScheduleResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop schedule")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Xóa lịch trình của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="scheduleId">ID của lịch trình</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông báo xóa thành công</returns>
    [HttpDelete("{scheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSchedule(Guid workshopId, Guid scheduleId, CancellationToken cancellationToken)
    {
        try
        {
            await _workshopService.DeleteScheduleAsync(workshopId, scheduleId);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: null,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "workshop schedule")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
        }
    }
}