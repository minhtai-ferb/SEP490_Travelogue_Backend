using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ReportModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Người dùng gửi báo cáo về một bài đánh giá (review) mà họ cho là vi phạm.
    /// </summary>
    /// <param name="dto">
    /// Thông tin báo cáo bao gồm ID của review và lý do báo cáo.
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Đối tượng <see cref="ReportResponseDto"/> chứa thông tin báo cáo vừa tạo.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _reportService.CreateReportAsync(dto, cancellationToken);
        return Ok(ResponseModel<ReportResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Người dùng xóa báo cáo của chính mình.
    /// </summary>
    /// <param name="reportId">
    /// ID của báo cáo cần xóa.
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Trả về <c>true</c> nếu xóa thành công (xóa logic).  
    /// Nếu báo cáo đã được xử lý thì không thể xóa.
    /// </returns>
    [HttpDelete("{reportId}")]
    public async Task<IActionResult> DeleteReport(Guid reportId, CancellationToken cancellationToken)
    {
        await _reportService.DeleteReportAsync(reportId, cancellationToken);
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Lấy tất cả báo cáo do người dùng hiện tại đã gửi.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Danh sách <see cref="ReportResponseDto"/> chứa thông tin các báo cáo của người dùng.
    /// </returns>
    [HttpGet("my-reports")]
    public async Task<IActionResult> GetMyReports(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetMyReportsAsync(cancellationToken);
        return Ok(ResponseModel<List<ReportResponseDto>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reports")
        ));
    }

    /// <summary>
    /// Lấy chi tiết một bài đánh giá cùng tất cả báo cáo liên quan đến nó.
    /// </summary>
    /// <param name="reviewId">
    /// ID của đánh giá cần lấy báo cáo.
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Đối tượng <see cref="ReviewReportDetailDto"/> chứa thông tin review và danh sách báo cáo.
    /// </returns>
    [HttpGet("by-review/{reviewId}")]
    public async Task<IActionResult> GetReportsById(Guid reviewId, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetReviewWithReportByIdAsync(reviewId, cancellationToken);
        return Ok(ResponseModel<ReviewReportDetailDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reports")
        ));
    }

    /// <summary>
    /// Người dùng cập nhật nội dung báo cáo của mình khi báo cáo vẫn còn ở trạng thái Pending.
    /// </summary>
    /// <param name="reportId">
    /// ID của báo cáo cần cập nhật.
    /// </param>
    /// <param name="dto">
    /// Thông tin cập nhật (ví dụ: lý do báo cáo mới).
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Đối tượng <see cref="ReportResponseDto"/> chứa thông tin báo cáo sau khi cập nhật.
    /// </returns>
    [HttpPut("{reportId}")]
    public async Task<IActionResult> UpdateReport(Guid reportId, [FromBody] UpdateReportRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _reportService.UpdateReportAsync(reportId, dto, cancellationToken);
        return Ok(ResponseModel<ReportResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Lấy danh sách các bài đánh giá đã có báo cáo, kèm thông tin thống kê.
    /// </summary>
    /// <param name="status">
    /// (Tùy chọn) Lọc theo trạng thái xử lý cuối cùng của báo cáo: Pending, Processed, Rejected.
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <param name="pageNumber">
    /// Trang hiện tại (mặc định = 1).
    /// </param>
    /// <param name="pageSize">
    /// Số lượng phần tử trên mỗi trang (mặc định = 10).
    /// </param>
    /// <returns>
    /// Danh sách phân trang chứa các review kèm báo cáo liên quan.
    /// </returns>
    [HttpGet("page")]
    public async Task<IActionResult> GetReportedReviewsAsync(ReportStatus? status, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _reportService.GetReportedReviewsAsync(status, cancellationToken, pageNumber, pageSize);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reviews with reports")
        ));
    }


    /// <summary>
    /// Admin/Moderator xử lý một báo cáo: phê duyệt hoặc từ chối.
    /// </summary>
    /// <param name="reportId">
    /// ID của báo cáo cần xử lý.
    /// </param>
    /// <param name="dto">
    /// Thông tin xử lý báo cáo (trạng thái mới, ghi chú của admin/mod).
    /// </param>
    /// <param name="cancellationToken">
    /// Token hủy thao tác bất đồng bộ.
    /// </param>
    /// <returns>
    /// Đối tượng <see cref="ReportResponseDto"/> chứa thông tin báo cáo sau khi được xử lý.
    /// </returns>
    [HttpPost("{reportId}/process")]
    public async Task<IActionResult> ProcessReport(Guid reportId, [FromBody] ProcessReportRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _reportService.ProcessReportAsync(reportId, dto, cancellationToken);
        return Ok(ResponseModel<ReportResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.PROCESS_SUCCESS, "report")
        ));
    }
}