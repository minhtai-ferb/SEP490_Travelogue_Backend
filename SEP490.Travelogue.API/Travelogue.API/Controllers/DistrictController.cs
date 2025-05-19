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
    /// Tạo mới District và Role
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddDistrictWithRole([FromBody] DistrictCreateModel model)
    {
        var ressult = await _districtService.AddDistrictWithRoleAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: ressult,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "district")
        ));
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

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedDistrict(int pageNumber = 1, int pageSize = 10)
    //{
    //    var districts = await _districtService.GetPagedDistrictsAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: districts.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "district"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}

    /// <summary>
    /// Tạo mới District
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    //[HttpPost]
    //public async Task<IActionResult> CreateDistrict([FromBody] DistrictCreateModel model)
    //{
    //    await _districtService.AddDistrictAsync(model, new CancellationToken());
    //    return Ok(ResponseModel<object>.OkResponseModel(
    //        data: true,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "district")
    //    ));
    //}

    /// <summary>
    /// Lấy tên DistrictAdmin theo District (dùng để test cách hệ thống cấu hình lại tên role theo District)
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    //[HttpPost("district_admin_name")]
    //public async Task<IActionResult> GetDistrictAdminName([FromBody] DistrictCreateModel model)
    //{
    //    var newName = await _districtService.GetDistrictRoleNameAsync(model, new CancellationToken());
    //    return Ok(ResponseModel<object>.OkResponseModel(
    //        data: newName,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "district")
    //    ));
    //}
}
