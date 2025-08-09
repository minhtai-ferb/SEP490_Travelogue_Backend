using Microsoft.AspNetCore.Mvc;
using Travelogue.Service.BusinessModels.BookingModels;
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

    [HttpGet("booking-statistics")]
    public async Task<IActionResult> GetBookingStatistics([FromQuery] BookingFilterDto filter)
    {
        var result = await _dashboardService.GetBookingStatisticsAsync(filter);
        return Ok(result);
    }
}