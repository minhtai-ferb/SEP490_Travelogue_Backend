using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.RefundRequestModels;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RejectionRequestController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public RejectionRequestController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    /// <summary>
    /// Tạo yêu cầu từ chối: hướng dẫn viên muốn hủy 1 lịch trình đã nhận (schedule hoặc booking).
    /// </summary>
    /// <param name="dto">Thông tin yêu cầu từ chối được gửi lên</param>
    /// <returns>Trả về kết quả tạo yêu cầu thành công hoặc thất bại</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRejectionRequest([FromBody] RejectionRequestCreateDto dto)
    {
        var response = await _tourGuideService.CreateRejectionRequestAsync(dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Phê duyệt yêu cầu từ chối: admin/moderator chấp nhận cho hướng dẫn viên hủy và chỉ định hướng dẫn viên mới.
    /// </summary>
    /// <param name="requestId">Id của yêu cầu từ chối</param>
    /// <param name="newTourGuideId">Id của hướng dẫn viên mới sẽ thay thế</param>
    /// <returns>Trả về kết quả phê duyệt thành công hoặc thất bại</returns>
    [HttpPut("{requestId}/approve")]
    public async Task<IActionResult> ApproveRejectionRequest(Guid requestId, Guid newTourGuideId)
    {
        var response = await _tourGuideService.ApproveRejectionRequestAsync(requestId, newTourGuideId);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Từ chối yêu cầu từ chối: admin/moderator không chấp nhận cho hủy lịch trình.
    /// </summary>
    /// <param name="requestId">Id của yêu cầu từ chối</param>
    /// <param name="dto">Thông tin lý do từ chối yêu cầu</param>
    /// <returns>Trả về kết quả từ chối thành công hoặc thất bại</returns>
    [HttpPut("{requestId}/reject")]
    public async Task<IActionResult> RejectRejectionRequest(Guid requestId, [FromBody] RejectRejectionRequestDto dto)
    {
        var response = await _tourGuideService.RejectRejectionRequestAsync(requestId, dto);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "report")
        ));
    }

    /// <summary>
    /// Lấy danh sách yêu cầu từ chối để quản trị viên xem (có phân trang và bộ lọc).
    /// </summary>
    /// <param name="filterm">Bộ lọc cho danh sách yêu cầu (trạng thái, ngày, ...)</param>
    /// <param name="pageNumber">Số trang cần lấy (mặc định = 1)</param>
    /// <param name="pageSize">Số lượng bản ghi trên mỗi trang (mặc định = 10)</param>
    /// <returns>Trả về danh sách yêu cầu từ chối theo phân trang</returns>
    [HttpGet("page")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<PagedResult<RejectionRequestResponseDto>>))]
    public async Task<IActionResult> GetRefundRequestsForAdmin([FromQuery] RejectionRequestFilter filterm, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _tourGuideService.GetRejectionRequestsForAdminAsync(filterm, pageNumber, pageSize );
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    /// <summary>
    /// Lấy chi tiết một yêu cầu từ chối theo Id.
    /// </summary>
    /// <param name="requestId">Id của yêu cầu từ chối</param>
    /// <returns>Trả về thông tin chi tiết yêu cầu từ chối</returns>
    [HttpGet("{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<RejectionRequestResponseDto>))]
    public async Task<IActionResult> GetRejectionRequestById(Guid requestId)
    {
        var result = await _tourGuideService.GetRejectionRequestByIdAsync(requestId);
        return Ok(ResponseModel<RejectionRequestResponseDto>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }
}
