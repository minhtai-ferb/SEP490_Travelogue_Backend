using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.TourTypeModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TourTypeController : ControllerBase
{
    private readonly ITourTypeService _tourTypeService;

    public TourTypeController(ITourTypeService tourTypeService)
    {
        _tourTypeService = tourTypeService;
    }

    /// <summary>
    /// Tạo mới tourType
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateTourType([FromBody] TourTypeCreateModel model)
    {
        await _tourTypeService.AddTourTypeAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Xóa tourType theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTourType(Guid id)
    {
        await _tourTypeService.DeleteTourTypeAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy tất cả tourType
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTourTypes()
    {
        var tourTypes = await _tourTypeService.GetAllTourTypesAsync(new CancellationToken());
        return Ok(ResponseModel<List<TourTypeDataModel>>.OkResponseModel(
            data: tourTypes,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy tourType theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTourTypeById(Guid id)
    {
        var tourType = await _tourTypeService.GetTourTypeByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<TourTypeDataModel>.OkResponseModel(
            data: tourType,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity")
        ));
    }
    /// <summary>
    /// Cập nhật tourType
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTourType(Guid id, [FromBody] TourTypeUpdateModel model)
    {
        await _tourTypeService.UpdateTourTypeAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "type activity")
        ));
    }

    /// <summary>
    /// Lấy danh sách tourType phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Trả về danh sách các tourType</returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTourType(int pageNumber = 1, int pageSize = 10)
    {
        var tourTypes = await _tourTypeService.GetPagedTourTypesAsync(pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: tourTypes.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity"),
            totalCount: tourTypes.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách tourType phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các tourType</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedTourTypeWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var tourTypes = await _tourTypeService.GetPagedTourTypesWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: tourTypes.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "type activity"),
            totalCount: tourTypes.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
