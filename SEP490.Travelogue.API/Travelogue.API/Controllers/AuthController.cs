using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travelogue.API.Models.RequestModels;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IAuthService authService, IGoogleAuthService googleAuthService)
    {
        _authService = authService;
        _googleAuthService = googleAuthService;
    }

    [HttpGet("check-deploy")]
    public async Task<IActionResult> CheckDeploy(string email)
    {
        try
        {

            //await _userService.UpdateEmailVerifiedStatusAsync(userRecord.Uid, true);
            return Ok(new { message = "13:20 30/7" });

        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    /// <summary>
    /// Kiểm tra trạng thái xác minh email của người dùng.
    /// </summary>
    /// <param name="email">Email của người dùng cần kiểm tra.</param>
    /// <returns>Thông báo xác minh email.</returns>
    [HttpGet("verify-email-status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public async Task<IActionResult> VerifyEmailStatus(string email)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            if (userRecord.EmailVerified)
            {
                //await _userService.UpdateEmailVerifiedStatusAsync(userRecord.Uid, true);
                return Ok(new { message = "Email verified successfully." });
            }
            else
            {
                return BadRequest(new { message = "Email not verified yet." });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Đăng ký người dùng mới.
    /// </summary>
    /// <param name="model">Thông tin đăng ký của người dùng.</param>
    /// <returns>Trạng thái đăng ký.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<bool>))]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _authService.RegisterAsync(model);
        return Ok(ResponseModel<LoginResponse>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Đăng ký người dùng mới với role tự chọn.
    /// </summary>
    /// <param name="model">Thông tin đăng ký của người dùng.</param>
    /// <returns>Trạng thái đăng ký.</returns>
    [HttpPost("register-with-role")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<bool>))]
    public async Task<IActionResult> RegisterWithRole([FromBody] RegisterModelWithRole model)
    {
        var result = await _authService.RegisterWithRoleAsync(model);
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Đăng nhập bằng Google.
    /// </summary>
    /// <param name="request">Thông tin đăng nhập Google của người dùng.</param>
    /// <returns>Trạng thái xác thực người dùng.</returns>
    [HttpPost("google-login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<LoginResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(object))]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var loginResponse = await _authService.LoginWithGoogleAsync(request.Token);

            return Ok(ResponseModel<LoginResponse>.OkResponseModel(
                data: loginResponse,
                message: $"{ResponseMessages.SUCCESS})"
            ));
        }
        catch (CustomException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = "Invalid token", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại.
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-current-user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<GetCurrentUserResponse>))]
    [Authorize]
    public async Task<ActionResult<GetCurrentUserResponse>> GetLoggedUser()
    {
        var response = await _authService.GetCurrentUser();
        return Ok(ResponseModel<GetCurrentUserResponse>.OkResponseModel(
            data: response,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.GET_SUCCESS, "user")
        ));
    }

    /// <summary>
    /// Đăng nhập người dùng.
    /// </summary>
    /// <param name="model">Thông tin đăng nhập của người dùng.</param>
    /// <param name="cancellationToken">Token để hủy bỏ yêu cầu nếu cần.</param>
    /// <returns>Kết quả đăng nhập.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<LoginResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(model);
        return Ok(ResponseModel<LoginResponse>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestModel request)
    {
        var result = await _authService.RefreshTokenAsync(request.Token);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    //[HttpPost("check-token")]
    //public async Task<IActionResult> CheckToken([FromBody] TokenRequestModel request)
    //{
    //    var result = await _authService.CheckTokenAsync(request.Token);
    //    return Ok(ResponseModel<object>.OkResponseModel(
    //        data: result,
    //        message: ResponseMessages.SUCCESS
    //    ));
    //}

    [HttpPost("check-token")]
    public async Task<IActionResult> CheckToken()
    {
        var result = await _authService.CheckTokenAsync();
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    /// <summary>
    /// Yêu cầu quên mật khẩu.
    /// </summary>
    /// <param name="model">Thông tin yêu cầu quên mật khẩu của người dùng.</param>
    /// <param name="cancellationToken">Token để hủy bỏ yêu cầu nếu cần.</param>
    /// <returns>Kết quả gửi yêu cầu quên mật khẩu.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel<bool>))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model, CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPassword(model, cancellationToken);
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUBMIT_SUCCESS, "mail")
        ));
    }

    /// <summary>
    /// Kiểm tra mã code hợp lệ để lấy lại mật khẩu.
    /// </summary>
    /// <param name="model">Thông tin mã code cần kiểm tra.</param>
    /// <returns>Trạng thái hợp lệ của mã code.</returns>
    [HttpPost("check-valid-code")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseModel<bool>))]
    public async Task<IActionResult> CheckValidCode([FromBody] ResetTokenModel model)
    {
        var isValid = await _authService.VerifyResetToken(model, new CancellationToken());
        return Ok(ResponseModel<bool>.OkResponseModel(
            data: isValid,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.SUCCESS)
        ));
    }

    /// <summary>
    /// Đặt lại mật khẩu sau khi gửi mail.
    /// </summary>
    /// <param name="model">Thông tin đặt lại mật khẩu của người dùng.</param>
    /// <returns>Kết quả đặt lại mật khẩu.</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseModel<LoginResponse>))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        var response = await _authService.ResetPassword(model, new CancellationToken());
        return Ok(ResponseModel<LoginResponse>.OkResponseModel(
            data: response,
            message: ResponseMessages.SUCCESS
        ));
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    /// /// <param name="model">Thông tin đặt lại mật khẩu của người dùng.</param>
    /// <returns>Kết quả đặt lại mật khẩu.</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseModel<LoginResponse>))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var response = await _authService.ChangePasswordAsync(model, new CancellationToken());
        return Ok(ResponseModel<LoginResponse>.OkResponseModel(
            data: response,
            message: ResponseMessages.SUCCESS
        ));
    }

    /// <summary>
    /// Xác thực email
    /// </summary>
    /// <param name="token">Token của người dùng</param>
    /// <returns>Thất bại hay thành công</returns>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        bool result = await _authService.VerifyEmailAsync(token);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    /// <summary>
    /// Gửi lại mail xác thực email
    /// </summary>
    /// <param name="email">Email của người dùng</param>
    /// <returns>Thất bại hay thành công</returns>
    [HttpGet("resend-email-verification")]
    public async Task<IActionResult> ResendEmailVerification([FromQuery] string email)
    {
        bool result = await _authService.ResendEmailVerificationAsync(email, new CancellationToken());
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessages.SUCCESS
        ));
    }

    //[HttpGet("verify-email")]
    //public async Task<IActionResult> VerifyEmail(string oobCode)
    //{
    //    if (string.IsNullOrEmpty(oobCode))
    //    {
    //        return BadRequest("Invalid verification code.");
    //    }

    //    var response = await _authService.VerifyEmailByOOBCode(oobCode, new CancellationToken());
    //    return Ok(ResponseModel<bool>.OkResponseModel(
    //        data: response,
    //        message: "Email verified successfully."
    //    ));
    //}

    //[HttpPost("change-password")]
    //public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
    //{
    //    if (string.IsNullOrEmpty(model.IdToken) || string.IsNullOrEmpty(model.NewPassword))
    //    {
    //        return BadRequest("Invalid request data.");
    //    }

    //    var result = await _authService.ChangePasswordAsync(model.IdToken, model.NewPassword);
    //    if (result)
    //    {
    //        return Ok(ResponseModel<bool>.OkResponseModel(
    //            data: result,
    //            message: "Password changed successfully."
    //        ));
    //    }
    //    else
    //    {
    //        return BadRequest("Failed to change password.");
    //    }
    //}
}