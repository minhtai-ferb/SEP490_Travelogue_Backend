using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TypeLocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TypeLocationController : ControllerBase
{
    private readonly ITypeLocationService _typeLocationService;

    public TypeLocationController(ITypeLocationService typeLocationService)
    {
        _typeLocationService = typeLocationService;
    }

    /// <summary>
    /// Tạo mới typeLocation
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTypeLocation([FromBody] TypeLocationCreateModel model)
    {
        await _typeLocationService.AddTypeLocationAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "type location")
        ));
    }

    /// <summary>
    /// Xóa typeLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeLocation(Guid id)
    {
        await _typeLocationService.DeleteTypeLocationAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "type location")
        ));
    }

    /// <summary>
    /// Lấy tất cả typeLocation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTypeLocations()
    {
        var typeLocations = await _typeLocationService.GetAllTypeLocationsAsync(new CancellationToken());
        return Ok(ResponseModel<List<TypeLocationDataModel>>.OkResponseModel(
            data: typeLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type location")
        ));
    }

    /// <summary>
    /// Lấy typeLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeLocationById(Guid id)
    {
        var typeLocation = await _typeLocationService.GetTypeLocationByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TypeLocationDataModel>.OkResponseModel(
            data: typeLocation,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type location")
        ));
    }

    /// <summary>
    /// Cập nhật typeLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeLocation(Guid id, [FromBody] TypeLocationUpdateModel model)
    {
        await _typeLocationService.UpdateTypeLocationAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "type location")
        ));
    }

    /// <summary>
    /// Lấy danh sách typeLocation phân trang
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTypeLocation(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var typeLocations = await _typeLocationService.GetPagedTypeLocationsAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: typeLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type location"),
            totalCount: typeLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
