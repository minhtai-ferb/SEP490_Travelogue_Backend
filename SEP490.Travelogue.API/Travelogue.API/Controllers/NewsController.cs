using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
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
        return Ok(ResponseModel<NewsDataModel>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy tất cả news
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllNews()
    {
        var newss = await _newsService.GetAllNewsAsync(new CancellationToken());
        return Ok(ResponseModel<List<NewsDataModel>>.OkResponseModel(
            data: newss,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy tất cả news theo category
    /// </summary>
    /// <returns></returns>
    [HttpGet("by-category")]
    public async Task<IActionResult> GetByCategory([FromQuery] NewsCategory? category, CancellationToken cancellationToken)
    {
        var result = await _newsService.GetNewsByCategoryAsync(category, cancellationToken);
        return Ok(ResponseModel<List<NewsDataModel>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news")
        ));
    }

    /// <summary>
    /// Lấy danh sách sự kiện (Event) phân trang kèm bộ lọc.
    /// </summary>
    /// <param name="title">Tiêu đề cần tìm (tùy chọn).</param>
    /// <param name="locationId">ID địa điểm cần lọc (tùy chọn).</param>
    /// <param name="isHighlighted">Trạng thái nổi bật (tùy chọn).</param>
    /// <param name="month">Tháng diễn ra sự kiện (tùy chọn).</param>
    /// <param name="year">Năm diễn ra sự kiện (tùy chọn).</param>
    /// <param name="pageNumber">Số trang (mặc định 1).</param>
    /// <param name="pageSize">Kích thước trang (mặc định 10).</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>
    /// Danh sách <see cref="NewsDataModel"/> của sự kiện,
    /// bọc trong <see cref="ResponseModel{T}"/>.
    /// </returns>
    [HttpGet("event/search-paged")]
    public async Task<IActionResult> GetPagedEvents(
        [FromQuery] string? title,
        [FromQuery] Guid? locationId,
        [FromQuery] Boolean? isHighlighted,
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _newsService.GetPagedEventWithFilterAsync(title, locationId, isHighlighted, month, year, pageNumber, pageSize, cancellationToken);

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "events"),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách bản tin (News thường) phân trang kèm bộ lọc.
    /// </summary>
    /// <param name="title">Tiêu đề cần tìm (tùy chọn).</param>
    /// <param name="locationId">ID địa điểm cần lọc (tùy chọn).</param>
    /// <param name="isHighlighted">Trạng thái nổi bật (tùy chọn).</param>
    /// <param name="pageNumber">Số trang (mặc định 1).</param>
    /// <param name="pageSize">Kích thước trang (mặc định 10).</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>
    /// Danh sách <see cref="NewsDataModel"/> của loại News,
    /// bọc trong <see cref="ResponseModel{T}"/>.
    /// </returns>
    [HttpGet("new/search-paged")]
    public async Task<IActionResult> GetPagedNews(
        [FromQuery] string? title,
        [FromQuery] Guid? locationId,
        [FromQuery] Boolean? isHighlighted,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _newsService.GetPagedNewsWithFilterAsync(title, locationId, isHighlighted, pageNumber, pageSize, cancellationToken);

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news"),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    /// <summary>
    /// Lấy danh sách trải nghiệm (Experience) phân trang kèm bộ lọc.
    /// </summary>
    /// <param name="title">Tiêu đề cần tìm (tùy chọn).</param>
    /// <param name="locationId">ID địa điểm cần lọc (tùy chọn).</param>
    /// <param name="isHighlighted">Trạng thái nổi bật (tùy chọn).</param>
    /// <param name="typeExperience">Loại trải nghiệm (tùy chọn).</param>
    /// <param name="pageNumber">Số trang (mặc định 1).</param>
    /// <param name="pageSize">Kích thước trang (mặc định 10).</param>
    /// <param name="cancellationToken">Token hủy bất đồng bộ.</param>
    /// <returns>
    /// Danh sách <see cref="NewsDataModel"/> của loại Experience,
    /// bọc trong <see cref="ResponseModel{T}"/>.
    /// </returns>
    [HttpGet("experience/search-paged")]
    public async Task<IActionResult> GetPagedExperiences(
        [FromQuery] string? title,
        [FromQuery] Guid? locationId,
        [FromQuery] Boolean? isHighlighted,
        [FromQuery] TypeExperience? typeExperience,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _newsService.GetPagedExperienceWithFilterAsync(title, locationId, typeExperience , isHighlighted, pageNumber, pageSize, cancellationToken);

        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: result.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "experiences"),
            totalCount: result.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
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
    /// Lấy danh sách news phân trang theo tiêu đề
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="title">Tiêu đề cần tìm kiếm</param>
    /// <returns>Trả về danh sách các news</returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedNewsWithSearch(string? title, int pageNumber = 1, int pageSize = 10)
    {
        var newss = await _newsService.GetPagedNewsWithSearchAsync(title, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: newss.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "news"),
            totalCount: newss.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
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
}
