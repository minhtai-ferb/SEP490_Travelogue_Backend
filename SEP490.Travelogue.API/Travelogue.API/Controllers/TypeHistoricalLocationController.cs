using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TypeHistoricalLocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TypeHistoricalLocationController : ControllerBase
{
    private readonly ITypeHistoricalLocationService _typeHistoricalLocationService;

    public TypeHistoricalLocationController(ITypeHistoricalLocationService typeHistoricalLocationService)
    {
        _typeHistoricalLocationService = typeHistoricalLocationService;
    }

    /// <summary>
    /// Tạo mới typeHistoricalLocation
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTypeHistoricalLocation([FromBody] TypeHistoricalLocationCreateModel model)
    {
        await _typeHistoricalLocationService.AddTypeHistoricalLocationAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Xóa typeHistoricalLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeHistoricalLocation(Guid id)
    {
        await _typeHistoricalLocationService.DeleteTypeHistoricalLocationAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy tất cả typeHistoricalLocation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTypeHistoricalLocations()
    {
        var typeHistoricalLocations = await _typeHistoricalLocationService.GetAllTypeHistoricalLocationsAsync(new CancellationToken());
        return Ok(ResponseModel<List<TypeHistoricalLocationDataModel>>.OkResponseModel(
            data: typeHistoricalLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy typeHistoricalLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeHistoricalLocationById(Guid id)
    {
        var typeHistoricalLocation = await _typeHistoricalLocationService.GetTypeHistoricalLocationByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TypeHistoricalLocationDataModel>.OkResponseModel(
            data: typeHistoricalLocation,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience")
        ));
    }
    /// <summary>
    /// Cập nhật typeHistoricalLocation
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeHistoricalLocation(Guid id, [FromBody] TypeHistoricalLocationUpdateModel model)
    {
        await _typeHistoricalLocationService.UpdateTypeHistoricalLocationAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "type experience")
        ));
    }

    /// <summary>
    /// Lấy danh sách typeHistoricalLocation phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các typeHistoricalLocation</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTypeHistoricalLocationWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var typeHistoricalLocations = await _typeHistoricalLocationService.GetPagedTypeHistoricalLocationsWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: typeHistoricalLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience"),
            totalCount: typeHistoricalLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedTypeHistoricalLocation(int pageNumber = 1, int pageSize = 10)
    //{
    //    var typeHistoricalLocations = await _typeHistoricalLocationService.GetPagedTypeHistoricalLocationsAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: typeHistoricalLocations.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type experience"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
