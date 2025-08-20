using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
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
    /// Tạo báo cáo mới cho một đánh giá
    /// </summary>
    /// <param name="dto">Thông tin báo cáo</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Thông tin báo cáo vừa tạo</returns>
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
    /// Xóa báo cáo theo ID
    /// </summary>
    /// <param name="reportId">ID của báo cáo</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Kết quả xóa</returns>
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
    /// Lấy danh sách báo cáo của người dùng hiện tại
    /// </summary>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Danh sách báo cáo</returns>
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
    /// Lấy danh sách báo cáo theo ID của đánh giá
    /// </summary>
    /// <param name="id">ID của đánh giá</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Danh sách báo cáo liên quan đến đánh giá</returns>
    [HttpGet("by-review/{id}")]
    public async Task<IActionResult> GetReportsById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetReportsByIdAsync(id, cancellationToken);
        return Ok(ResponseModel<List<ReportResponseDto>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reports")
        ));
    }

    /// <summary>
    /// Cập nhật báo cáo theo ID
    /// </summary>
    /// <param name="reportId">ID của báo cáo</param>
    /// <param name="dto">Thông tin cập nhật báo cáo</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Thông tin báo cáo đã cập nhật</returns>
    [HttpPut("{reportId}")]
    public async Task<IActionResult> UpdateReport(Guid reportId, [FromBody] UpdateReportRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _reportService.UpdateReportAsync(reportId, dto, cancellationToken);
        return Ok(ResponseModel<ReportResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "report")
        ));
    }

    ///// <summary>
    ///// Lấy tất cả báo cáo - admin
    ///// </summary>
    ///// <param name="cancellationToken">Token hủy thao tác</param>
    ///// <returns>Danh sách tất cả báo cáo</returns>
    //[HttpGet]
    //public async Task<IActionResult> GetAllReports(CancellationToken cancellationToken)
    //{
    //    var result = await _reportService.GetAllReportsAsync(cancellationToken);
    //    return Ok(ResponseModel<List<ReportResponseDto>>.OkResponseModel(
    //        data: result,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reports")
    //    ));
    //}

    /// <summary>
    /// Lấy danh sách báo cáo theo trạng thái - admin
    /// </summary>
    /// <param name="status">Trạng thái báo cáo (Pending, Approved, Rejected)</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Danh sách báo cáo theo trạng thái</returns>
    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetReportsByStatus(ReportStatus status, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetReportsByStatusAsync(status, cancellationToken);
        return Ok(ResponseModel<List<ReportResponseDto>>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reports")
        ));
    }

    ///// <summary>
    ///// Lấy danh sách tất cả đánh giá có báo cáo
    ///// </summary>
    //[HttpGet]
    //public async Task<IActionResult> GetAllReviewsHaveReports(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken )
    //{
    //    var result = await _reportService.GetAllReviewsHaveReportsAsync(pageNumber, pageSize, cancellationToken);
    //    return Ok(ResponseModel<object>.OkResponseModel(
    //        data: result,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "reviews with reports")
    //    ));
    //}


    /// <summary>
    /// Xử lý báo cáo - admin
    /// </summary>
    /// <param name="reportId">ID của báo cáo</param>
    /// <param name="dto">Thông tin xử lý báo cáo</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Thông tin báo cáo đã xử lý</returns>
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