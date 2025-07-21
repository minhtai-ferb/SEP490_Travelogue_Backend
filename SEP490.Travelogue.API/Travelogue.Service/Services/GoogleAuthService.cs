using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.Const;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IGoogleAuthService
{
    Task<LoginResponse> HandleGoogleLoginAsync();
}

public class GoogleAuthService : IGoogleAuthService
{
    //private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITimeService _timeService;

    public GoogleAuthService(IAuthService authService, IHttpContextAccessor httpContextAccessor, ITimeService timeService, IUnitOfWork unitOfWork)
    {
        //_userManager = userManager;
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
        _timeService = timeService;
        this._unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> HandleGoogleLoginAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xác thực người dùng từ Google. Vui lòng thử lại.");
            }

            var claims = result.Principal.Identities.First().Claims;
            var userInfo = new
            {
                Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                GoogleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            };

            if (string.IsNullOrEmpty(userInfo.Name) || string.IsNullOrEmpty(userInfo.Email) || string.IsNullOrEmpty(userInfo.GoogleId))
            {
                throw new ArgumentException("Invalid user info from claims.");
            }

            // Tìm hoặc tạo người dùng mới
            var user = await FindOrCreateUserAsync(userInfo.Name, userInfo.Email, userInfo.GoogleId);

            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            var roleNames = roles.Select(r => r.Name).ToList();

            // Tạo token đăng nhập
            var tokens = _authService.GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
            var refreshTokens = _authService.GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

            return new LoginResponse
            {
                VerificationToken = tokens,
                RefreshTokens = refreshTokens,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };

            #region DO NOT OPEN THIS 
            //var existingUser = await _userManager.FindByEmailAsync(userInfo.Email);
            //User newUser = existingUser;

            //if (existingUser is null)
            //{
            //    newUser = new User
            //    {
            //        FullName = userInfo.Name,
            //        Email = userInfo.Email,
            //        UserName = userInfo.Email,
            //        GoogleId = userInfo.GoogleId,
            //        CreatedTime = DateTimeOffset.UtcNow,
            //        LastUpdatedTime = DateTimeOffset.UtcNow,
            //        CreatedBy = "google",
            //        LastUpdatedBy = "google",
            //        IsActive = true,
            //        IsDeleted = false
            //    };
            //    // HARD CODE
            //    var userRole = "1";

            //    // Generate access token
            //    var accessToken = _authService.GenerateJwtToken(newUser.Id, userRole, isRefreshToken: false);
            //    // Generate refresh token
            //    var refreshToken = _authService.GenerateJwtToken(newUser.Id, userRole, isRefreshToken: true);

            //    // Save Database
            //    newUser.VerificationToken = accessToken;
            //    newUser.ResetToken = refreshToken;
            //    newUser.VerificationTokenExpires = _timeService.SystemTimeNow.AddHours(_exRefreshToken);
            //    newUser.VerificationTokenExpires = _timeService.SystemTimeNow.AddHours(_exRefreshToken);
            //    newUser.ResetTokenExpires = _timeService.SystemTimeNow.AddMinutes(_exAccessToken);
            //    var resultCreateUser = await _userManager.CreateAsync(newUser);
            //    if (!resultCreateUser.Succeeded)
            //        return new LoginResponse
            //        {
            //            VerificationToken = accessToken,
            //            ResetToken = refreshToken,
            //            UserId = newUser.Id,
            //            Username = newUser.UserName,
            //            FullName = newUser.FullName,
            //            Email = newUser.Email,
            //            Role = userRole
            //        };
            //}
            ////else
            ////{
            ////    existingUser.LastUpdatedTime = DateTimeOffset.UtcNow;
            ////    await _userManager.UpdateAsync(existingUser);
            ////}

            //return new LoginResponse
            //{
            //    VerificationToken = accessToken,
            //    ResetToken = refreshToken,
            //    UserId = newUser.Id,
            //    Username = newUser.UserName,
            //    FullName = newUser.FullName,
            //    Email = newUser.Email,
            //    Role = userRole
            //};
            #endregion
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    private async Task<User> FindOrCreateUserAsync(string userName, string userEmail, string userGoogleId)
    {
        try
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(userEmail);
            if (existingUser != null)
            {
                existingUser.LastUpdatedTime = _timeService.SystemTimeNow;
                await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                return existingUser;
            }

            var newUser = new User
            {
                FullName = userName,
                Email = userEmail,
                GoogleId = userGoogleId,
                CreatedTime = _timeService.SystemTimeNow,
                LastUpdatedTime = _timeService.SystemTimeNow,
                CreatedBy = "google",
                LastUpdatedBy = "google",
                IsActive = true,
                IsDeleted = false
            };

            var result = await _unitOfWork.UserRepository.AddAsync(newUser);

            bool resultAddRole = await _unitOfWork.UserRepository.AddToRoleAsync(newUser, AppRole.USER);
            if (!resultAddRole)
                throw CustomExceptionFactory.CreateBadRequestError("Thêm role thất bại");
            //if (!result.Succeeded)
            //{
            //    throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
            //}

            return newUser;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }
}