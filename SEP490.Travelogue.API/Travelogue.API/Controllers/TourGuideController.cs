using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    /// <summary>
    /// Lấy thông tin Tour Guide theo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetTourGuideByIdAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guide")
        ));
    }

    /// <summary>
    /// Lấy danh sách tất cả Tour Guides.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<IActionResult> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetAllTourGuidesAsync(cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guides")
        ));
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetTourGuidesByFilterAsync([FromQuery] TourGuideFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetTourGuidesByFilterAsync(filter, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guides")
        ));
    }


    // [HttpPost("assign")]
    // public async Task<IActionResult> AssignToTourGuideAsync([FromBody] List<string> emails, CancellationToken cancellationToken)
    // {
    //     var result = await _tourGuideService.AssignToTourGuideAsync(emails, cancellationToken);
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
    //     ));
    // }

    /// <summary>
    /// tour guide tự lấy các schedule của mình theo loại (all, booking, tour schedule)
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet("schedules")]
    public async Task<IActionResult> GetSchedules([FromQuery] TourGuideScheduleFilterDto filter)
    {
        var result = await _tourGuideService.GetSchedulesAsync(filter);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Lấy thông tin chi tiết lịch làm việc (schedule) của Tour Guide theo Id lịch làm việc.
    /// </summary>
    /// <param name="tourGuideSchedulesId">Id của lịch làm việc của Tour Guide</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ</param>
    /// <returns>Thông tin schedule kèm rejection request (nếu có)</returns>
    [HttpGet("schedules/{tourGuideSchedulesId:guid}")]
    public async Task<IActionResult> GetScheduleByIdAsync(Guid tourGuideSchedulesId, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.GetShedulesById(tourGuideSchedulesId, cancellationToken);

        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(
                ResponseMessages.GET_SUCCESS,
                "schedule by id with rejection requests"
            )
        ));
    }

    /// <summary>
    /// Tour guide tạo yêu cầu để cập nhật giá
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("booking-price-request")]
    public async Task<IActionResult> CreateBookingPriceRequestAsync(
    [FromBody] BookingPriceRequestCreateDto dto)
    {
        var result = await _tourGuideService.CreateBookingPriceRequestAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "booking price request")
        ));
    }

    /// <summary>
    /// moderator chấp nhận yêu cầu của tour guide
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    [HttpPut("booking-price-request/{requestId}/approve")]
    public async Task<IActionResult> ApproveBookingPriceRequestAsync(Guid requestId)
    {
        var result = await _tourGuideService.ApproveBookingPriceRequestAsync(requestId);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "booking price request")
        ));
    }

    /// <summary>
    /// moderator từ chối yêu cầu cập nhật giá
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("booking-price-request/{requestId}/reject")]
    public async Task<IActionResult> RejectBookingPriceRequestAsync(
        Guid requestId,
        [FromBody] RejectBookingPriceRequestDto dto)
    {
        var result = await _tourGuideService.RejectBookingPriceRequestAsync(requestId, dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "booking price request")
        ));
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateTourGuide([FromBody] TourGuideUpdateModel model, CancellationToken cancellationToken)
    {
        var updatedData = await _tourGuideService.UpdateTourGuideAsync(model, cancellationToken);
        return Ok(ResponseModel<TourGuideDataModel>.OkResponseModel(
            data: updatedData,
            message: ResponseMessages.UPDATE_SUCCESS
        ));
    }

    [HttpPost("certification")]
    public async Task<IActionResult> AddCertification([FromBody] CertificationDto dto, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.AddCertificationAsync(dto, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpDelete("{certificationId:guid}")]
    public async Task<IActionResult> SoftDeleteCertification(Guid certificationId, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.SoftDeleteCertificationAsync(certificationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetTourGuideDashboardAsync(Guid tourGuideId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var result = await _tourGuideService.GetTourGuideDashboardAsync(tourGuideId, fromDate, toDate, ct);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }
}