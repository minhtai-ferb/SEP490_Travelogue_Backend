using Microsoft.AspNetCore.Mvc;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RejectionRequestController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public RejectionRequestController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRejectionRequest([FromBody] RejectionRequestCreateDto dto)
    {
        var response = await _tourGuideService.CreateRejectionRequestAsync(dto);
        return Ok(response);
    }

    [HttpPut("{requestId}/approve")]
    public async Task<IActionResult> ApproveRejectionRequest(Guid requestId)
    {
        var response = await _tourGuideService.ApproveRejectionRequestAsync(requestId);
        return Ok(response);
    }

    [HttpPut("{requestId}/reject")]
    public async Task<IActionResult> RejectRejectionRequest(Guid requestId, [FromBody] RejectRejectionRequestDto dto)
    {
        var response = await _tourGuideService.RejectRejectionRequestAsync(requestId, dto);
        return Ok(response);
    }
}
