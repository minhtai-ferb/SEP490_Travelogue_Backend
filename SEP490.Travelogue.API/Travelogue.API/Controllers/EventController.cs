using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.EventModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Tạo mới event
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateModel model)
    {
        var result = await _eventService.AddEventAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "event")
        ));
    }

    /// <summary>
    /// Xóa event theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        await _eventService.DeleteEventAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "event")
        ));
    }

    /// <summary>
    /// Lấy tất cả event
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _eventService.GetAllEventsAsync(new CancellationToken());
        return Ok(ResponseModel<List<EventDataModel>>.OkResponseModel(
            data: events,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event")
        ));
    }

    /// <summary>
    /// Lấy event theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        var eventResult = await _eventService.GetEventByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<EventDataModel>.OkResponseModel(
            data: eventResult,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event")
        ));
    }
    /// <summary>
    /// Cập nhật event
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventUpdateModel model)
    {
        await _eventService.UpdateEventAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "event")
        ));
    }

    /// <summary>
    /// Lấy danh sách event phân trang theo tiêu đề, loại event, địa điểm, quận, tháng, năm
    /// </summary>
    /// <param name="title">Tiêu đề event</param>
    /// <param name="typeId">Thể loại event</param>
    /// <param name="locationId">Địa điểm diễn ra sự kiện</param>
    /// <param name="districtId"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedEventWithFilter(
        string? title = null,
        Guid? typeId = null,
        Guid? locationId = null,
        Guid? districtId = null,
        int? month = null,
        int? year = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        month ??= 0;
        year ??= 0;

        var events = await _eventService.GetPagedEventsWithSearchAsync(
            title, typeId, locationId, districtId, month.Value, year.Value, pageNumber, pageSize, new CancellationToken()
        );

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: events.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event"),
            totalCount: events.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy event nổi bật
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-highlighted-event")]
    public async Task<IActionResult> GetEventsGrouped()
    {
        var events = await _eventService.GetHighlightedEventsObjectAsync(new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: events,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event")
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    {
        var result = await _eventService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpDelete("delete-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    {
        var result = await _eventService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }

    [HttpGet("admin-get")]
    public async Task<IActionResult> GetAllEventsAdmin()
    {
        var events = await _eventService.GetAllEventAdminAsync();
        return Ok(ResponseModel<List<EventDataModel>>.OkResponseModel(
            data: events,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event")
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedEvent(int pageNumber = 1, int pageSize = 10)
    //{
    //    var events = await _eventService.GetPagedEventsAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: events.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}

    //[HttpGet("search-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedEventWithSearch(int pageNumber = 1, int pageSize = 10, string title = "")
    //{
    //    var events = await _eventService.GetPagedEventsWithSearchAsync(pageNumber, pageSize, title, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: events.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}

    //[HttpGet("filter-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedEventWithFilter(int pageNumber = 1, int pageSize = 10,
    //    string title = "", Guid typeId = new Guid(), Guid locationId = new Guid(), int month = 0, int year = 0)
    //{
    //    if (month == 0) month = DateTime.Now.Month;
    //    if (year == 0) year = DateTime.Now.Year;
    //    var events = await _eventService.GetPagedEventsWithSearchAsync(pageNumber, pageSize, title, typeId, locationId, month, year, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: events.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "event"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
