using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.HotelModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    /// <summary>
    /// Tạo mới hotel
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateHotel([FromBody] HotelCreateModel model)
    {
        await _hotelService.AddHotelAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "hotel")
        ));
    }

    /// <summary>
    /// Xóa hotel theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        await _hotelService.DeleteHotelAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "hotel")
        ));
    }

    /// <summary>
    /// Lấy tất cả hotel
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllHotels()
    {
        var hotels = await _hotelService.GetAllHotelsAsync(new CancellationToken());
        return Ok(ResponseModel<List<HotelDetailDataModel>>.OkResponseModel(
            data: hotels,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "hotel")
        ));
    }

    /// <summary>
    /// Lấy hotel theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotelById(Guid id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<HotelDetailDataModel>.OkResponseModel(
            data: hotel,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "hotel")
        ));
    }
    /// <summary>
    /// Cập nhật hotel
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelUpdateModel model)
    {
        await _hotelService.UpdateHotelAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "hotel")
        ));
    }

    /// <summary>
    /// Lấy danh sách hotel phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Trả về danh sách các hotel</returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedHotel(int pageNumber = 1, int pageSize = 10)
    {
        var hotels = await _hotelService.GetPagedHotelsAsync(pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: hotels.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "hotel"),
            totalCount: hotels.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách hotel phân trang theo tên
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedHotelWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var hotels = await _hotelService.GetPagedHotelsWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: hotels.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "hotel"),
            totalCount: hotels.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    // [HttpPost("upload-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    // {
    //     var result = await _hotelService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
    //     ));
    // }
}
