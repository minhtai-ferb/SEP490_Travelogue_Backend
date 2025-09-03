using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.BusinessModels.WorkshopModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkshopController : ControllerBase
{
    private readonly IWorkshopService _workshopService;
    private readonly IBookingService _bookingService;
    public WorkshopController(IWorkshopService workshopService, IBookingService bookingService)
    {
        _workshopService = workshopService;
        _bookingService = bookingService;
    }

    /// <summary>
    /// user filter theo tên các workshop
    /// </summary>
    /// <param name="name"></param>
    /// <param name="craftVillageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<IActionResult> GetFilteredWorkshopsAsync([FromQuery] string? name, [FromQuery] Guid? craftVillageId, CancellationToken cancellationToken)
    {
        var result = await _workshopService.GetWorkshopsAsync(craftVillageId, name, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "workshops")
        ));
    }

    [HttpGet("{workshopId}")]
    public async Task<IActionResult> GetWorkshopByIdAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        var result = await _workshopService.GetWorkshopByIdAsync(workshopId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "workshops")
        ));
    }

    /// <summary>
    /// moderator filter các workshop theo các điều kiện kèm theo status (đã duyệt hay chưa)
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("moderator-filter")]
    public async Task<IActionResult> ModeratorGetFilteredWorkshopsAsync([FromQuery] FilterWorkshop filter, CancellationToken cancellationToken)
    {
        var result = await _workshopService.ModeratorGetFilteredWorkshopsAsync(filter, cancellationToken);
        return Ok(ResponseModel<List<WorkshopResponseDtoOLD>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "workshops")
        ));
    }

    /// <summary>
    /// Tạo mới workshop
    /// </summary>
    /// <param name="model">Thông tin workshop cần tạo</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin workshop đã tạo</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDtoOLD>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWorkshop([FromBody] CreateWorkshopDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.CreateWorkshopAsync(model);
            return Ok(ResponseModel<WorkshopResponseDtoOLD>.OkResponseModel(
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
    [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDtoOLD>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateWorkshop(Guid workshopId, [FromBody] UpdateWorkshopDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workshopService.UpdateWorkshopAsync(workshopId, model);
            return Ok(ResponseModel<WorkshopResponseDtoOLD>.OkResponseModel(
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

    // [HttpPut("{workshopId}/confirm")]
    // [ProducesResponseType(typeof(ResponseModel<WorkshopResponseDto>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> ConfirmWorkshop(Guid workshopId, [FromBody] ConfirmWorkshopDto model, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _workshopService.ConfirmWorkshopAsync(workshopId, model);
    //         return Ok(ResponseModel<WorkshopResponseDto>.OkResponseModel(
    //             data: result,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop status")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //     }
    // }

    /// <summary>
    /// gửi workshop cho moderator để duyệt
    /// </summary>
    /// <param name="workshopId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("submit/{workshopId}")]
    public async Task<IActionResult> SubmitWorkshopForReviewAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        var result = await _workshopService.SubmitWorkshopForReviewAsync(workshopId, cancellationToken);
        return Ok(ResponseModel<WorkshopResponseDtoOLD>.OkResponseModel(
            data: result,
            message: "Workshop submitted for review successfully."
        ));
    }

    [HttpPost("{ticketTypeId:guid}/request")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<WorkshopTicketPriceUpdateDto>))]
    public async Task<IActionResult> RequestPriceChange(Guid ticketTypeId, [FromBody] RequestPriceChangeDto dto, CancellationToken ct)
    {
        var res = await _workshopService.RequestTicketTypePriceChangeAsync(ticketTypeId, dto.NewPrice, dto.Reason, ct);
        return Ok(ResponseModel<WorkshopTicketPriceUpdateDto>.OkResponseModel(res, ResponseMessages.SUCCESS));
    }

    [HttpPost("{requestId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid requestId, [FromBody] ModeratorDecisionDto dto, CancellationToken ct)
    {
        await _workshopService.ApproveTicketTypePriceChangeAsync(requestId, dto.Note, ct);
        return Ok(ResponseModel<string>.OkResponseModel("Đã duyệt yêu cầu đổi giá.", ResponseMessages.SUCCESS));
    }

    [HttpPost("{requestId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid requestId, [FromBody] ModeratorDecisionDto dto, CancellationToken ct)
    {
        await _workshopService.RejectTicketTypePriceChangeAsync(requestId, dto.Note ?? "Không có lý do", ct);
        return Ok(ResponseModel<string>.OkResponseModel("Đã từ chối yêu cầu đổi giá.", ResponseMessages.SUCCESS));
    }

    [HttpGet("price-requests/moderator")]
    public async Task<IActionResult> GetForModerator(
        [FromQuery] Guid? craftVillageId,
        [FromQuery] Guid? workshopId,
        [FromQuery] Guid? ticketTypeId,
        [FromQuery] PriceUpdateStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken ct = default)
    {
        var data = await _workshopService.GetRequestsForModeratorAsync(
            craftVillageId, workshopId, ticketTypeId, status, fromDate, toDate, ct);

        return Ok(ResponseModel<List<WorkshopTicketPriceUpdateListItemDto>>
            .OkResponseModel(data, ResponseMessages.SUCCESS));
    }

    [HttpGet("price-requests/mine")]
    public async Task<IActionResult> GetMine(
        [FromQuery] PriceUpdateStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken ct = default)
    {
        var data = await _workshopService.GetMyRequestsAsync(status, fromDate, toDate, ct);

        return Ok(ResponseModel<List<WorkshopTicketPriceUpdateListItemDto>>
            .OkResponseModel(data, ResponseMessages.SUCCESS));
    }

    [HttpGet("{workshopId}/bookings")]
    public async Task<IActionResult> GetBookingsByWorkshopAsync(Guid workshopId, [FromQuery] BookingFilterDto filter, CancellationToken cancellationToken = default)
    {
        var result = await _bookingService.GetBookingsByWorkshopAsync(workshopId, filter, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "booking")
        ));
    }

    ///// <summary>
    ///// Lấy chi tiết workshop
    ///// </summary>
    ///// <param name="workshopId">ID của workshop</param>
    ///// <param name="scheduleId">ID của schedule</param>
    ///// <param name="cancellationToken">Token để hủy thao tác</param>
    ///// <returns>Chi tiết workshop bao gồm hoạt động và lịch trình</returns>
    //[HttpGet("{workshopId}")]
    //[ProducesResponseType(typeof(ResponseModel<WorkshopDetailsResponseDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> GetWorkshopDetails(Guid workshopId, Guid? scheduleId, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _workshopService.GetWorkshopDetailsAsync(workshopId, scheduleId);
    //        return Ok(ResponseModel<WorkshopDetailsResponseDto>.OkResponseModel(
    //            data: result,
    //            message: "Workshop details retrieved successfully."
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    ///// <summary>
    ///// làng nghề xóa workshop
    ///// </summary>
    ///// <param name="workshopId"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns></returns>
    //[HttpPatch("{workshopId}")]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> DeleteSchedule(Guid workshopId, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        await _workshopService.DeleteWorkshopAsync(workshopId);
    //        return Ok(ResponseModel<object>.OkResponseModel(
    //            data: null,
    //            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "workshop")
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    ///// <summary>
    ///// Cập nhật hàng loạt hoạt động của workshop
    ///// </summary>
    ///// <param name="workshopId">ID của workshop</param>
    ///// <param name="dtos">Danh sách các hoạt động cần thêm hoặc cập nhật</param>
    ///// <param name="cancellationToken">Token để hủy thao tác</param>
    ///// <returns>Danh sách các hoạt động không bị xóa</returns>
    ///// <remarks>
    ///// - Nếu ActivityId là null, hoạt động sẽ được thêm mới.
    ///// - Nếu ActivityId được cung cấp, hoạt động sẽ được cập nhật.
    ///// - Các hoạt động không xuất hiện trong danh sách dtos sẽ được đánh dấu IsDeleted = true.
    ///// </remarks>
    //[HttpPut("bulk")]
    //[ProducesResponseType(typeof(ResponseModel<List<ActivityResponseDto>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> UpdateActivities(Guid workshopId, [FromBody] List<UpdateActivityRequestDto> dtos, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _workshopService.UpdateActivitiesAsync(workshopId, dtos);
    //        return Ok(ResponseModel<List<ActivityResponseDto>>.OkResponseModel(
    //            data: result,
    //            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop activities")
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    /// <summary>
    /// Lấy danh sách hoạt động của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách các hoạt động không bị xóa</returns>
    // [HttpGet("{workshopId}/activities")]
    // [ProducesResponseType(typeof(ResponseModel<List<ActivityResponseDto>>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> GetActivities(Guid workshopId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _workshopService.GetActivitiesAsync(workshopId);
    //         return Ok(ResponseModel<List<ActivityResponseDto>>.OkResponseModel(
    //             data: result,
    //             message: "Workshop activities retrieved successfully."
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //     }
    // }

    ///// <summary>
    ///// Tạo mới danh sách lịch trình cho workshop
    ///// </summary>
    ///// <param name="workshopId">ID của workshop</param>
    ///// <param name="dtos">Danh sách lịch trình cần tạo</param>
    ///// <param name="cancellationToken">Token để hủy thao tác</param>
    ///// <returns>Danh sách lịch trình đã tạo</returns>
    //[HttpPost("{workshopId}/schedules")]
    //[ProducesResponseType(typeof(ResponseModel<List<ScheduleResponseDto>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> CreateSchedules(Guid workshopId, [FromBody] List<CreateScheduleDto> dtos, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _workshopService.CreateSchedulesAsync(workshopId, dtos);
    //        return Ok(ResponseModel<List<ScheduleResponseDto>>.OkResponseModel(
    //            data: result,
    //            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "workshop schedules")
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    /// <summary>
    /// Lấy danh sách lịch trình của workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách lịch trình</returns>
    // [HttpGet("{workshopId}/schedules")]
    // [ProducesResponseType(typeof(ResponseModel<List<ScheduleResponseDto>>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> GetSchedules(Guid workshopId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _workshopService.GetSchedulesAsync(workshopId);
    //         return Ok(ResponseModel<List<ScheduleResponseDto>>.OkResponseModel(
    //             data: result,
    //             message: "Workshop schedules retrieved successfully."
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //     }
    // }

    ///// <summary>
    ///// Cập nhật lịch trình của workshop
    ///// </summary>
    ///// <param name="workshopId">ID của workshop</param>
    ///// <param name="scheduleId">ID của lịch trình</param>
    ///// <param name="dto">Thông tin lịch trình cần cập nhật</param>
    ///// <param name="cancellationToken">Token để hủy thao tác</param>
    ///// <returns>Lịch trình đã cập nhật</returns>
    //[HttpPut("update-schedule/{scheduleId}")]
    //[ProducesResponseType(typeof(ResponseModel<ScheduleResponseDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> UpdateSchedule(Guid workshopId, Guid scheduleId, [FromBody] CreateScheduleDto dto, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var result = await _workshopService.UpdateScheduleAsync(workshopId, scheduleId, dto);
    //        return Ok(ResponseModel<ScheduleResponseDto>.OkResponseModel(
    //            data: result,
    //            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "workshop schedule")
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    ///// <summary>
    ///// Xóa lịch trình của workshop
    ///// </summary>
    ///// <param name="workshopId">ID của workshop</param>
    ///// <param name="scheduleId">ID của lịch trình</param>
    ///// <param name="cancellationToken">Token để hủy thao tác</param>
    ///// <returns>Thông báo xóa thành công</returns>
    //[HttpDelete("{scheduleId}")]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    //public async Task<IActionResult> DeleteSchedule(Guid workshopId, Guid scheduleId, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        await _workshopService.DeleteScheduleAsync(workshopId, scheduleId);
    //        return Ok(ResponseModel<object>.OkResponseModel(
    //            data: null,
    //            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "workshop schedule")
    //        ));
    //    }
    //    catch (CustomException ex)
    //    {
    //        return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //    }
    //}

    /// <summary>
    /// Thêm danh sách hình ảnh cho workshop
    /// </summary>
    /// <param name="workshopId">ID của workshop</param>
    /// <param name="createDtos">Danh sách ảnh cần thêm</param>
    /// <returns>Danh sách media đã được thêm</returns>
    // [HttpPost("workshop-media")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> AddWorkshopMedias(Guid workshopId, [FromBody] List<WorkshopMediaCreateDto> createDtos)
    // {
    //     try
    //     {
    //         var result = await _workshopService.AddWorkshopMediasAsync(workshopId, createDtos);
    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: result,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "media")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //     }
    // }

    /// <summary>
    /// Xóa một media của workshop
    /// </summary>
    /// <param name="mediaId">ID của media cần xóa</param>
    /// <returns>Thông báo xóa thành công</returns>
    // [HttpDelete("workshop-media/{mediaId:guid}")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> DeleteWorkshopMedia(Guid mediaId)
    // {
    //     try
    //     {
    //         var success = await _workshopService.DeleteWorkshopMediaAsync(mediaId);
    //         if (!success)
    //             throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa media.");

    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: null,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, "An unexpected error occurred."));
    //     }
    // }
}