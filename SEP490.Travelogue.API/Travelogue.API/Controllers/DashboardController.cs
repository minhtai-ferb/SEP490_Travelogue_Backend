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
    public async Task<IActionResult> GetTourStatistics(int month, int year, int topCount, BookingStatus? status)
    {
        var result = await _dashboardService.GetTopToursByMonthAsync(month, year, topCount, status);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("system-revenue-statistics")]
    public async Task<IActionResult> GetSystemRevenueStatisticsAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _dashboardService.GetSystemRevenueStatisticsAsync(fromDate, toDate);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("admin-revenue-statistics")]
    public async Task<IActionResult> GetAdminRevenueStatisticsAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _dashboardService.GetAdminRevenueStatisticsAsync(fromDate, toDate);
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

    [HttpGet("tours-statistic/{tourId}")]
    public async Task<IActionResult> GetTourStatistics(Guid tourId)
    {
        var result = await _dashboardService.GetTourBookingStatisticAsync(tourId);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("tour-schedules-statistic/{tourScheduleId}")]
    public async Task<IActionResult> GetTourScheduleStatistics(Guid tourScheduleId)
    {
        var result = await _dashboardService.GetTourScheduleBookingStatisticAsync(tourScheduleId);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("tour-guides-statistic/{tourGuideId}")]
    public async Task<IActionResult> GetTourGuideStatistics(Guid tourGuideId)
    {
        var result = await _dashboardService.GetTourGuideBookingStatisticAsync(tourGuideId);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("workshops-statistic/{workshopId}")]
    public async Task<IActionResult> GetWorkshopStatistics(Guid workshopId)
    {
        var result = await _dashboardService.GetWorkshopBookingStatisticAsync(workshopId);
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("workshop-schedules-statistic/{workshopScheduleId}")]
    public async Task<IActionResult> GetWorkshopScheduleStatistics(Guid workshopScheduleId)
    {
        var result = await _dashboardService.GetWorkshopScheduleBookingStatisticAsync(workshopScheduleId);
        return Ok(PagedResponseModel<object>.OkResponseModel(
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