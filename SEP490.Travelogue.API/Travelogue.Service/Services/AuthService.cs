using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.Const;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IAuthService
{
    Task<GetCurrentUserResponse> GetCurrentUser();
    Task<GetCurrentUserResponse> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<LoginResponse> RegisterAsync(RegisterModel userRequestModel);
    Task<bool> RegisterWithRoleAsync(RegisterModelWithRole userRequestModel);
    Task<LoginResponse> LoginAsync(LoginModel loginModel);
    Task<bool> ForgotPassword(ForgotPasswordModel request, CancellationToken cancellationToken);
    Task<bool> VerifyResetToken(ResetTokenModel model, CancellationToken cancellationToken);
    Task<LoginResponse> ResetPassword(ResetPasswordModel model, CancellationToken cancellationToken);
    Task<LoginResponse> LoginWithGoogleAsync(string token);
    string GenerateJwtToken(Guid userId, List<string> roles, bool isRefreshToken);
    Task<LoginResponse> ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken);
    Task<bool> VerifyEmailByOOBCode(string oob, CancellationToken cancellationToken);
    Task<bool> VerifyEmailAsync(string email);
    Task<bool> ResendEmailVerificationAsync(string email, CancellationToken cancellationToken);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> CheckTokenAsync(string refreshToken);
    Task<CheckTokenResponse> CheckTokenAsync();
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _timeService;
    private readonly IConfiguration _configuration;
    private readonly int _exAccessToken;
    private readonly int _exRefreshToken;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly FirebaseAuth _firebaseAuth;
    private readonly IHttpContextAccessor _httpContextAccessor;

    //private readonly UserManager<User> _userManager;
    //private readonly RoleManager<IdentityRole> _roleManager;

    //public AuthService(IUnitOfWork unitOfWork, ITimeService timeService, UserManager<User> userManager,
    //    RoleManager<IdentityRole> roleManager, IConfiguration configuration,
    //    IEmailService emailService, IMapper mapper, IUserContextService userContextService, FirebaseAuth firebaseAuth)
    //{
    //    _unitOfWork = unitOfWork;
    //    _timeService = timeService;
    //    _userManager = userManager;
    //    _roleManager = roleManager;
    //    _configuration = configuration;
    //    _exAccessToken = int.Parse(_configuration["JwtSettings:ExpirationAccessToken"]!);
    //    _exRefreshToken = int.Parse(_configuration["JwtSettings:ExpirationRefreshToken"]!);
    //    _emailService = emailService;
    //    _mapper = mapper;
    //    _userContextService = userContextService;
    //    _firebaseAuth = firebaseAuth;
    //}

    public AuthService(
        IUnitOfWork unitOfWork,
        ITimeService timeService,
        IConfiguration configuration,
       IEmailService emailService,
       IMapper mapper,
       IUserContextService userContextService,
       FirebaseAuth firebaseAuth,
       IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _timeService = timeService;
        _configuration = configuration;
        _exAccessToken = int.Parse(_configuration["JwtSettings:ExpirationAccessToken"]!);
        _exRefreshToken = int.Parse(_configuration["JwtSettings:ExpirationRefreshToken"]!);
        _emailService = emailService;
        _mapper = mapper;
        _userContextService = userContextService;
        _firebaseAuth = firebaseAuth;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetCurrentUserResponse> GetCurrentUser()
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            //Guid? currentUserIdLong = string.IsNullOrEmpty(currentUserId) ? null : Convert.ToInt64(currentUserId);
            var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(currentUserId!), new CancellationToken());
            return _mapper.Map<GetCurrentUserResponse>(currentUser);
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

    public async Task<LoginResponse> RegisterAsync(RegisterModel model)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        string? firebaseUid = null;

        try
        {
            // Kiểm tra người dùng đã tồn tại chưa
            if (await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email) != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError($"{ResponseMessages.EXISTED.Replace("{0}", "người dùng")}");
            }

            // Tạo mới role "user" nếu chưa có
            Role? userRole = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.USER);
            if (userRole == null)
            {
                userRole = new Role(AppRole.USER)
                {
                    CreatedBy = "system",
                    LastUpdatedBy = "system"
                };

                var roleResult = await _unitOfWork.RoleRepository.AddAsync(userRole);
                if (roleResult == null)
                {
                    throw CustomExceptionFactory.CreateInternalServerError("Thêm Role thất bại");
                }
            }

            // Kiểm tra mật khẩu xác nhận
            if (model.Password != model.ConfirmPassword)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Vui lòng nhập lại mật khẩu chính xác");
            }

            // Khởi tạo User
            var newUser = _mapper.Map<User>(model);
            newUser.IsActive = true;
            newUser.IsDeleted = false;

            // tạo user
            User? resultCreateUser = await _unitOfWork.UserRepository.CreateUser(newUser, model.Password);
            if (resultCreateUser == null)
            {
                throw CustomExceptionFactory.CreateInternalServerError("Tạo người dùng thất bại");
            }

            resultCreateUser.CreatedBy = resultCreateUser.Id.ToString();
            resultCreateUser.LastUpdatedBy = resultCreateUser.Id.ToString();
            await _unitOfWork.UserRepository.UpdateAsync(resultCreateUser);

            // Thêm user vào role
            bool resultAddRole = await _unitOfWork.UserRepository.AddToRoleAsync(newUser, userRole.Id);
            if (!resultAddRole)
            {
                throw CustomExceptionFactory.CreateInternalServerError("Thêm role cho người dùng thất bại");
            }

            // Đăng ký tài khoản với Firebase
            var userRecordArgs = new UserRecordArgs()
            {
                Email = model.Email,
                EmailVerified = false,
                Password = model.Password,
                DisplayName = model.FullName,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
            firebaseUid = userRecord.Uid;

            await transaction.CommitAsync();

            //Đăng nhập
            LoginResponse response = await LoginAsync(new LoginModel
            {
                Email = model.Email,
                Password = model.Password
            });

            // Tạo link xác thực email
            string domainFrontend = _configuration["Domain:UrlFrontEnd"];
            //var actionCodeSettings = new ActionCodeSettings()
            //{
            //    //Url = $"{domainBackend}/api/auth/verify-email?email={model.Email}",
            //    Url = $"{domainFrontend}/auth/verify/{response.VerificationToken}",
            //    HandleCodeInApp = false
            //};

            var verificationLink = $"{domainFrontend}/auth/verify/{response.VerificationToken}";
            var selectedEmail = new List<string> { model.Email };
            var mailModel = new
            {
                UserName = newUser.FullName,
                VerificationLink = verificationLink
            };

            // Gửi email xác thực
            await _emailService.SendEmailWithTemplateAsync(
                selectedEmail,
                "Xác minh đăng ký Goyoung Tây Ninh",
                MailTemplateLinks.VerifyAccountMailTemplate,
                mailModel);

            return response;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            if (firebaseUid != null)
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid); // Rollback Firebase nếu có lỗi
            }
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            if (firebaseUid != null)
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid); // Rollback Firebase nếu có lỗi
            }
            throw CustomExceptionFactory.CreateInternalServerError(ex.ToString());
        }
    }

    public ClaimsPrincipal DecodeJWT(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (handler.CanReadToken(token))
        {
            // Đọc và kiểm tra JWT token
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims);
            return new ClaimsPrincipal(identity);
        }

        throw new SecurityTokenException("Invalid token");
    }

    public async Task<bool> ResendEmailVerificationAsync(string email, CancellationToken cancellationToken)
    {

        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email) ??
                throw CustomExceptionFactory.CreateNotFoundError("người dùng");
            // Tạo link xác thực email
            string domainFrontend = _configuration["Domain:UrlFrontEnd"];
            var actionCodeSettings = new ActionCodeSettings()
            {
                //Url = $"{domainBackend}/api/auth/verify-email?email={model.Email}",
                Url = $"{domainFrontend}/auth/verify/{user.VerificationToken}",
                HandleCodeInApp = false
            };

            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(user.Email, actionCodeSettings);
            var selectedEmail = new List<string> { user.Email };
            var mailModel = new
            {
                UserName = user.FullName,
                VerificationLink = verificationLink
            };

            // Gửi email xác thực
            await _emailService.SendEmailWithTemplateAsync(
                selectedEmail,
                "Xác minh đăng ký Goyoung Tây Ninh",
                MailTemplateLinks.VerifyAccountMailTemplate,
                mailModel);
            return true;
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

    public async Task<bool> VerifyEmailAsync(string token)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            Guid id = Guid.Parse(DecodeJWT(token).FindFirst("Id")?.Value ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Token không hợp lệ"));

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Người dùng không tồn tại.");
            }

            if (user.IsEmailVerified ?? false)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tài khoản đã được xác thực trước đó.");
            }

            user.IsEmailVerified = true;
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> RegisterWithRoleAsync(RegisterModelWithRole model)
    {
        try
        {
            // Bussiness Logic: user login with their email ==> userName = email field
            // Find the Email string in the User Name field in the database 
            if ((await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email)) != null)
                throw CustomExceptionFactory.CreateBadRequestError($"{ResponseMessages.EXISTED.Replace("{0}", "người dùng")}");

            string roleId = model.RoleId?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("RoleId cannot be empty.");
            }

            Role? role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId, new CancellationToken());
            if (role == null)
                throw CustomExceptionFactory.CreateNotFoundError("role");

            if (model.Password != model.ConfirmPassword)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Vui lòng nhập lại mật khẩu chính xác");
            }
            var newUser = _mapper.Map<User>(model);
            newUser.IsActive = true;
            newUser.IsDeleted = false;

            User? resultCreateUser = await _unitOfWork.UserRepository.CreateUser(newUser, model.Password);

            bool resultAddRole = await _unitOfWork.UserRepository.AddToRoleAsync(newUser, role.Id!);

            var userRecordArgs = new UserRecordArgs()
            {
                Email = model.Email,
                EmailVerified = false,
                Password = model.Password,
                DisplayName = model.FullName,
            };
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

            // Verify nguoi dung
            var actionCodeSettings = new ActionCodeSettings()
            {
                //Url = "http://143.198.206.133:8888/api/auth/verify-email",
                Url = "https://goyoungtayninh.netlify.app/",
                HandleCodeInApp = false
            };

            // Gửi email xác thực bằng Firebase Admin SDK
            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(model.Email, actionCodeSettings);

            var selectedEmail = new List<string> { model.Email };
            var maiModel = new
            {
                UserName = newUser.FullName,
                VerificationLink = verificationLink
            };

            await _emailService.SendEmailWithTemplateAsync(
                selectedEmail,
                "VERIFY YOUR EMAIL",
                MailTemplateLinks.VerifyAccountMailTemplate,
                maiModel);

            return true;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> VerifyResetToken(ResetTokenModel model, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email) ??
                throw CustomExceptionFactory.CreateNotFoundError("người dùng");
            //var verificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.EmailCode, model.Token);
            //return verificationResult != PasswordVerificationResult.Failed;

            var emailCode = await _unitOfWork.PasswordResetTokenRepository.GetValidTokenAsync(model.Email, model.Token);

            if (emailCode == null)
            {
                return false;
            }

            return true;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> ForgotPassword(ForgotPasswordModel request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) ??
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy người dùng");
            //var code = new Random().Next(1000, 9999).ToString();
            //var codeHash = _userManager.PasswordHasher.HashPassword(user, code);

            //var token = new Random().Next(1000, 9999).ToString();
            var token = new Random().Next(1000, 9999).ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(30);

            //user.EmailCode = token;
            //await _unitOfWork.UserRepository.UpdateAsync(user);

            var resetToken = new PasswordResetToken
            {
                Email = request.Email,
                Token = token,
                ExpiryTime = expiryTime,
                IsUsed = false
            };
            await _unitOfWork.PasswordResetTokenRepository.AddAsync(resetToken);

            var selectedEmail = new List<string> { request.Email };
            // await _emailService.SendEmailAsync(selectedEmail, "TRAVELOUGE RESET PASSWORK TOKEN", $"Your reset token is: {code}");
            var model = new
            {
                UserName = user.FullName,
                ResetCode = token
            };

            await _emailService.SendEmailWithTemplateAsync(
                selectedEmail,
                "MÃ ĐỂ ĐẶT LẠI MẬT KHẨU GO YOUNG TÂY NINH",
                MailTemplateLinks.CodeForResetPasswordMailTemplate,
                model);

            return true;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LoginResponse> ResetPassword(ResetPasswordModel model, CancellationToken cancellationToken)
    {
        try
        {
            var resetToken = await _unitOfWork.PasswordResetTokenRepository.GetValidTokenAsync(model.Email, model.Token);
            if (resetToken == null || resetToken.ExpiryTime < DateTime.UtcNow || resetToken.IsUsed)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Token không hợp lệ hoặc đã hết hạn.");
            }

            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("nguời dùng");
            }

            //if (user.EmailCode != model.Token)
            //{
            //    throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_TOKEN, "Mã xác thực không hợp lệ.");
            //}

            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "Mật khẩu nhập lại không đúng.");
            }

            user.SetPassword(model.NewPassword);
            user.EmailCode = null;

            await _unitOfWork.UserRepository.UpdateAsync(user);

            resetToken.IsUsed = true;
            _unitOfWork.PasswordResetTokenRepository.Update(resetToken);

            var selectedEmail = new List<string> { model.Email };
            var time = _timeService.SystemTimeNow.ToString("yyyy-MM-dd HH:mm:ss");
            await _emailService.SendEmailAsync(selectedEmail, "THAY ĐỔI MẬT KHẨU THÀNH CÔNG", $"Your password has changed successfully at: {time}");

            //Thay đổi token mới
            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            var roleNames = roles.Select(r => r.Name).ToList();
            var accessToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
            var refreshToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

            // Cập nhật lại thời gian hết hạn
            user.VerificationToken = accessToken;
            user.ResetToken = refreshToken;
            user.VerificationTokenExpires = _timeService.SystemTimeNow.AddDays(_exAccessToken);
            user.ResetTokenExpires = _timeService.SystemTimeNow.AddDays(_exRefreshToken);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return new LoginResponse
            {
                VerificationToken = accessToken,
                RefreshTokens = refreshToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginModel.Email) ??
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tên đăng nhập hoặc mật khẩu không đúng");

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Tài khoản của bạn đã bị khóa.");

            // Kiểm tra trạng thái xác minh email từ Firebase
            //var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(loginModel.Email);
            //if (!firebaseUser.EmailVerified)
            //{
            //    throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Tài khoản của bạn chưa được xác thực.");
            //}
            //user.IsEmailVerified = true;

            //if (!(user.IsEmailVerified ?? false))

            var checkPassword = user.PasswordHash != null && user.PasswordSalt != null
                ? await _unitOfWork.UserRepository.CheckPassword(loginModel.Password, user.PasswordHash, user.PasswordSalt)
                : false;
            if (!checkPassword)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Tên đăng nhập hoặc mật khẩu không đúng");
            }

            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            var roleNames = roles.Select(r => r.Name).ToList();

            // Kiểm tra nếu token còn hạn
            string accessToken;
            string refreshToken;

            if (user.VerificationTokenExpires > DateTimeOffset.UtcNow)
            {
                // Nếu token còn hạn, cấp lại token cũ
                accessToken = user.VerificationToken;
                refreshToken = user.ResetToken;
            }
            else
            {
                // Nếu token hết hạn, cấp token mới
                accessToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
                refreshToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

                // Cập nhật lại thời gian hết hạn
                user.VerificationToken = accessToken;
                user.ResetToken = refreshToken;
                user.VerificationTokenExpires = _timeService.SystemTimeNow.AddDays(_exAccessToken);
                user.ResetTokenExpires = _timeService.SystemTimeNow.AddDays(_exRefreshToken);
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
            }

            return new LoginResponse
            {
                VerificationToken = accessToken,
                RefreshTokens = refreshToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var claimsPrincipal = DecodeJWT(refreshToken);

            var userIdClaim = claimsPrincipal.FindFirst("Id");
            if (userIdClaim == null)
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Token không hợp lệ");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, new CancellationToken());
            if (user == null)
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user.ResetToken != refreshToken)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Refresh token không hợp lệ");
            }

            if (user.ResetTokenExpires < DateTimeOffset.UtcNow)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Refresh token đã hết hạn");
            }

            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null || !roles.Any())
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Người dùng chưa được cấp quyền");

            var roleNames = roles.Select(r => r.Name).ToList();
            var newAccessToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
            var newRefreshToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

            user.VerificationToken = newAccessToken;
            user.ResetToken = newRefreshToken;
            user.VerificationTokenExpires = _timeService.SystemTimeNow.AddDays(_exAccessToken);
            user.ResetTokenExpires = _timeService.SystemTimeNow.AddDays(_exRefreshToken);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await transaction.CommitAsync();

            return new LoginResponse
            {
                VerificationToken = newAccessToken,
                RefreshTokens = newRefreshToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> CheckTokenAsync(string token)
    {
        try
        {
            var claimsPrincipal = DecodeJWT(token);

            var userIdClaim = claimsPrincipal.FindFirst("Id");
            if (userIdClaim == null)
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Verification không hợp lệ");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, new CancellationToken());
            if (user == null)
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user.VerificationToken != token || user.ResetTokenExpires < DateTimeOffset.UtcNow)
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Refresh token không hợp lệ hoặc đã hết hạn");

            return true;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CheckTokenResponse> CheckTokenAsync()
    {
        try
        {
            var token = _userContextService.GetUserToken();
            var userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, new CancellationToken());
            var response = new CheckTokenResponse();
            if (user == null)
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user.VerificationToken != token)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Verification token không hợp lệ");
            }

            if (user.VerificationTokenExpires < DateTimeOffset.UtcNow)
            {
                response.Data = false;
                response.Message = "Token hết hạn";
                return response;
            }

            response.Data = true;
            response.Message = "Token còn hạn";

            return response;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    // Đăng nhập với Google
    public async Task<LoginResponse> LoginWithGoogleAsync(string token)
    {
        try
        {
            // Xác thực token từ Firebase
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            string uid = decodedToken.Uid;

            // Lấy email và tên đầy đủ từ token
            string? email = decodedToken.Claims["email"]?.ToString();
            string fullName = decodedToken.Claims["name"]?.ToString() ?? "User";
            if (email == null)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "UNAUTHORIZED");
            }

            var role = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.USER);

            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = await CreateNewUserAsync(email, uid, fullName, role.Id);
            }
            else
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.FORBIDDEN, "Tài khoản của bạn đã bị khóa.");
                }
            }

            // Lấy role của người dùng
            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "User has no assigned role");
            }

            var roleNames = roles.Select(r => r.Name).ToList();

            var accessToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
            var refreshToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

            user.VerificationToken = accessToken;
            user.ResetToken = refreshToken;
            user.VerificationTokenExpires = _timeService.SystemTimeNow.AddDays(_exAccessToken);
            user.ResetTokenExpires = _timeService.SystemTimeNow.AddDays(_exRefreshToken);
            await _unitOfWork.UserRepository.UpdateAsync(user);

            return new LoginResponse
            {
                VerificationToken = accessToken,
                RefreshTokens = refreshToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };
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

    // Additional function
    // Tạo mới User
    private async Task<User> CreateNewUserAsync(string email, string googleId, string fullName, Guid roleId)
    {
        try
        {
            var newUser = new User
            {
                Email = email,
                GoogleId = googleId,
                FullName = fullName,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = "system",
                LastUpdatedBy = "system",
                CreatedTime = _timeService.SystemTimeNow,
                LastUpdatedTime = _timeService.SystemTimeNow
            };

            User resultCreateUser = await _unitOfWork.UserRepository.AddAsync(newUser);

            // Gán quyền 
            bool resultAddRole = await _unitOfWork.UserRepository.AddToRoleAsync(newUser, roleId);

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

    public string GenerateJwtToken(Guid userId, List<string> roles, bool isRefreshToken)
    {
        try
        {
            var keyString = _configuration["JwtSettings:SecretKey"]
                           ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("Id", userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                //new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            if (isRefreshToken)
            {
                claims.Add(new Claim("isRefreshToken", "true"));
            }

            DateTime expiresDateTime;
            if (isRefreshToken)
            {
                expiresDateTime = _timeService.SystemTimeNow.AddDays(_exRefreshToken).DateTime;
            }
            else
            {
                expiresDateTime = _timeService.SystemTimeNow.AddDays(_exAccessToken).DateTime;
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiresDateTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

    public async Task<LoginResponse> ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("nguời dùng");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "Mật khẩu nhập lại không đúng.");
            }

            user.SetPassword(model.NewPassword);
            user.EmailCode = null;

            await _unitOfWork.UserRepository.UpdateAsync(user);

            var selectedEmail = new List<string> { model.Email };
            var time = _timeService.SystemTimeNow.ToString("yyyy-MM-dd HH:mm:ss");
            await _emailService.SendEmailAsync(selectedEmail, "THAY ĐỔI MẬT KHẨU THÀNH CÔNG", $"Your password has changed successfully at: {time}");

            //Thay đổi token mới
            var roles = await _unitOfWork.UserRepository.GetRolesAsync(user);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            var roleNames = roles.Select(r => r.Name).ToList();
            var accessToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: false);
            var refreshToken = GenerateJwtToken(user.Id, roleNames, isRefreshToken: true);

            // Cập nhật lại thời gian hết hạn
            user.VerificationToken = accessToken;
            user.ResetToken = refreshToken;
            user.VerificationTokenExpires = _timeService.SystemTimeNow.AddDays(_exAccessToken);
            user.ResetTokenExpires = _timeService.SystemTimeNow.AddDays(_exRefreshToken);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return new LoginResponse
            {
                VerificationToken = accessToken,
                RefreshTokens = refreshToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                IsEmailVerified = user.IsEmailVerified ?? false,
                Roles = roleNames
            };
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

    public async Task<bool> VerifyEmailByOOBCode(string oobCode, CancellationToken cancellationToken)
    {
        try
        {
            FirebaseToken token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(oobCode, cancellationToken);

            if (token != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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

    public async Task<GetCurrentUserResponse> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);

            return _mapper.Map<GetCurrentUserResponse>(user);
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
