using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoricalLocationController : ControllerBase
{
    private readonly IHistoricalLocationService _historicalLocationService;

    public HistoricalLocationController(IHistoricalLocationService historicalLocationService)
    {
        _historicalLocationService = historicalLocationService;
    }

    /// <summary>
    /// Lấy tất cả historicalLocation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllHistoricalLocations()
    {
        var historicalLocations = await _historicalLocationService.GetAllHistoricalLocationsAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: historicalLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy historicalLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHistoricalLocationById(Guid id)
    {
        var historicalLocationResult = await _historicalLocationService.GetHistoricalLocationByLocationIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: historicalLocationResult,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy danh sách historicalLocation phân trang theo tiêu đề, loại historicalLocation, địa điểm, quận, tháng, năm
    /// </summary>
    /// <param name="name">Tiêu đề historicalLocation</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedHistoricalLocationWithFilter(
        string? name = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var historicalLocations = await _historicalLocationService.GetPagedHistoricalLocationsWithSearchAsync(
            name, pageNumber, pageSize, new CancellationToken()
        );

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: historicalLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation"),
            totalCount: historicalLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
