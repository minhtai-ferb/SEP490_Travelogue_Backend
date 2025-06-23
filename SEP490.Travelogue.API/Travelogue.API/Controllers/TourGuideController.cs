using Microsoft.AspNetCore.Mvc;
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
        var tourGuide = await _tourGuideService.GetTourGuideByIdAsync(id, cancellationToken);
        if (tourGuide == null)
        {
            return NotFound();
        }
        return Ok(tourGuide);
    }

    /// <summary>
    /// Lấy danh sách tất cả Tour Guides.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        var tourGuides = await _tourGuideService.GetAllTourGuidesAsync(cancellationToken);
        return Ok(tourGuides);
    }

    /// <summary>
    /// Cấp quyền Tour Guide cho người dùng dựa trên email.
    /// </summary>
    /// <param name="emails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignToTourGuideAsync([FromBody] List<string> emails, CancellationToken cancellationToken)
    {
        var result = await _tourGuideService.AssignToTourGuideAsync(emails, cancellationToken);
        if (result == null)
        {
            return NotFound("No users found with the provided emails.");
        }
        return Ok(result);
    }
}