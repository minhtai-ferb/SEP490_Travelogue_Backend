using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.NewsModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// Tạo mới news
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateNews([FromBody] NewsCreateModel model)
    {
        var result = await _newsService.AddNewsAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Xóa news theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNews(Guid id)
    {
        await _newsService.DeleteNewsAsync(id, new CancellationToken());
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
    public async Task<IActionResult> GetAllNewss()
    {
        var newss = await _newsService.GetAllNewssAsync(new CancellationToken());
        return Ok(ResponseModel<List<NewsDataModel>>.OkResponseModel(
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
    public async Task<IActionResult> GetNewsById(Guid id)
    {
        var news = await _newsService.GetNewsByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<NewsDataDetailModel>.OkResponseModel(
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
    public async Task<IActionResult> UpdateNews(Guid id, [FromBody] NewsUpdateModel model)
    {
        await _newsService.UpdateNewsAsync(id, model, new CancellationToken());
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
    public async Task<IActionResult> GetPagedNews(int pageNumber = 1, int pageSize = 10)
    {
        var newss = await _newsService.GetPagedNewssAsync(pageNumber, pageSize, new CancellationToken());
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
    /// <param name="title">Tiêu đề cần tìm kiếm</param>
    /// <param name="categoryName">Tên thể loại cần tìm kiếm</param>
    /// <param name="categoryId">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các news</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedNewsWithSearch(string? title, string? categoryName, Guid? categoryId, int pageNumber = 1, int pageSize = 10)
    {
        var newss = await _newsService.GetPagedNewsWithSearchAsync(title, categoryName, categoryId, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: newss.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news"),
            totalCount: newss.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpPost("upload-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadMedia(Guid id, [FromForm] List<IFormFile> imageUploads, string? thumbnailFileName)
    {
        var result = await _newsService.UploadMediaAsync(id, imageUploads, thumbnailFileName, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPLOAD_SUCCESS, "media")
        ));
    }

    [HttpDelete("delete-media")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMedia(Guid id, List<string> deletedImages)
    {
        var result = await _newsService.DeleteMediaAsync(id, deletedImages, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "media")
        ));
    }
}
