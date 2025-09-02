using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CraftVillageController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ICraftVillageService _craftVillageService;

    public CraftVillageController(ILocationService locationService, ICraftVillageService craftVillageService)
    {
        _locationService = locationService;
        _craftVillageService = craftVillageService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCraftVillageByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _locationService.GetCraftVillageByIdAsync(id, cancellationToken);
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: location,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetCraftVillageWorkshopDashboardAsync(Guid craftVillageId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var response = await _craftVillageService.GetCraftVillageWorkshopDashboardAsync(craftVillageId, fromDate, toDate, ct);
        return Ok(ResponseModel<CraftVillageWorkshopDashboardDto>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }
}