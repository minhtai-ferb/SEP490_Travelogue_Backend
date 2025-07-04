using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CuisineController : ControllerBase
{
    private readonly ICuisineService _cuisineService;

    public CuisineController(ICuisineService cuisineService)
    {
        _cuisineService = cuisineService;
    }

    /// <summary>
    /// Tạo mới cuisine
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateCuisine([FromBody] CuisineCreateModel model)
    {
        await _cuisineService.AddCuisineAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "cuisine")
        ));
    }

    /// <summary>
    /// Xóa cuisine theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCuisine(Guid id)
    {
        await _cuisineService.DeleteCuisineAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "cuisine")
        ));
    }

    /// <summary>
    /// Lấy tất cả cuisine
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCuisines()
    {
        var cuisines = await _cuisineService.GetAllCuisinesAsync(new CancellationToken());
        return Ok(ResponseModel<List<CuisineDataModel>>.OkResponseModel(
            data: cuisines,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "cuisine")
        ));
    }

    /// <summary>
    /// Lấy cuisine theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCuisineById(Guid id)
    {
        var cuisine = await _cuisineService.GetCuisineByLocationIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<CuisineDataModel>.OkResponseModel(
            data: cuisine,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "cuisine")
        ));
    }
    /// <summary>
    /// Cập nhật cuisine
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCuisine(Guid id, [FromBody] CuisineUpdateModel model)
    {
        await _cuisineService.UpdateCuisineAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "cuisine")
        ));
    }

    /// <summary>
    /// Lấy danh sách cuisine phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các cuisine</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCuisineWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var cuisines = await _cuisineService.GetPagedCuisinesWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: cuisines.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "cuisine"),
            totalCount: cuisines.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    // [HttpPost("upload-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    // {
    //     var result = await _cuisineService.UploadMediaAsync(id, imageUploads, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
    //     ));
    // }

    [HttpDelete("delete-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    {
        var result = await _cuisineService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedCuisine(int pageNumber = 1, int pageSize = 10)
    //{
    //    var cuisines = await _cuisineService.GetPagedCuisinesAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: cuisines.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "cuisine"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
