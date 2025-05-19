using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.NewsCategoryModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NewsCategoryController : ControllerBase
{
    private readonly INewsCategoryService _newsService;

    public NewsCategoryController(INewsCategoryService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// Tạo mới news
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateNewsCategory([FromBody] NewsCategoryCreateModel model)
    {
        await _newsService.AddNewsCategoryAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Xóa news theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNewsCategory(Guid id)
    {
        await _newsService.DeleteNewsCategoryAsync(id, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy tất cả news
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllNewsCategorys()
    {
        var newss = await _newsService.GetAllNewsCategorysAsync(new CancellationToken());
        return Ok(ResponseModel<List<NewsCategoryDataModel>>.OkResponseModel(
            data: newss,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy news theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsCategoryById(Guid id)
    {
        var news = await _newsService.GetNewsCategoryByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<NewsCategoryDataModel>.OkResponseModel(
            data: news,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news")
        ));
    }
    /// <summary>
    /// Cập nhật news
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNewsCategory(Guid id, [FromBody] NewsCategoryUpdateModel model)
    {
        await _newsService.UpdateNewsCategoryAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy danh sách news phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <returns>Trả về danh sách các news</returns>
    [HttpGet("get-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedNewsCategory(int pageNumber = 1, int pageSize = 10)
    {
        var newss = await _newsService.GetPagedNewsCategorysAsync(pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: newss.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news"),
            totalCount: newss.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách news phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="name">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các news</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedNewsCategoryWithSearch(string? name, int pageNumber = 1, int pageSize = 10)
    {
        var newss = await _newsService.GetPagedNewsCategorysWithSearchAsync(pageNumber, pageSize, name ??= "", new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: newss.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news"),
            totalCount: newss.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }
}
