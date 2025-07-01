using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IEnumService _enumService;

    public LocationController(ILocationService locationService, IEnumService enumService)
    {
        _locationService = locationService;
        _enumService = enumService;
    }

    /// <summary>
    /// Tạo mới location
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] LocationCreateModel model)
    {
        var result = await _locationService.AddLocationAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Xóa location theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(Guid id)
    {
        await _locationService.DeleteLocationAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Lấy tất cả location
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllLocations()
    {
        var locations = await _locationService.GetAllLocationsAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: locations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Lấy thông tin location theo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById(Guid id)
    {
        var location = await _locationService.GetLocationByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: location,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    [HttpGet("{id}/with-video")]
    public async Task<IActionResult> GetLocationByIdVideo(Guid id)
    {
        var location = await _locationService.GetLocationByIdWithVideosAsync(id, new CancellationToken());
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: location,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Cập nhật thông tin location
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] LocationUpdateModel model)
    {
        await _locationService.UpdateLocationAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }

    [HttpGet("{locationId}/recommended-craftVillages")]
    public async Task<IActionResult> GetRecommendedCraftVillages(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetRecommendedCraftVillagesAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    [HttpPost("{locationId}/recommended-craftVillages")]
    public async Task<IActionResult> AddRecommendedCraftVillages(Guid locationId, [FromBody] List<Guid> craftVillageIds, CancellationToken cancellationToken)
    {
        var success = await _locationService.AddRecommendedCraftVillagesAsync(locationId, craftVillageIds, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "location")
        ));
    }

    [HttpPut("{locationId}/recommended-craftVillages")]
    public async Task<IActionResult> UpdateRecommendedCraftVillages(Guid locationId, [FromBody] List<Guid> craftVillageIds, CancellationToken cancellationToken)
    {
        var success = await _locationService.UpdateRecommendedCraftVillagesAsync(locationId, craftVillageIds, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }

    [HttpGet("{locationId}/recommended-cuisines")]
    public async Task<IActionResult> GetRecommendedCuisines(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetRecommendedCuisinesAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    [HttpPost("{locationId}/recommended-cuisines")]
    public async Task<IActionResult> AddRecommendedCuisines(Guid locationId, [FromBody] List<Guid> cuisineIds, CancellationToken cancellationToken)
    {
        var success = await _locationService.AddRecommendedCuisinesAsync(locationId, cuisineIds, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "location")
        ));
    }

    [HttpPut("{locationId}/recommended-cuisines")]
    public async Task<IActionResult> UpdateRecommendedCuisines(Guid locationId, [FromBody] List<Guid> cuisineIds, CancellationToken cancellationToken)
    {
        var success = await _locationService.UpdateRecommendedCuisinesAsync(locationId, cuisineIds, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Lấy danh sách location phân trang theo tiêu đề, loại, quận/huyện, hạng mục di sản
    /// </summary>
    /// <param name="title"></param>
    /// <param name="typeId"></param>
    /// <param name="districtId"></param>
    /// <param name="heritageRank"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedLocationWithSearch(
        string? title = null,
        Guid? typeId = null,
        Guid? districtId = null,
        HeritageRank? heritageRank = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var locations = await _locationService.GetPagedLocationsWithSearchAsync(
            title, typeId, districtId, heritageRank, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: locations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location"),
            totalCount: locations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpGet("favorite")]
    public async Task<IActionResult> GetFavoriteLocations([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetFavoriteLocationsAsync(pageNumber, pageSize, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "favorite locations")
        ));
    }

    [HttpPost("{locationId}/favorite")]
    public async Task<IActionResult> AddFavoriteLocation(Guid locationId, CancellationToken cancellationToken)
    {
        await _locationService.AddFavoriteLocationAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "favorite location")
        ));
    }

    [HttpDelete("{locationId}/favorite")]
    public async Task<IActionResult> RemoveFavoriteLocation(Guid locationId, CancellationToken cancellationToken)
    {
        await _locationService.RemoveFavoriteLocationAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "favorite location")
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads)
    {
        var result = await _locationService.UploadMediaAsync(id, imageUploads, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpPost("upload-media-2")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMediaV2(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    {
        var result = await _locationService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpPost("upload-video")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadVideo(Guid id, [FromForm] List<IFormFile> imageUploads)
    {
        var result = await _locationService.UploadVideoAsync(id, imageUploads, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    /// <summary>
    /// Cập nhật location với media
    /// </summary>
    /// <param name="id"></param>
    /// <param name="locationUpdateModel"></param>
    /// <param name="thumbnailSelected"></param>
    /// <returns></returns>
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromForm] LocationUpdateWithMediaFileModel locationUpdateModel, string? thumbnailSelected)
    {
        await _locationService.UpdateLocationAsync(id, locationUpdateModel, thumbnailSelected,
            new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpGet("heritage-rank")]
    public IActionResult GetOrderStatus()
    {
        var result = _enumService.GetEnumValues<HeritageRank>();
        return Ok(ResponseModel<List<EnumResponse>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "heritage rank")
        ));
    }

    [HttpDelete("delete-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    {
        var result = await _locationService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }

    [HttpGet("admin-get")]
    public async Task<IActionResult> GetAllLocationsAdmin()
    {
        var locations = await _locationService.GetAllLocationAdminAsync();
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: locations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location")
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedLocation(int pageNumber = 1, int pageSize = 10)
    //{
    //    var locations = await _locationService.GetPagedLocationsAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: locations.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}

    //[HttpPut("upload-image/{id}")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<object>))]
    //public async Task<IActionResult> UploadImageLocation(Guid id, IFormFile image)
    //{
    //    await _locationService.UploadImageLocationAsync(id, image, new CancellationToken());
    //    return Ok(new ResponseModel<object>(
    //        statusCode: StatusCodes.Status200OK,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "image"),
    //        data: true
    //    ));
    //}

    //[HttpGet("search-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedLocationWithSearch(int pageNumber = 1, int pageSize = 10, string title = "")
    //{
    //    var locations = await _locationService.GetPagedLocationsWithSearchAsync(pageNumber, pageSize, title, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: locations.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
