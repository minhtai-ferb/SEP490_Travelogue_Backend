using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TypeEventModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TypeEventController : ControllerBase
{
    private readonly ITypeEventService _typeEventService;

    public TypeEventController(ITypeEventService typeEventService)
    {
        _typeEventService = typeEventService;
    }

    /// <summary>
    /// Tạo mới typeEvent
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTypeEvent([FromBody] TypeEventCreateModel model)
    {
        await _typeEventService.AddTypeEventAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Xóa typeEvent theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeEvent(Guid id)
    {
        await _typeEventService.DeleteTypeEventAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy tất cả typeEvent
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTypeEvents()
    {
        var typeEvents = await _typeEventService.GetAllTypeEventsAsync(new CancellationToken());
        return Ok(ResponseModel<List<TypeEventDataModel>>.OkResponseModel(
            data: typeEvents,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy typeEvent theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeEventById(Guid id)
    {
        var typeEvent = await _typeEventService.GetTypeEventByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TypeEventDataModel>.OkResponseModel(
            data: typeEvent,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity")
        ));
    }
    /// <summary>
    /// Cập nhật typeEvent
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeEvent(Guid id, [FromBody] TypeEventUpdateModel model)
    {
        await _typeEventService.UpdateTypeEventAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy danh sách typeEvent phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Trả về danh sách các typeEvent</returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTypeEvent(int pageNumber = 1, int pageSize = 10)
    {
        var typeEvents = await _typeEventService.GetPagedTypeEventsAsync(pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: typeEvents.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity"),
            totalCount: typeEvents.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách typeEvent phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các typeEvent</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTypeEventWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var typeEvents = await _typeEventService.GetPagedTypeEventsWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: typeEvents.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity"),
            totalCount: typeEvents.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
