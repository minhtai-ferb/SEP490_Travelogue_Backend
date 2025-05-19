using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.RestaurantModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Tạo mới restaurant
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantCreateModel model)
    {
        await _restaurantService.AddRestaurantAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "restaurant")
        ));
    }

    /// <summary>
    /// Xóa restaurant theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(Guid id)
    {
        await _restaurantService.DeleteRestaurantAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "restaurant")
        ));
    }

    /// <summary>
    /// Lấy tất cả restaurant
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllRestaurants()
    {
        var restaurants = await _restaurantService.GetAllRestaurantsAsync(new CancellationToken());
        return Ok(ResponseModel<List<RestaurantDataModel>>.OkResponseModel(
            data: restaurants,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "restaurant")
        ));
    }

    /// <summary>
    /// Lấy restaurant theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurantById(Guid id)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<RestaurantDataModel>.OkResponseModel(
            data: restaurant,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "restaurant")
        ));
    }
    /// <summary>
    /// Cập nhật restaurant
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(Guid id, [FromBody] RestaurantUpdateModel model)
    {
        await _restaurantService.UpdateRestaurantAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "restaurant")
        ));
    }

    /// <summary>
    /// Lấy danh sách restaurant phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các restaurant</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedRestaurantWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var restaurants = await _restaurantService.GetPagedRestaurantsWithSearchAsync(name, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: restaurants.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "restaurant"),
            totalCount: restaurants.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    {
        var result = await _restaurantService.UploadMediaAsync(id, imageUploads, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpDelete("delete-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    {
        var result = await _restaurantService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }

    //[HttpGet("get-paged")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> GetPagedRestaurant(int pageNumber = 1, int pageSize = 10)
    //{
    //    var restaurants = await _restaurantService.GetPagedRestaurantsAsync(pageNumber, pageSize, new CancellationToken());
    //    return Ok(PagedResponseModel<object>.OkResponseModel(
    //        data: restaurants.Items,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "restaurant"),
    //        pageSize: pageSize,
    //        pageNumber: pageNumber
    //    ));
    //}
}
