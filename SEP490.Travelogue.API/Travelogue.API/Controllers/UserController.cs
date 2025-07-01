using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
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
    [HttpPost("update-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(Guid userId, [FromBody] UserRoleUpdateModel request)
    {
        var result = await _userService.UpdateUserRolesAsync(userId, request.RoleIds);
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.UPDATE_SUCCESS, "user - role")
        ));
    }

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

    [HttpPost("send-feedback")]
    public async Task<IActionResult> SendFeedback([FromBody] FeedbackModel model)
    {
        await _userService.SendFeedbackAsync(model, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: true,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUBMIT_SUCCESS, "feedback")
        ));
    }

    /// <summary>
    /// Lấy tất cả user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
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
}
