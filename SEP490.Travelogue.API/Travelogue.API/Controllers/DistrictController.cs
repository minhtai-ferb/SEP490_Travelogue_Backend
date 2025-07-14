using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.DistrictModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictController(IDistrictService DistrictService)
    {
        _districtService = DistrictService;
    }

    /// <summary>
    /// Xóa District theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDistrict(Guid id)
    {
        await _districtService.DeleteDistrictAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "district")
        ));
    }

    /// <summary>
    /// Lấy tất cả District
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllDistricts()
    {
        var districts = await _districtService.GetAllDistrictsAsync(new CancellationToken());
        return Ok(ResponseModel<List<DistrictDataModel>>.OkResponseModel(
            data: districts,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "district")
        ));
    }

    /// <summary>
    /// Lấy District theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDistrictById(Guid id)
    {
        var district = await _districtService.GetDistrictByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<DistrictDataModel>.OkResponseModel(
            data: district,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "district")
        ));
    }
    /// <summary>
    /// Cập nhật District
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDistrict(Guid id, [FromBody] DistrictUpdateModel model)
    {
        await _districtService.UpdateDistrictAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "district")
        ));
    }

    /// <summary>
    /// Lấy danh sách District phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các District</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedDistrictWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var districts = await _districtService.GetPagedDistrictsWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: districts.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "district"),
            totalCount: districts.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads)
    {
        var result = await _districtService.UploadMediaAsync(id, imageUploads, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddWithMedia(Guid id, [FromForm] DistrictCreateWithMediaFileModel model)
    {
        var result = await _districtService.AddDistrictWithMediaAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpPut("update")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateWithMedia(Guid id, [FromForm] DistrictUpdateWithMediaFileModel model)
    {
        var result = await _districtService.UpdateDistrictAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }
}
