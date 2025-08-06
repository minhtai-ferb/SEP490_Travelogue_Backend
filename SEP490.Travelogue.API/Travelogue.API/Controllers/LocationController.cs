using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HistoricalLocationModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IEnumService _enumService;
    private readonly IHistoricalLocationService _historicalLocationService;
    private readonly ICraftVillageService _craftVillageService;
    private readonly ICuisineService _cuisineService;

    public LocationController(ILocationService locationService, IEnumService enumService, IHistoricalLocationService historicalLocationService, ICraftVillageService craftVillageService, ICuisineService cuisineService)
    {
        _locationService = locationService;
        _enumService = enumService;
        _historicalLocationService = historicalLocationService;
        _craftVillageService = craftVillageService;
        _cuisineService = cuisineService;
    }

    /// <summary>
    /// Tạo mới location
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] LocationCreateModel model)
    {
        var result = await _locationService.CreateBasicLocationAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "location")
        ));
    }

    /// <summary>
    /// Adds cuisine data to an existing location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="model">The cuisine data.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{locationId}/cuisine")]
    public async Task<IActionResult> AddCuisineData(Guid locationId, [FromBody] CuisineCreateModel model)
    {
        var result = await _locationService.AddCuisineDataAsync(locationId, model, CancellationToken.None);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "cuisine data")
        ));
    }

    /// <summary>
    /// Adds craft village data to an existing location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="model">The craft village data.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{locationId}/craft-village")]
    public async Task<IActionResult> AddCraftVillageData(Guid locationId, [FromBody] CraftVillageCreateModel model)
    {
        var result = await _locationService.AddCraftVillageDataAsync(locationId, model, CancellationToken.None);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "craft village data")
        ));
    }

    /// <summary>
    /// Adds historical location data to an existing location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="model">The historical location data.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{locationId}/historical-location")]
    public async Task<IActionResult> AddHistoricalLocationData(Guid locationId, [FromBody] HistoricalLocationCreateModel model)
    {
        var result = await _locationService.AddHistoricalLocationDataAsync(locationId, model, CancellationToken.None);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "historical location data")
        ));
    }

    /// <summary>
    /// Cập nhật dữ liệu ẩm thực 
    /// </summary>
    /// <param name="locationId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{locationId}/cuisine")]
    public async Task<IActionResult> UpdateCuisineData(
        Guid locationId,
        [FromBody] CuisineUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _locationService.UpdateCuisineDataAsync(locationId, dto, cancellationToken);

        return Ok(ResponseModel<LocationDataModel>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "cuisine")
        ));
    }

    /// <summary>
    /// Cập nhật dữ liệu làng nghề
    /// </summary>
    /// <param name="locationId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{locationId}/craft-village")]
    public async Task<IActionResult> UpdateCraftVillageData(
            Guid locationId,
            [FromBody] CraftVillageUpdateDto dto,
            CancellationToken cancellationToken = default)
    {
        var result = await _locationService.UpdateCraftVillageDataAsync(locationId, dto, cancellationToken);

        return Ok(ResponseModel<LocationDataModel>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "craft‑village")
        ));
    }

    /// <summary>
    /// Cập nhật dữ liệu địa điểm lịch sử
    /// </summary>
    /// <param name="locationId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{locationId}/historical-location")]
    public async Task<IActionResult> UpdateHistoricalLocationData(
        Guid locationId,
        [FromBody] HistoricalLocationUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _locationService.UpdateHistoricalLocationDataAsync(locationId, dto, cancellationToken);

        return Ok(ResponseModel<LocationDataModel>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "historical location")
        ));
    }

    /// <summary>
    /// Cập nhật dữ liệu danh lam thắng cảnh
    /// </summary>
    /// <param name="locationId"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{locationId}/scenic-spot")]
    public async Task<IActionResult> UpdateScenicSpotData(
        Guid locationId,
        [FromBody] HistoricalLocationUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _locationService.UpdateHistoricalLocationDataAsync(locationId, dto, cancellationToken);

        return Ok(ResponseModel<LocationDataModel>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "historical location")
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
    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] LocationUpdateModel model)
    // {
    //     await _locationService.UpdateLocationAsync(id, model, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: true,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "location")
    //     ));
    // }

    [HttpGet("nearest-cuisine")]
    public async Task<IActionResult> GetNearestCuisineLocations(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetNearestCuisineLocationsAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "cuisines")
        ));
    }

    [HttpGet("nearest-craft-village")]
    public async Task<IActionResult> GetNearestCraftVillageLocations(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetNearestCraftVillageLocationsAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craft villages")
        ));
    }

    [HttpGet("nearest-historical")]
    public async Task<IActionResult> GetNearestHistoricalLocations(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetNearestHistoricalLocationsAsync(locationId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historical locations")
        ));
    }

    /// <summary>
    /// Lấy danh sách location phân trang theo tiêu đề, loại, quận/huyện, hạng mục di sản
    /// </summary>
    /// <param name="title"></param>
    /// <param name="type"></param>
    /// <param name="districtId"></param>
    /// <param name="heritageRank"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedLocationWithSearch(
        string? title = null,
        LocationType? type = null,
        Guid? districtId = null,
        HeritageRank? heritageRank = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var locations = await _locationService.GetPagedLocationsWithSearchAsync(
            title, type, districtId, heritageRank, pageNumber, pageSize, new CancellationToken());
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

    // [HttpPost("upload-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads)
    // {
    //     var result = await _locationService.UploadMediaAsync(id, imageUploads, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
    //     ));
    // }

    // lay thi lay ham nay
    // [HttpPost("upload-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> UploadMediaV2(Guid id, [FromForm] UploadMediasDto imageUploads, string? thumbnailFileName)
    // {
    //     var result = await _locationService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
    //     ));
    // }

    [HttpGet("heritage-rank")]
    public IActionResult GetOrderStatus()
    {
        var result = _enumService.GetEnumValues<HeritageRank>();
        return Ok(ResponseModel<List<EnumResponse>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "heritage rank")
        ));
    }

    [HttpGet("location-type")]
    public IActionResult GetLocationType()
    {
        var result = _enumService.GetEnumValues<LocationType>();
        return Ok(ResponseModel<List<EnumResponse>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "location type")
        ));
    }

    /// <summary>
    /// Xóa media của location
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deletedImages"></param>
    /// <returns></returns>
    // [HttpDelete("delete-media")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    // {
    //     var result = await _locationService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
    //     ));
    // }

    /// <summary>
    /// Lấy tất cả historicalLocation
    /// </summary>
    /// <returns></returns>
    [HttpGet("historical-locations")]
    public async Task<IActionResult> GetAllHistoricalLocations()
    {
        var historicalLocations = await _historicalLocationService.GetAllHistoricalLocationsAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: historicalLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy historicalLocation theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("historical-locations/{id}")]
    public async Task<IActionResult> GetHistoricalLocationById(Guid id)
    {
        var historicalLocationResult = await _historicalLocationService.GetHistoricalLocationByLocationIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<LocationDataDetailModel>.OkResponseModel(
            data: historicalLocationResult,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy danh sách historicalLocation phân trang theo tên, loại historicalLocation, địa điểm, quận, tháng, năm
    /// </summary>
    /// <param name="name">Tên historicalLocation</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("historical-locations/filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedHistoricalLocationWithFilter(
        string? name = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var historicalLocations = await _historicalLocationService.GetPagedHistoricalLocationsWithSearchAsync(
            name, pageNumber, pageSize, new CancellationToken()
        );

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: historicalLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation"),
            totalCount: historicalLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy tất cả làng nghề
    /// </summary>
    /// <returns></returns>
    [HttpGet("craft-villages")]
    public async Task<IActionResult> GetAllCraftVillages()
    {
        var historicalLocations = await _craftVillageService.GetAllCraftVillagesAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: historicalLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy danh sách hotel phân trang theo tên, loại hotel, địa điểm, quận, tháng, năm
    /// </summary>
    /// <param name="name">Tên historicalLocation</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("craft-villages/filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCraftVillageWithFilter(
        string? name = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var historicalLocations = await _craftVillageService.GetPagedCraftVillagesWithSearchAsync(
            name, pageNumber, pageSize, new CancellationToken()
        );

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: historicalLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation"),
            totalCount: historicalLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy tất cả cuisine
    /// </summary>
    /// <returns></returns>
    [HttpGet("cuisine")]
    public async Task<IActionResult> GetAllCuisines()
    {
        var historicalLocations = await _cuisineService.GetAllCuisinesAsync(new CancellationToken());
        return Ok(ResponseModel<List<LocationDataModel>>.OkResponseModel(
            data: historicalLocations,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation")
        ));
    }

    /// <summary>
    /// Lấy danh sách hotel phân trang theo tên, loại hotel, địa điểm, quận, tháng, năm
    /// </summary>
    /// <param name="name">Tên historicalLocation</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("cuisine/filter-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedCuisineWithFilter(
        string? name = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var historicalLocations = await _cuisineService.GetPagedCuisinesWithSearchAsync(
            name, pageNumber, pageSize, new CancellationToken()
        );

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: historicalLocations.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "historicalLocation"),
            totalCount: historicalLocations.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
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
