using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    /// <summary>
    /// Lấy thông tin tour
    /// </summary>
    /// <returns>Thông tin tour</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(ResponseModel<List<TourResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTours([FromQuery] TourFilterModel filter)
    {
        try
        {
            var result = await _tourService.GetAllToursAsync(filter);
            return Ok(ResponseModel<List<TourResponseDto>>.OkResponseModel(
                data: result,
                message: "Tour details retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Tạo mới tour
    /// </summary>
    /// <param name="model">Thông tin tour cần tạo</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin tour đã tạo</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<TourResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTour([FromBody] CreateTourDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.CreateTourAsync(model);
            return Ok(ResponseModel<TourResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "tour")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Cập nhật thông tin tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="model">Thông tin tour cần cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin tour đã cập nhật</returns>
    [HttpPut("{tourId}")]
    [ProducesResponseType(typeof(ResponseModel<TourResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTour(Guid tourId, [FromBody] UpdateTourDto model, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.UpdateTourAsync(tourId, model);
            return Ok(ResponseModel<TourResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Xác nhận tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="model">Thông tin xác nhận</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin tour đã xác nhận</returns>
    // [HttpPut("{tourId}/confirm")]
    // [ProducesResponseType(typeof(ResponseModel<TourResponseDto>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> ConfirmTour(Guid tourId, [FromBody] ConfirmTourDto model, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _tourService.ConfirmTourAsync(tourId, model);
    //         return Ok(ResponseModel<TourResponseDto>.OkResponseModel(
    //             data: result,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour status")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    /// <summary>
    /// Xóa tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="model">Thông tin tour cần cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông tin tour đã cập nhật</returns>
    [HttpPatch("{tourId}")]
    public async Task<IActionResult> DeleteTour(Guid tourId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.DeleteTourAsync(tourId, cancellationToken);
            return Ok(ResponseModel<bool>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy chi tiết tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Chi tiết tour bao gồm địa điểm và lịch trình</returns>
    [HttpGet("{tourId}")]
    [ProducesResponseType(typeof(ResponseModel<TourDetailsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTourDetails(Guid tourId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.GetTourDetailsAsync(tourId);
            return Ok(ResponseModel<TourDetailsResponseDto>.OkResponseModel(
                data: result,
                message: "Tour details retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy chi tiết tour theo lịch trình
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="scheduleId">ID của lịch trình (nếu có)</param>
    /// <returns>Chi tiết tour bao gồm địa điểm và lịch trình</returns>
    [HttpGet("{tourId}/schedules/{scheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<TourDetailsResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTourDetails(Guid tourId, Guid? scheduleId = null)
    {
        try
        {
            var result = await _tourService.GetTourDetailsAsync(tourId, scheduleId);
            return Ok(ResponseModel<TourDetailsResponseDto>.OkResponseModel(
                data: result,
                message: "Tour details retrieved successfully."
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    #region Tour Locations

    /// <summary>
    /// Cập nhật hàng loạt địa điểm của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="dtos">Danh sách các địa điểm cần thêm hoặc cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách các địa điểm không bị xóa</returns>
    /// <remarks>
    /// - Nếu LocationId là null, địa điểm sẽ được thêm mới.
    /// - Nếu LocationId được cung cấp, địa điểm sẽ được cập nhật.
    /// - Các địa điểm không xuất hiện trong danh sách dtos sẽ được đánh dấu IsDeleted = true.
    /// </remarks>
    [HttpPut("bulk")]
    [ProducesResponseType(typeof(ResponseModel<List<TourPlanLocationResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateLocations(Guid tourId, [FromBody] List<UpdateTourPlanLocationDto> dtos, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.UpdateLocationsAsync(tourId, dtos);
            return Ok(ResponseModel<List<TourPlanLocationResponseDto>>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour locations")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy danh sách địa điểm của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách các địa điểm không bị xóa</returns>
    // [HttpGet("{tourId}/locations")]
    // [ProducesResponseType(typeof(ResponseModel<List<TourPlanLocationResponseDto>>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> GetLocations(Guid tourId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _tourService.GetLocationsAsync(tourId);
    //         return Ok(ResponseModel<List<TourPlanLocationResponseDto>>.OkResponseModel(
    //             data: result,
    //             message: "Tour locations retrieved successfully."
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    #endregion

    #region Tour Locations

    /// <summary>
    /// Tạo mới danh sách lịch trình cho tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="dtos">Danh sách lịch trình cần tạo</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách lịch trình đã tạo</returns>
    [HttpPost("{tourId}/schedules")]
    [ProducesResponseType(typeof(ResponseModel<List<TourScheduleResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSchedules(Guid tourId, [FromBody] List<CreateTourScheduleDto> dtos, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.CreateSchedulesAsync(tourId, dtos);
            return Ok(ResponseModel<List<TourScheduleResponseDto>>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "tour schedules")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy danh sách lịch trình của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Danh sách lịch trình</returns>
    // [HttpGet("{tourId}/schedules")]
    // [ProducesResponseType(typeof(ResponseModel<List<TourScheduleResponseDto>>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> GetSchedules(Guid tourId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var result = await _tourService.GetSchedulesAsync(tourId);
    //         return Ok(ResponseModel<List<TourScheduleResponseDto>>.OkResponseModel(
    //             data: result,
    //             message: "Tour schedules retrieved successfully."
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    /// <summary>
    /// Cập nhật lịch trình của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="scheduleId">ID của lịch trình</param>
    /// <param name="dto">Thông tin lịch trình cần cập nhật</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Lịch trình đã cập nhật</returns>
    [HttpPut("tour-schedule/{scheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<TourScheduleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSchedule(Guid tourId, Guid scheduleId, [FromBody] CreateTourScheduleDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _tourService.UpdateScheduleAsync(tourId, scheduleId, dto);
            return Ok(ResponseModel<TourScheduleResponseDto>.OkResponseModel(
                data: result,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour schedule")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }

    [HttpPost("tour-schedules/validate")]
    public async Task<IActionResult> ValidateScheduleAsync(Guid tourId, [FromBody] CreateTourScheduleDto dto, CancellationToken cancellationToken)
    {
        var result = await _tourService.ValidateAsync(tourId, dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour schedule")
        ));
    }

    /// <summary>
    /// Xóa lịch trình của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="scheduleId">ID của lịch trình</param>
    /// <param name="cancellationToken">Token để hủy thao tác</param>
    /// <returns>Thông báo xóa thành công</returns>
    [HttpDelete("{scheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSchedule(Guid tourId, Guid scheduleId, CancellationToken cancellationToken)
    {
        try
        {
            await _tourService.DeleteScheduleAsync(tourId, scheduleId);
            return Ok(ResponseModel<object>.OkResponseModel(
                data: null,
                message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour schedule")
            ));
        }
        catch (CustomException ex)
        {
            return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
        }
    }
    #endregion

    /// <summary>
    /// Gán tour guide để dẫn 1 tour
    /// </summary>
    /// <param name="tourScheduleId"></param>
    /// <param name="guideId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    // [HttpPost("{tourId}/guides")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> AddTourGuides(Guid tourScheduleId, Guid guideId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         await _tourService.AddTourGuideToScheduleAsync(tourScheduleId, guideId);
    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: null,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour schedule")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    /// <summary>
    /// Xóa 1 tour guide ra khỏi đoàn dẫn tour
    /// </summary>
    /// <param name="tourId"></param>
    /// <param name="guideId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    // [HttpDelete("{tourId}/guides")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> RemoveTourGuide(Guid tourId, Guid guideId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         await _tourService.RemoveTourGuideAsync(tourId, guideId);
    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: null,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour schedule")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    /// <summary>
    /// Thêm danh sách hình ảnh cho tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="createDtos">Danh sách media cần thêm</param>
    /// <returns>Danh sách media đã được thêm</returns>
    // [HttpPost("tour-media")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> AddTourMedias(Guid tourId, [FromBody] List<TourMediaCreateDto> createDtos)
    // {
    //     try
    //     {
    //         var result = await _tourService.AddTourMediasAsync(tourId, createDtos);
    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: result,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "tour media")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }

    /// <summary>
    /// Xóa một media của tour
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <param name="mediaId">ID của media cần xóa</param>
    /// <returns>Thông báo xóa thành công</returns>
    // [HttpDelete("tour-media/{mediaId:guid}")]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ResponseModel<object>), StatusCodes.Status500InternalServerError)]
    // public async Task<IActionResult> DeleteTourMedia(Guid tourId, Guid mediaId)
    // {
    //     try
    //     {
    //         var success = await _tourService.DeleteTourMediaAsync(mediaId);
    //         if (!success)
    //             throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa media.");

    //         return Ok(ResponseModel<object>.OkResponseModel(
    //             data: null,
    //             message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour media")
    //         ));
    //     }
    //     catch (CustomException ex)
    //     {
    //         return BadRequest(ResponseModel<object>.ErrorResponseModel(ex.StatusCode, ex.Message));
    //     }
    //     catch (Exception)
    //     {
    //         return StatusCode(500, ResponseModel<object>.ErrorResponseModel(500, $"An unexpected error occurred. {ex.Message}"));
    //     }
    // }
}