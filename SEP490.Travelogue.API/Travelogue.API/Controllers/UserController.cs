using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.UserModels;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICloudinaryService _cloudinaryService;
    public UserController(IUserService userService, ICloudinaryService cloudinaryService)
    {
        _userService = userService;
        _cloudinaryService = cloudinaryService;
    }

    //[HttpGet("profile")]
    //[Authorize]
    //public async Task<IActionResult> Profile()
    //{
    //    var user = await _userService.GetUser(User);
    //    return Ok(user);
    //}

    /// <summary>
    /// Update user role
    /// </summary>
    /// <param name="userId">Id người dùng muốn cập nhật role</param>
    /// <param name="request">danh sách role mới của người dùng</param>
    /// <returns></returns>
    // [HttpPost("update-role")]
    // [Authorize(Roles = "Admin")]
    // public async Task<IActionResult> UpdateRole(Guid userId, [FromBody] UserRoleUpdateModel request)
    // {
    //     var result = await _userService.UpdateUserRolesAsync(userId, request.RoleIds);
    //     return Ok(ResponseModel<bool>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user - role")
    //     ));
    // }

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpPost("assign-role-to-user")]
    public async Task<IActionResult> AssignRoleToUser(Guid userId, Guid roleId)
    {
        var result = await _userService.AssignRoleToUserAsync(userId, roleId, new CancellationToken());
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user - role")
        ));
    }

    /// <summary>
    /// Xóa vai trò của người dùng
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpDelete("remove-user-from-role")]
    public async Task<IActionResult> RemoveUserFromRole(Guid userId, Guid roleId)
    {
        var result = await _userService.RemoveUserFromRole(userId, roleId, new CancellationToken());
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user - role")
        ));
    }

    /// <summary>
    /// Mobile - gửi mail ý kiến
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("send-feedback")]
    public async Task<IActionResult> SendFeedback([FromBody] FeedbackModel model)
    {
        await _userService.SendFeedbackAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUBMIT_SUCCESS, "feedback")
        ));
    }

    [HttpGet("user-detail")]
    public async Task<IActionResult> GetUserManageAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await _userService.GetUserManageAsync(userId, ct);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpGet("all-user-detail")]
    public async Task<IActionResult> GetAllUserManageAsync(CancellationToken ct = default)
    {
        var response = await _userService.GetAllUserManageAsync(ct);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpPatch("enable-user-role/{userId}/{roleId}")]
    public async Task<IActionResult> EnableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var response = await _userService.EnableUserRoleAsync(userId, roleId, ct);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    [HttpPatch("disable-user-role/{userId}/{roleId}")]
    public async Task<IActionResult> DisableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var response = await _userService.DisableUserRoleAsync(userId, roleId, ct);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    ///// <summary>
    ///// Upload ảnh lên Cloudinary --> link ảnh cho api sau
    ///// </summary>
    ///// <param name="file"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns></returns>
    //[HttpPost("upload-avatar")]
    //[Authorize]
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarDto request, CancellationToken cancellationToken)
    //{
    //    var file = request.File;

    //    if (file == null || file.Length == 0)
    //        return BadRequest("Không có file hợp lệ");

    //    var imageUrl = await _cloudinaryService.UploadImageAsync(file, cancellationToken);

    //    return Ok(ResponseModel<object>.OkResponseModel(
    //        data: imageUrl,
    //        message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUBMIT_SUCCESS, "feedback")
    //    ));
    //}

    /// <summary>
    /// Lấy link ảnh sau khi upload lên Cloudinary để cập nhật lại cho user
    /// </summary>
    /// <param name="mediaDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("update-avatar")]
    [Authorize]
    public async Task<IActionResult> UpdateAvatar(UploadMediaDto mediaDto, CancellationToken cancellationToken)
    {
        var result = await _userService.UploadAvatarAsync(mediaDto, cancellationToken);
        return Ok(ResponseModel<MediaResponse>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUBMIT_SUCCESS, "feedback")
        ));
    }

    /// <summary>
    /// Lấy tất cả user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(string? searchFullName = null, string? role = null)
    {
        var users = await _userService.GetAllUsersAsync(searchFullName, role);
        return Ok(ResponseModel<List<UserResponseModel>>.OkResponseModel(
            data: users,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Lấy user theo id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id, new CancellationToken());
        return Ok(ResponseModel<UserResponseModel>.OkResponseModel(
            data: user,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Cập nhật user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateModel model)
    {
        await _userService.UpdateUserAsync(id, model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Lấy danh sách user phân trang với điều kiện tìm kiếm
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Kích thước trang</param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="fullName"></param>
    /// <returns></returns>
    [HttpGet("search-paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedUserWithSearch(string? email, string? phoneNumber, string? fullName, int pageNumber = 1, int pageSize = 10)
    {
        var users = await _userService.GetPagedUsersAsync(email, phoneNumber, fullName, pageNumber, pageSize, new CancellationToken());
        return Ok(PagedResponseModel<object>.OkResponseModel(
            data: users.Items,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "user"),
            totalCount: users.TotalCount,
            pageSize: pageSize,
            pageNumber: pageNumber
        ));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
    {
        var result = await _userService.CreateUserAsync(model);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "user")
        ));
    }

    // [HttpPut("{userId}/role")]
    // public async Task<IActionResult> AssignModeratorRole(Guid userId, [FromBody] UpdateUserRoleDto model)
    // {
    //     var result = await _userService.AssignModeratorRoleAsync(userId, model);
    //     return Ok(ResponseModel<object>.OkResponseModel(
    //         data: result,
    //         message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user")
    //     ));
    // }

    /// <summary>
    /// tạo yêu cầu để nâng cấp role cho tour guide
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("tour-guide-request")]
    public async Task<IActionResult> CreateTourGuideRequest([FromBody] CreateTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateTourGuideRequestAsync(model, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user")
        ));
    }
    /// <summary>
    /// lấy tất cả các yêu cầu trở thành tour guide
    /// </summary>
    /// <param name="status"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("tour-guide-request")]
    public async Task<IActionResult> GetTourGuideRequests([FromQuery] TourGuideRequestStatus? status, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetTourGuideRequestsAsync(status, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// xem chi tiết yêu cầu trở thành tour guide (moderator)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("tour-guide-request/{id}")]
    public async Task<IActionResult> GetTourGuideRequestById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetTourGuideRequestByIdAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "tour guide request")
        ));
    }

    /// <summary>
    /// người dùng tự xem các yêu cầu trở thành tour guide mà mình đã gửi (user)
    /// </summary>
    /// <param name="status"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("my-tour-guide-requests")]
    public async Task<IActionResult> GetMyTourGuideRequests(
        [FromQuery] TourGuideRequestStatus? status,
        int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetMyTourGuideRequestsAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "my tour guide requests")
        ));
    }

    /// <summary>
    /// kiểm tra yêu cầu trở thành tour guide mới nhất (moderator)
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("latest-tour-guide-request")]
    public async Task<IActionResult> GetLatestTourGuideRequest(
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetLatestTourGuideRequestsAsync(userId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "latest tour guide request")
        ));
    }

    /// <summary>
    /// người dùng tự cập nhật yêu cầu trước khi được xử lý (user)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("tour-guide-request/{id}")]
    public async Task<IActionResult> UpdateTourGuideRequest(Guid id, [FromBody] UpdateTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateTourGuideRequestAsync(id, model, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "tour guide request")
        ));
    }

    /// <summary>
    /// người dùng xóa yêu cầu trước khi được xử lý (user)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("tour-guide-request/{id}")]
    public async Task<IActionResult> DeleteTourGuideRequest(Guid id, CancellationToken cancellationToken = default)
    {
        var success = await _userService.DeleteTourGuideRequestAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "tour guide request")
        ));
    }

    /// <summary>
    /// moderator review yêu cầu mà người dùng gửi 
    /// </summary>
    /// <remarks>
    /// Trạng thái yêu cầu của <see cref="TourGuideRequestStatus"/>:
    /// <list type="bullet">
    ///   <item>
    ///     <term>Pending (1)</term>
    ///     <description>Chờ xác nhận</description>
    ///   </item>
    ///   <item>
    ///     <term>Approved (2)</term>
    ///     <description>Đã xác nhận</description>
    ///   </item>
    ///   <item>
    ///     <term>Rejected (3)</term>
    ///     <description>Từ chối</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="requestId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{requestId}/review")]
    public async Task<IActionResult> ReviewTourGuideRequest(Guid requestId, [FromBody] ReviewTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ReviewTourGuideRequestAsync(requestId, model, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user")
        ));
    }
    /// <summary>
    /// tạo yêu cầu để trở thành làng nghề
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("craft-village-request")]
    public async Task<IActionResult> CreateCraftVillageRequest([FromBody] CreateCraftVillageRequestDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateCraftVillageRequestAsync(model, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Lấy yêu cầu trở thành tour guide (moderator)
    /// </summary>
    /// <param name="status"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("craft-village-request")]
    public async Task<IActionResult> GetCraftVillageRequests([FromQuery] CraftVillageRequestStatus? status, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetCraftVillageRequestsAsync(status, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// người dùng tự lấy yêu cầu của người dùng trở thành role làng nghề (user)
    /// </summary>
    /// <param name="status"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("my-craft-village-requests")]
    public async Task<IActionResult> GetMyCraftVillageRequests(
        [FromQuery] CraftVillageRequestStatus? status,
        int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetMyCraftVillageRequestsAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "my craft village requests")
        ));
    }

    /// <summary>
    /// moderator check yêu cầu mới nhất của người dùng
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("latest-craft-village-request/{userId:guid}")]
    public async Task<IActionResult> GetLatestCraftVillageRequest(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetLatestCraftVillageRequestsAsync(userId, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "latest craft village request")
        ));
    }

    /// <summary>
    /// moderator review yêu cầu trở thành làng nghề
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("craft-village-request/{requestId}/review")]
    public async Task<IActionResult> ReviewCraftVillageRequest(Guid requestId, [FromBody] ReviewCraftVillageRequestDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ReviewCraftVillageRequestAsync(requestId, model, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "craft village request")
        ));
    }

    /// <summary>
    /// lấy chi tiết yêu cầu trở thành làng nghề
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("craft-village-request/{id}")]
    public async Task<IActionResult> GetCraftVillageRequest(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetCraftVillageRequestAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "craft village request")
        ));
    }

    /// <summary>
    /// xóa yêu cầu trở thành làng nghề trước khi được xử lý
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("craft-village-request/{id}")]
    public async Task<IActionResult> DeleteCraftVillageRequest(Guid id, CancellationToken cancellationToken = default)
    {
        var success = await _userService.DeleteCraftVillageRequestAsync(id, cancellationToken);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: success,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.DELETE_SUCCESS, "craft village request")
        ));
    }

    // [HttpGet("{requestId}")]
    // public async Task<IActionResult> GetTourGuideRequest(Guid requestId, CancellationToken cancellationToken = default)
    // {
    //     var result = await _userService.GetTourGuideRequestsAsync(requestId, cancellationToken); // Giả định có phương thức này
    //     if (result == null)
    //     {
    //         return NotFound(new { Message = "TourGuide request not found" });
    //     }
    //     return Ok(result);
    // }
}
