using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CraftVillageController : ControllerBase
{
    private readonly ICraftVillageService _craftVillageService;

    public CraftVillageController(ICraftVillageService craftVillageService)
    {
        _craftVillageService = craftVillageService;
    }

    /// <summary>
    /// Tạo mới craftVillage
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateCraftVillage([FromBody] CraftVillageCreateModel model)
    {
        await _craftVillageService.AddCraftVillageAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "craftVillage")
        ));
    }

    /// <summary>
    /// Xóa craftVillage theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCraftVillage(Guid id)
    {
        await _craftVillageService.DeleteCraftVillageAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "craftVillage")
        ));
    }

    /// <summary>
    /// Lấy tất cả craftVillage
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCraftVillages()
    {
        var craftVillages = await _craftVillageService.GetAllCraftVillagesAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: craftVillages,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craftVillage")
        ));
    }

    /// <summary>
    /// Lấy craftVillage theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCraftVillageById(Guid id)
    {
        var craftVillage = await _craftVillageService.GetCraftVillageByLocationIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: craftVillage,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craftVillage")
        ));
    }
    /// <summary>
    /// Cập nhật craftVillage
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCraftVillage(Guid id, [FromBody] CraftVillageUpdateModel model)
    {
        await _craftVillageService.UpdateCraftVillageAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "craftVillage")
        ));
    }

    /// <summary>
    /// Lấy danh sách craftVillage phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Trả về danh sách các craftVillage</returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCraftVillage(int pageNumber = 1, int pageSize = 10)
    {
        var craftVillages = await _craftVillageService.GetPagedCraftVillagesAsync(pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: craftVillages.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craftVillage"),
            totalCount: craftVillages.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách craftVillage phân trang theo tên
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCraftVillageWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var craftVillages = await _craftVillageService.GetPagedCraftVillagesWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: craftVillages.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craftVillage"),
            totalCount: craftVillages.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    // [HttpPost("upload-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    // {
    //     var result = await _craftVillageService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
    //     ));
    // }
}
