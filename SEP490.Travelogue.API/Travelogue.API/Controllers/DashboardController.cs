using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // [HttpGet("booking-statistics")]
    // public async Task<IActionResult> GetBookingStatistics([FromQuery] BookingFilterDto filter)
    // {
    //     var result = await _dashboardService.GetBookingStatisticsAsync(filter);
    //     return Ok(result);
    // }

    [HttpGet("tour-statistics")]
    public async Task<IActionResult> GetTourStatistics(int month, int year, int topCount, BookingStatus status)
    {
        var result = await _dashboardService.GetTopToursByMonthAsync(month, year, topCount, status);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("revenue-statistics")]
    public async Task<IActionResult> GetRevenueStatisticsAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _dashboardService.GetRevenueStatisticsAsync(fromDate, toDate);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("booking-statistics")]
    public async Task<IActionResult> GetBookingStatisticsAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _dashboardService.GetBookingStatisticsAsync(fromDate, toDate);
        return Ok(ResponseModel<object>.OkResponseModel(
             data: result,
             message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
         ));
    }

    [HttpGet("tours/{tourId}")]
    public async Task<IActionResult> GetTourBookings(Guid tourId, BookingStatus? status, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _dashboardService.GetTourBookingsAsync(tourId, status, pageNumber, pageSize);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpGet("tours/{tourScheduleId}/paged")]
    public async Task<IActionResult> GetTourScheduleBookingsAsync(Guid tourScheduleId, BookingStatus? status, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _dashboardService.GetTourScheduleBookingsAsync(tourScheduleId, status, pageNumber, pageSize);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    // [HttpGet("trip-plans/{tripPlanId}")]
    // public async Task<IActionResult> GetTripPlanBookings(Guid tripPlanId)
    // {
    //     var result = await _dashboardService.GetTripPlanBookingsAsync(tripPlanId);
    //     return Ok(result);
    // }

    [HttpGet("tour-guides/{tourGuideId}")]
    public async Task<IActionResult> GetTourGuideBookings(Guid tourGuideId, BookingStatus? status, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _dashboardService.GetTourGuideBookingsAsync(tourGuideId, status, pageNumber, pageSize);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}