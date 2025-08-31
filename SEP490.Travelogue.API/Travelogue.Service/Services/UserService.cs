using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BankAccountModels;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.TransactionModels;
using Travelogue.Service.BusinessModels.UserModels;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.BusinessModels.WorkshopModels;
using Travelogue.Service.Commons.Const;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IUserService
{
    Task<UserResponseModel> CreateUserAsync(CreateUserDto model, CancellationToken cancellationToken = default);
    Task<UserResponseModel> AssignModeratorRoleAsync(Guid userId, UpdateUserRoleDto model, CancellationToken cancellationToken = default);
    Task<UserResponseModel> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<UserResponseModel>> GetAllUsersAsync(string? searchFullName = null, string? role = null, CancellationToken cancellationToken = default);
    Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(string? email, string? phoneNumber, string? fullName, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> BlockUserAsync(Guid userId);
    Task<bool> UnblockUserAsync(Guid userId);
    Task<bool> UpdateUserRolesAsync(Guid userId, List<Guid> roleIds);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid districtId, string roleName);
    Task<UserResponseModel> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken);
    Task<MediaResponse> UploadAvatarAsync(UploadMediaDto uploadMediaDto, CancellationToken cancellationToken);
    Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<bool> SendFeedbackAsync(FeedbackModel model, CancellationToken cancellationToken);
    Task<UserManageResponse> GetUserManageAsync(Guid userId, CancellationToken ct = default);
    Task<bool> EnableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<bool> DisableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<TourGuideRequestResponseDto> CreateTourGuideRequestAsync(CreateTourGuideRequestDto model, CancellationToken cancellationToken = default);
    Task<List<TourGuideRequestResponseDto>> GetTourGuideRequestsAsync(TourGuideRequestStatus? status, CancellationToken cancellationToken = default);
    Task<TourGuideRequestResponseDto> ReviewTourGuideRequestAsync(Guid requestId, ReviewTourGuideRequestDto model, CancellationToken cancellationToken = default);
    Task<TourGuideRequestResponseDto> GetTourGuideRequestByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TourGuideRequestResponseDto> UpdateTourGuideRequestAsync(Guid id, UpdateTourGuideRequestDto model, CancellationToken cancellationToken = default);
    Task<bool> DeleteTourGuideRequestAsync(Guid id, CancellationToken cancellationToken = default);
    //Task GetPagedUsersWithSearchAsync(int pageNumber, int pageSize, string email, string phoneNumber, string fullName, CancellationToken cancellationToken);
    Task<CraftVillageRequestResponseDto> CreateCraftVillageRequestAsync(CreateCraftVillageRequestDto model, CancellationToken cancellationToken = default);
    Task<List<CraftVillageRequestResponseDto>> GetCraftVillageRequestsAsync(CraftVillageRequestStatus? status, CancellationToken cancellationToken = default);
    Task<CraftVillageRequestResponseDto> ReviewCraftVillageRequestAsync(Guid requestId, ReviewCraftVillageRequestDto model, CancellationToken cancellationToken = default);
    Task<CraftVillageRequestResponseDto> GetCraftVillageRequestAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<bool> DeleteCraftVillageRequestAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<TourGuideRequestResponseDto>> GetMyTourGuideRequestsAsync(TourGuideRequestStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<TourGuideRequestResponseDto> GetLatestTourGuideRequestsAsync(Guid? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<CraftVillageRequestResponseDto>> GetMyCraftVillageRequestsAsync(CraftVillageRequestStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<CraftVillageRequestResponseDto> GetLatestCraftVillageRequestsAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _timeService;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IEmailService _emailService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IEnumService _enumService;
    private readonly int YEARS_TO_BLOCK = 30;

    public UserService(IUnitOfWork unitOfWork, ITimeService timeService, IMapper mapper, IUserContextService userContextService, IEmailService emailService, ICloudinaryService cloudinaryService, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _timeService = timeService;
        _mapper = mapper;
        _userContextService = userContextService;
        _emailService = emailService;
        _cloudinaryService = cloudinaryService;
        _enumService = enumService;
    }
    public async Task<UserResponseModel> CreateUserAsync(CreateUserDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Email already exists");
            }

            var temporaryPassword = GenerateRandomPassword();

            var user = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = false,
                IsActive = true,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow,
                CreatedBy = "System",
                LastUpdatedBy = "System"
            };
            user.SetPassword(temporaryPassword);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            if (model.RoleId != Guid.Empty)
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(model.RoleId, cancellationToken);
                if (role == null)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Invalid role");
                }

                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                await _unitOfWork.UserRoleRepository.AddAsync(userRole);
                await _unitOfWork.SaveAsync();
            }

            await _emailService.SendEmailAsync(
                new[] { user.Email },
                $"mat khau tam ",
                $"mat khau tam {temporaryPassword}"
            );

            var userResponse = _mapper.Map<UserResponseModel>(user);
            userResponse.Roles = (await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id))
                .Select(r => r.Name).ToList();

            return userResponse;
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

    public async Task<UserResponseModel> AssignModeratorRoleAsync(Guid userId, UpdateUserRoleDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateBadRequestError("User");

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(model.RoleId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateBadRequestError("Invalid role");

            var newUserRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id
            };
            await _unitOfWork.UserRoleRepository.AddAsync(newUserRole);
            await _unitOfWork.SaveAsync();

            await _emailService.SendEmailAsync(
                new[] { user.Email },
                $"nâng cấp role",
                $"nâng cấp role {role.Name}"
            );

            var userResponse = _mapper.Map<UserResponseModel>(user);
            userResponse.Roles = (await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id))
                .Select(r => r.Name).ToList();

            return userResponse;
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

    public async Task<UserResponseModel> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new CustomException(StatusCodes.Status204NoContent, ResponseCodeConstants.NOT_FOUND, ResponseMessages.NOT_FOUND);
            var user = _mapper.Map<UserResponseModel>(existingUser);

            var roles = await _unitOfWork.UserRepository.GetRolesAsync(existingUser);
            if (roles == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            var roleNames = roles.Select(r => r.Name).ToList();

            user.Roles = roleNames;
            return user;
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

    public async Task<List<UserResponseModel>> GetAllUsersAsync(string? searchFullName = null, string? role = null, CancellationToken cancellationToken = default)
    {
        try
        {
            //var currentUserId = _userContextService.GetCurrentUserId();
            //var currentTime = _timeService.SystemTimeNow;

            //var isValidRole = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            //if (!isValidRole)
            //    throw CustomExceptionFactory.CreateForbiddenError();

            var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);

            if (users == null || !users.Any())
            {
                return new List<UserResponseModel>();
            }

            if (!string.IsNullOrEmpty(searchFullName))
            {
                users = users.Where(u => u.FullName.Contains(searchFullName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(role))
            {
                var filteredUsers = new List<User>();
                foreach (var user in users)
                {
                    var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id);
                    if (roles != null && roles.Any(r => r.Name.Contains(role, StringComparison.OrdinalIgnoreCase)))
                    {
                        filteredUsers.Add(user);
                    }
                }
                users = filteredUsers;
            }

            if (!users.Any())
            {
                return new List<UserResponseModel>();
            }

            var userDataModels = _mapper.Map<List<UserResponseModel>>(users);

            foreach (var user in userDataModels)
            {
                var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id);
                if (roles == null || !roles.Any())
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Người dùng chưa được cấp quyền");
                }

                var roleNames = roles.Select(r => r.Name).ToList();
                user.Roles = roleNames;
            }

            return userDataModels;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            // _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(string? email, string? phoneNumber, string? fullName, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.UserRepository.GetPageAsync(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(email))
            {
                //pagedResult.Items = pagedResult.Items.Where(u => u.Email.ToLower().Contains(email.ToLower())).ToList();
                pagedResult.Items = pagedResult.Items
                    .Where(u => !string.IsNullOrEmpty(u.Email) &&
                                u.Email.Contains(email, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                // SQL Server dung dong này
                //pagedResult.Items = pagedResult.Items.Where(u => u.PhoneNumber.ToLower().Contains(phoneNumber.ToLower())).ToList();
                pagedResult.Items = pagedResult.Items
                    .Where(u => !string.IsNullOrEmpty(u.PhoneNumber) &&
                                u.PhoneNumber.Contains(phoneNumber, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                pagedResult.Items = pagedResult.Items.Where(u => u.FullName.ToLower().Contains(fullName.ToLower())).ToList();
            }

            //pagedResult.TotalCount = pagedResult.Items.Count;

            var userDataModels = _mapper.Map<List<UserResponseModel>>(pagedResult.Items);

            foreach (var user in userDataModels)
            {
                var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id);
                if (roles == null)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
                }

                var roleNames = roles.Select(r => r.Name).ToList();

                user.Roles = roleNames;
            }

            return new PagedResult<UserResponseModel>
            {
                Items = userDataModels,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> BlockUserAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, new CancellationToken());
            if (user == null)
                return false;

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(YEARS_TO_BLOCK);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

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

    public async Task<bool> UnblockUserAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, new CancellationToken());
            if (user == null)
                return false;

            user.LockoutEnd = null;
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

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

    public async Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.UserRepository.GetPageAsync(pageNumber, pageSize);

            var userDataModels = _mapper.Map<List<UserResponseModel>>(pagedResult.Items);

            foreach (var user in userDataModels)
            {
                var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(user.Id);
                if (roles == null)
                {
                    throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
                }

                var roleNames = roles.Select(r => r.Name).ToList();

                user.Roles = roleNames;
            }

            return new PagedResult<UserResponseModel>
            {
                Items = userDataModels,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> UpdateUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var currentRoles = await _unitOfWork.UserRoleRepository.GetByUserId(userId);

            var currentRoleIds = currentRoles.Select(x => x.RoleId).ToList();

            var rolesToAdd = roleIds.Except(currentRoleIds).ToList();
            var rolesToRemove = currentRoleIds.Except(roleIds).ToList();

            var removeResult = await _unitOfWork.UserRoleRepository.RemoveFromRolesAsync(userId, rolesToRemove);
            var addResult = await _unitOfWork.UserRoleRepository.AddRolesToUser(userId, rolesToAdd);

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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserIdGuid();
            var isValidRole = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!isValidRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("vai trò");
            }

            if (role.Name == AppRole.TOUR_GUIDE || role.Name == AppRole.CRAFT_VILLAGE_OWNER)
                throw CustomExceptionFactory.CreateBadRequestError("Không được gán thủ công role này");

            var isAdmin = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (isAdmin && role.Name == AppRole.MODERATOR)
                throw CustomExceptionFactory.CreateBadRequestError("Bạn không được tự cấp quyền Moderator cho mình");

            // check user có role này chưa
            var userRoleExists = await _unitOfWork.UserRoleRepository.RoleExistsForUserAsync(userId, role.Id);
            if (userRoleExists)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Người dùng đã có vai trò này");
            }
            var result = await _unitOfWork.UserRoleRepository.AddRolesToUser(userId, new List<Guid> { role.Id });

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid districtId, string roleName)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                role = new Role(roleName)
                {
                    Id = Guid.NewGuid()
                };
                await _unitOfWork.RoleRepository.AddAsync(role);
            }

            // check user có role này chưa
            var userRoleExists = await _unitOfWork.UserRoleRepository.RoleExistsForUserAsync(userId, role.Id);
            if (!userRoleExists)
            {
                await _unitOfWork.UserRoleRepository.AddRolesToUser(userId, new List<Guid> { role.Id });
            }

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

    public async Task<UserResponseModel> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status204NoContent, ResponseCodeConstants.NOT_FOUND, ResponseMessages.NOT_FOUND);
            }

            var userResponse = _mapper.Map<UserResponseModel>(user);
            return userResponse;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            if (existingUser == null || existingUser.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("user");
            }

            _mapper.Map(model, existingUser);

            existingUser.LastUpdatedBy = currentUserId;
            existingUser.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.UserRepository.Update(existingUser);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<MediaResponse> UploadAvatarAsync(UploadMediaDto uploadMediaDto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(currentUserId), cancellationToken);
            if (existingUser == null || existingUser.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("user");
            }
            if (uploadMediaDto.File == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("image");
            }

            if (existingUser.AvatarUrl != null && existingUser.AvatarUrl != string.Empty)
            {
                await _cloudinaryService.DeleteImageAsync(existingUser.AvatarUrl);
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(uploadMediaDto.File);
            var mediaResponses = new MediaResponse()
            {
                MediaUrl = imageUrl,
                FileName = uploadMediaDto.File.FileName,
                FileType = uploadMediaDto.File.ContentType,
                SizeInBytes = uploadMediaDto.File.Length
            };

            existingUser.AvatarUrl = imageUrl;

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return mediaResponses;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("người dùng");
            }

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("vai trò");
            }

            // check user có role này chưa
            var userRoleExists = await _unitOfWork.UserRoleRepository.RoleExistsForUserAsync(userId, role.Id);
            if (!userRoleExists)
            {
                throw CustomExceptionFactory.CreateNotFoundError("vai trò trong người dùng này");
            }

            var result = await _unitOfWork.UserRoleRepository.RemoveFromRolesAsync(userId, new List<Guid> { role.Id });

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> SendFeedbackAsync(FeedbackModel model, CancellationToken cancellationToken)
    {
        try
        {
            var userIdGuid = Guid.Parse(_userContextService.GetCurrentUserId());

            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email ??= string.Empty, cancellationToken);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("người dùng");
            }

            var selectedEmail = new List<string> { "traveloguetayninh@gmail.com" };
            var maiModel = new
            {
                Email = model.Email,
                Message = model.Message
            };

            var resultSendMail = await _emailService.SendEmailWithTemplateAsync(
                selectedEmail,
                $"Đóng góp ý kiến từ người dùng {user.Email}",
                MailTemplateLinks.SendFeedbackTemplate,
                maiModel
            );

            return resultSendMail;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<UserManageResponse> GetUserManageAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.ActiveEntities
                .Include(u => u.Wallet)
                .Include(u => u.UserRoles)!.ThenInclude(ur => ur.Role)
                .Include(u => u.BankAccounts)
                .FirstOrDefaultAsync(u => u.Id == userId, ct)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            var transactionDtos = new List<TransactionDto>();
            if (user.Wallet != null)
            {
                var transactions = await _unitOfWork.TransactionEntryRepository.ActiveEntities
                    .Where(t => t.WalletId == user.Wallet.Id)
                    .OrderByDescending(t => t.CreatedTime)
                    .ToListAsync(ct);

                transactionDtos = transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    WalletId = t.WalletId,
                    UserId = t.UserId,
                    IsSystem = t.IsSystem,
                    SystemKind = t.SystemKind,
                    SystemKindText = t.SystemKind.HasValue
                        ? _enumService.GetEnumDisplayName<SystemTransactionKind>(t.SystemKind.Value)
                        : null,
                    Channel = t.Channel,
                    PaymentChannelText = t.Channel.HasValue
                        ? _enumService.GetEnumDisplayName<PaymentChannel>(t.Channel.Value)
                        : null,
                    AccountNumber = t.AccountNumber,
                    PaidAmount = t.PaidAmount,
                    PaymentReference = t.PaymentReference,
                    TransactionDateTime = t.TransactionDateTime,
                    CounterAccountBankId = t.CounterAccountBankId,
                    CounterAccountName = t.CounterAccountName,
                    CounterAccountNumber = t.CounterAccountNumber,
                    Currency = t.Currency,
                    PaymentLinkId = t.PaymentLinkId,
                    PaymentStatus = t.PaymentStatus,
                    PaymentStatusText = t.PaymentStatus.HasValue
                        ? _enumService.GetEnumDisplayName<PaymentStatus>(t.PaymentStatus.Value)
                        : null,
                    Status = t.Status,
                    StatusText = _enumService.GetEnumDisplayName<TransactionStatus>(t.Status),
                    Type = t.Type,
                    TypeText = _enumService.GetEnumDisplayName<TransactionType>(t.Type),
                    TransactionDirection = t.TransactionDirection,
                    TransactionDirectionText = _enumService.GetEnumDisplayName<TransactionDirection>(t.TransactionDirection),
                    Reason = t.Reason,
                    Method = t.Method
                }).ToList();
            }

            // role
            var roleDtos = (user.UserRoles ?? Array.Empty<UserRole>())
                .Where(ur => ur.Role != null)
                .Select(ur => new RoleManageDto
                {
                    Name = ur.Role!.Name,
                    CreatedAt = ur.Role.CreatedTime.UtcDateTime,
                    IsActive = !ur.Role.IsDeleted
                })
                .ToList();

            // bank account
            var bankAccounts = user.BankAccounts?
                .Select(b => new BankAccountDto
                {
                    Id = b.Id,
                    BankName = b.BankName,
                    BankAccountNumber = b.BankAccountNumber,
                    BankOwnerName = b.BankOwnerName,
                    CreatedAt = b.CreatedAt,
                    IsDefault = b.IsDefault
                })
                .ToList() ?? new List<BankAccountDto>();

            // tour guide
            TourGuideInfo? tourGuideInfo = null;
            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(g => g.Certifications)
                .FirstOrDefaultAsync(g => g.UserId == user.Id, ct);

            if (tourGuide != null)
            {
                var guideReviews = await _unitOfWork.ReviewRepository.ActiveEntities
                    .Include(r => r.Booking)
                    .Where(r => r.Booking.TourGuideId == tourGuide.Id)
                    .Select(r => (double?)r.Rating)
                    .ToListAsync(ct);

                var avg = guideReviews.Any() ? (int)Math.Round(guideReviews.Average() ?? 0) : 0;
                var total = guideReviews.Count;

                tourGuideInfo = new TourGuideInfo
                {
                    Id = tourGuide.Id,
                    Rating = avg,
                    TotalReviews = total,
                    Price = tourGuide.Price,
                    Introduction = tourGuide.Introduction,
                    Certifications = (tourGuide.Certifications ?? Enumerable.Empty<Certification>())
                        .Select(c => new CertificationDto
                        {
                            Name = c.Name,
                            CertificateUrl = c.CertificateUrl,
                        })
                        .ToList()
                };
            }


            // lang nghe
            CraftVillagesInfo? craftInfo = null;
            var craftVillage = await _unitOfWork.CraftVillageRepository.ActiveEntities
                .FirstOrDefaultAsync(cv => cv.OwnerId == user.Id, ct);

            if (craftVillage != null)
            {
                craftInfo = new CraftVillagesInfo
                {
                    Id = craftVillage.Id,
                    PhoneNumber = craftVillage.PhoneNumber,
                    Email = craftVillage.Email,
                    Website = craftVillage.Website,
                    SignatureProduct = craftVillage.SignatureProduct,
                    YearsOfHistory = craftVillage.YearsOfHistory,
                    IsRecognizedByUnesco = craftVillage.IsRecognizedByUnesco,
                    WorkshopsAvailable = craftVillage.WorkshopsAvailable,
                    LocationId = craftVillage.LocationId
                };
            }

            // response
            var response = new UserManageResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.FullName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl ?? user.ProfilePictureUrl,
                Roles = roleDtos,
                Sex = user.Sex,
                GenderText = _enumService.GetEnumDisplayName<Gender>(user.Sex),
                Address = user.Address,
                IsEmailVerified = user.IsEmailVerified,
                LockoutEnd = user.LockoutEnd,
                BankAccounts = bankAccounts,
                Wallet = new WalletDto
                {
                    UserWalletAmount = user.Wallet?.Balance ?? 0m,
                    TransactionDtos = transactionDtos
                },
                TourGuideInfo = tourGuideInfo,
                CraftVillagesInfo = craftInfo,
                CreatedBy = user.CreatedBy,
                LastUpdatedBy = user.LastUpdatedBy,
                CreatedTime = user.CreatedTime,
                LastUpdatedTime = user.LastUpdatedTime
            };

            return response;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> DisableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var currentUserId = _userContextService.GetCurrentUserIdGuid();

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.UserRepository.ActiveEntities
                .FirstOrDefaultAsync(u => u.Id == userId, ct)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            var role = await _unitOfWork.RoleRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == roleId, ct)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Role");

            var userRole = await _unitOfWork.UserRoleRepository.Entities
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id, ct)
                ?? throw CustomExceptionFactory.CreateBadRequestError("User chưa gán role này.");

            if (!userRole.IsActive)
                return true;

            if (string.Equals(role.Name, AppRole.ADMIN, StringComparison.OrdinalIgnoreCase))
            {
                var otherActiveAdmins = await _unitOfWork.UserRoleRepository.Entities
                    .CountAsync(ur => ur.RoleId == role.Id && ur.IsActive && ur.UserId != userId, ct);

                if (otherActiveAdmins == 0)
                    throw CustomExceptionFactory.CreateBadRequestError("Phải có 1 admin tồn tại");
            }

            userRole.IsActive = false;
            userRole.LastUpdatedBy = currentUserId.ToString();
            userRole.LastUpdatedTime = DateTimeOffset.UtcNow;
            _unitOfWork.UserRoleRepository.Update(userRole);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<bool> EnableUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var currentUserId = _userContextService.GetCurrentUserIdGuid();

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.UserRepository.ActiveEntities
                .FirstOrDefaultAsync(u => u.Id == userId, ct)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            var role = await _unitOfWork.RoleRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == roleId, ct)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Role");

            var userRole = await _unitOfWork.UserRoleRepository.Entities
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id, ct)
                ?? throw CustomExceptionFactory.CreateBadRequestError("User chưa gán role này.");

            if (userRole.IsActive)
                return true;

            userRole.IsActive = true;
            userRole.LastUpdatedBy = currentUserId.ToString();
            userRole.LastUpdatedTime = DateTimeOffset.UtcNow;
            _unitOfWork.UserRoleRepository.Update(userRole);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<TourGuideRequestResponseDto> CreateTourGuideRequestAsync(CreateTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentUserIdString = currentUserId.ToString();
            var currentTime = _timeService.SystemTimeNow;
            var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("User");
            }

            var existingTourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(currentUserId);
            if (existingTourGuide != null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đã là 1 tour guide");
            }

            var tourGuideRequest = new TourGuideRequest
            {
                UserId = currentUserId,
                Introduction = model.Introduction,
                Price = model.Price,
                Status = TourGuideRequestStatus.Pending,
                CreatedTime = currentTime,
                LastUpdatedTime = currentTime,
                CreatedBy = currentUserIdString,
                LastUpdatedBy = currentUserIdString
            };

            foreach (var cert in model.Certifications)
            {
                tourGuideRequest.Certifications.Add(new TourGuideRequestCertification
                {
                    Name = cert.Name,
                    CertificateUrl = cert.CertificateUrl,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime,
                    CreatedBy = currentUserIdString,
                    LastUpdatedBy = currentUserIdString
                });
            }

            await _unitOfWork.TourGuideRequestRepository.AddAsync(tourGuideRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            // Gửi email thông báo cho Moderator
            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                   new[] { moderator.Email },
                    "Có người dùng cần đăng ký role",
                    "Có người dùng cần đăng ký role"
                );
            }

            var response = new TourGuideRequestResponseDto
            {
                Id = tourGuideRequest.Id,
                UserId = tourGuideRequest.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Introduction = tourGuideRequest.Introduction,
                Price = tourGuideRequest.Price,
                Status = tourGuideRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(tourGuideRequest.Status),
                RejectionReason = tourGuideRequest.RejectionReason,
                Certifications = tourGuideRequest.Certifications
                .Select(c => new CertificationDto
                {
                    Name = c.Name,
                    CertificateUrl = c.CertificateUrl
                })
                .ToList()
            };

            response.Email = user.Email;
            response.FullName = user.FullName;
            // response.Certifications = _mapper.Map<List<CertificationDto>>(tourGuideRequest.Certifications);

            return response;

        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<TourGuideRequestResponseDto> UpdateTourGuideRequestAsync(Guid id, UpdateTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentUserIdString = currentUserId.ToString();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Include(x => x.Certifications)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuideRequest");

            if (request.UserId != currentUserId)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (request.Status != TourGuideRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể sửa request đã xử lý");

            request.Introduction = model.Introduction;
            request.Price = model.Price;
            request.LastUpdatedTime = currentTime;
            request.LastUpdatedBy = currentUserIdString;

            request.Certifications.Clear();
            foreach (var cert in model.Certifications)
            {
                request.Certifications.Add(new TourGuideRequestCertification
                {
                    Name = cert.Name,
                    CertificateUrl = cert.CertificateUrl,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime,
                    CreatedBy = currentUserIdString,
                    LastUpdatedBy = currentUserIdString
                });
            }

            _unitOfWork.TourGuideRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            return new TourGuideRequestResponseDto
            {
                Id = request.Id,
                UserId = request.UserId,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Introduction = request.Introduction,
                Price = request.Price,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(request.Status),
                RejectionReason = request.RejectionReason,
                Certifications = request.Certifications
                    .Select(c => new CertificationDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl
                    })
                    .ToList()
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> DeleteTourGuideRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentUserIdString = currentUserId.ToString();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuideRequest");

            if (request.UserId != currentUserId)
                throw CustomExceptionFactory.CreateForbiddenError();

            request.IsDeleted = true;
            request.LastUpdatedTime = currentTime;
            request.LastUpdatedBy = currentUserIdString;

            _unitOfWork.TourGuideRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<TourGuideRequestResponseDto>> GetTourGuideRequestsAsync(TourGuideRequestStatus? status, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<TourGuideRequest> query = _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Include(x => x.Certifications);

            if (status != null)
            {
                query = query.Where(x => x.Status == status);
            }

            var requests = await query.ToListAsync();
            var response = new List<TourGuideRequestResponseDto>();

            foreach (var request in requests)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("User");
                var requestDto = new TourGuideRequestResponseDto
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Introduction = request.Introduction,
                    Price = request.Price,
                    Status = request.Status,
                    StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(request.Status),
                    RejectionReason = request.RejectionReason,
                    Certifications = request.Certifications
                    .Select(c => new CertificationDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl
                    })
                    .ToList()
                };
                requestDto.Email = user?.Email ?? string.Empty;
                requestDto.FullName = user?.FullName ?? string.Empty;
                // requestDto.Certifications = _mapper.Map<List<CertificationDto>>(request.Certifications);
                response.Add(requestDto);
            }

            return response;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<PagedResult<TourGuideRequestResponseDto>> GetMyTourGuideRequestsAsync(TourGuideRequestStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            IQueryable<TourGuideRequest> query = _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Where(r => r.UserId == currentUserId)
                .Include(x => x.Certifications)
                .OrderByDescending(r => r.CreatedTime)
                .Include(x => x.Certifications);

            if (status != null)
            {
                query = query.Where(x => x.Status == status);
            }

            var requests = await query.ToListAsync();
            var response = new List<TourGuideRequestResponseDto>();

            foreach (var request in requests)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("User");
                var requestDto = new TourGuideRequestResponseDto
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Introduction = request.Introduction,
                    Price = request.Price,
                    Status = request.Status,
                    StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(request.Status),
                    RejectionReason = request.RejectionReason,
                    Certifications = request.Certifications
                    .Select(c => new CertificationDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl
                    })
                    .ToList()
                };
                requestDto.Email = user?.Email ?? string.Empty;
                requestDto.FullName = user?.FullName ?? string.Empty;
                // requestDto.Certifications = _mapper.Map<List<CertificationDto>>(request.Certifications);
                response.Add(requestDto);
            }

            // return response;

            return new PagedResult<TourGuideRequestResponseDto>
            {
                Items = response,
                TotalCount = query.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<TourGuideRequestResponseDto> GetLatestTourGuideRequestsAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == null)
                throw CustomExceptionFactory.CreateBadRequestError("UserId is required");

            var latestRequest = await _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Where(r => r.UserId == userId)
                .Include(x => x.Certifications)
                .OrderByDescending(r => r.CreatedTime)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestRequest == null)
                return null;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(latestRequest.UserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            return new TourGuideRequestResponseDto
            {
                Id = latestRequest.Id,
                UserId = latestRequest.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Introduction = latestRequest.Introduction,
                Price = latestRequest.Price,
                Status = latestRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(latestRequest.Status),
                RejectionReason = latestRequest.RejectionReason,
                Certifications = latestRequest.Certifications
                    .Select(c => new CertificationDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl
                    })
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<TourGuideRequestResponseDto> GetTourGuideRequestByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Include(x => x.Certifications)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuideRequest");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("User");

            return new TourGuideRequestResponseDto
            {
                Id = request.Id,
                UserId = request.UserId,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Introduction = request.Introduction,
                Price = request.Price,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(request.Status),
                RejectionReason = request.RejectionReason,
                Certifications = request.Certifications
                    .Select(c => new CertificationDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl
                    })
                    .ToList()
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


    public async Task<TourGuideRequestResponseDto> ReviewTourGuideRequestAsync(Guid requestId, ReviewTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.TourGuideRequestRepository
                .ActiveEntities
                .Include(r => r.Certifications)
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
            if (request == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("TourGuide request");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("User");
            }

            request.Status = model.Status;
            request.ReviewedAt = currentTime;
            request.ReviewedBy = Guid.Parse(currentUserId);
            request.LastUpdatedTime = currentTime;
            request.LastUpdatedBy = currentUserId;

            if (model.Status == TourGuideRequestStatus.Rejected)
            {
                request.RejectionReason = model.RejectionReason;
            }
            else if (model.Status == TourGuideRequestStatus.Approved)
            {
                var tourGuide = new TourGuide
                {
                    UserId = request.UserId,
                    Rating = 0,
                    Price = request.Price,
                    Introduction = request.Introduction,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    Certifications = new List<Certification>()
                };

                foreach (var cert in request.Certifications)
                {
                    tourGuide.Certifications.Add(new Certification
                    {
                        Name = cert.Name,
                        CertificateUrl = cert.CertificateUrl,
                        TourGuideId = tourGuide.Id,
                        CreatedTime = currentTime,
                        LastUpdatedBy = currentUserId,
                        CreatedBy = currentUserId,
                        LastUpdatedTime = currentTime
                    });
                }

                await _unitOfWork.TourGuideRepository.AddAsync(tourGuide);

                var role = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.TOUR_GUIDE);
                if (role == null)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("TourGuide role");
                }

                var userRole = new UserRole
                {
                    UserId = request.UserId,
                    RoleId = role.Id
                };
                await _unitOfWork.UserRoleRepository.AddAsync(userRole);

                user.VerificationToken = string.Empty;
                user.ResetToken = string.Empty;
                user.VerificationTokenExpires = currentTime.AddYears(-1);
                user.ResetTokenExpires = currentTime.AddYears(-1);
                _unitOfWork.UserRepository.Update(user);
            }

            _unitOfWork.TourGuideRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            if (model.Status == TourGuideRequestStatus.Rejected)
            {
                await _emailService.SendEmailAsync(
                    new[] { user.Email },
                    $"Yêu cầu nâng cấp tour guide của bạn đã bị từ chối",
                    $"Yêu cầu nâng cấp tour guide của bạn đã bị từ chối"
                );
            }
            else if (model.Status == TourGuideRequestStatus.Approved)
            {
                await _emailService.SendEmailAsync(
                    new[] { user.Email },
                    $"Nâng cấp role thành công Tour guide",
                    $"Tour guide"
                );
            }

            var response = MapToTourGuideRequestResponseDto(request, user);
            response.Email = user.Email;
            response.FullName = user.FullName;
            response.StatusText = _enumService.GetEnumDisplayName<TourGuideRequestStatus>(request.Status);
            // response.Certifications = _mapper.Map<List<CertificationDto>>(request.Certifications);

            return response;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    //public async Task<CraftVillageRequestResponseDto> CreateCraftVillageRequestAsync(CreateCraftVillageRequestDto model, CancellationToken cancellationToken = default)
    //{
    //    using var transaction = await _unitOfWork.BeginTransactionAsync();
    //    try
    //    {
    //        var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
    //        var hasPermission = _userContextService.HasAnyRole(AppRole.USER);
    //        if (!hasPermission)
    //            throw CustomExceptionFactory.CreateForbiddenError();

    //        var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId, cancellationToken);
    //        if (user == null)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("User");
    //        }

    //        var district = await _unitOfWork.DistrictRepository
    //            .ActiveEntities
    //            .Where(d => d.Id == model.DistrictId)
    //            .FirstOrDefaultAsync()
    //            ?? throw CustomExceptionFactory.CreateNotFoundError("district");

    //        var existingCraftVillage = await _unitOfWork.CraftVillageRepository
    //            .ActiveEntities
    //            .FirstOrDefaultAsync(cv => cv.OwnerId == user.Id);
    //        if (existingCraftVillage != null)
    //        {
    //            throw CustomExceptionFactory.CreateBadRequestError("Bạn đã là chủ 1 làng nghề");
    //        }

    //        List<MediaRequest> medias = model.MediaDtos.ToMediaRequest();

    //        var craftVillageRequest = new CraftVillageRequest
    //        {
    //            Name = model.Name,
    //            Description = model.Description,
    //            Content = model.Content,
    //            Address = model.Address,
    //            Latitude = model.Latitude,
    //            Longitude = model.Longitude,
    //            OpenTime = model.OpenTime,
    //            CloseTime = model.CloseTime,
    //            DistrictId = model.DistrictId,
    //            OwnerId = currentUserId,
    //            PhoneNumber = model.PhoneNumber,
    //            Email = model.Email,
    //            Website = model.Website,
    //            WorkshopsAvailable = model.WorkshopsAvailable,
    //            SignatureProduct = model.SignatureProduct,
    //            YearsOfHistory = model.YearsOfHistory,
    //            IsRecognizedByUnesco = model.IsRecognizedByUnesco,
    //            Status = CraftVillageRequestStatus.Pending,
    //            CreatedTime = DateTimeOffset.UtcNow,
    //            LastUpdatedTime = DateTimeOffset.UtcNow,
    //            CreatedBy = currentUserId.ToString(),
    //            LastUpdatedBy = currentUserId.ToString(),
    //            Medias = medias
    //        };

    //        await _unitOfWork.CraftVillageRequestRepository.AddAsync(craftVillageRequest);
    //        await _unitOfWork.SaveAsync();
    //        await transaction.CommitAsync();

    //        var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
    //        foreach (var moderator in moderators)
    //        {
    //            await _emailService.SendEmailAsync(
    //                new[] { moderator.Email },
    //                "Yêu cầu duyệt làng nghề",
    //                "Yêu cầu duyệt làng nghề"
    //            );
    //        }

    //        var response = MapToCraftVillageRequestResponseDto(craftVillageRequest, user);

    //        return response;
    //    }
    //    catch (CustomException)
    //    {
    //        await transaction.RollbackAsync(cancellationToken);
    //        throw;
    //    }
    //    catch (Exception ex)
    //    {
    //        await transaction.RollbackAsync(cancellationToken);
    //        throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //    }
    //}

    public async Task<CraftVillageRequestResponseDto> CreateCraftVillageRequestAsync(
        CreateCraftVillageRequestDto model,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            // var hasPermission = _userContextService.HasAnyRole(AppRole.USER);
            // if (!hasPermission)
            //     throw CustomExceptionFactory.CreateForbiddenError();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (user == null)
                throw CustomExceptionFactory.CreateNotFoundError("User");

            var district = await _unitOfWork.DistrictRepository
                .ActiveEntities
                .Where(d => d.Id == model.DistrictId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("district");

            var existingCraftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(cv => cv.OwnerId == user.Id, cancellationToken);
            if (existingCraftVillage != null)
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đã là chủ 1 làng nghề");

            List<MediaRequest> medias = model.MediaDtos.ToMediaRequest();

            var craftVillageRequest = new CraftVillageRequest
            {
                Name = model.Name,
                Description = model.Description,
                Content = model.Content,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                OpenTime = model.OpenTime,
                CloseTime = model.CloseTime,
                DistrictId = model.DistrictId,
                OwnerId = currentUserId,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                WorkshopsAvailable = model.WorkshopsAvailable,
                SignatureProduct = model.SignatureProduct,
                YearsOfHistory = model.YearsOfHistory,
                IsRecognizedByUnesco = model.IsRecognizedByUnesco,
                Status = CraftVillageRequestStatus.Pending,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow,
                CreatedBy = currentUserId.ToString(),
                LastUpdatedBy = currentUserId.ToString(),
                Medias = medias
            };

            // Nếu có workshop kèm theo
            if (model.WorkshopsAvailable && model.Workshop != null)
            {
                var workshopDto = model.Workshop;

                int SessionLengthMinutes(TimeSpan start, TimeSpan end)
                {
                    if (end > start) return (int)(end - start).TotalMinutes;
                    return (int)((TimeSpan.FromDays(1) - start) + end).TotalMinutes;
                }

                var allSessionLengths = (workshopDto.RecurringRules ?? Enumerable.Empty<RecurringRuleRequestDto>())
                    .SelectMany(r => r.Sessions ?? Enumerable.Empty<SessionRequestDto>())
                    .Select(s =>
                    {
                        if (s.EndTime == s.StartTime)
                            throw CustomExceptionFactory.CreateBadRequestError(
                                $"Session không hợp lệ: StartTime và EndTime không được bằng nhau. (Start={s.StartTime}, End={s.EndTime})");

                        var len = SessionLengthMinutes(s.StartTime, s.EndTime);
                        if (len < 1)
                            throw CustomExceptionFactory.CreateBadRequestError(
                                $"Session không hợp lệ: độ dài phải >= 1 phút. (Start={s.StartTime}, End={s.EndTime})");

                        return len;
                    })
                    .ToList();

                if (!allSessionLengths.Any())
                    throw CustomExceptionFactory.CreateBadRequestError("RecurringRule phải có ít nhất một session.");

                var minSessionLen = allSessionLengths.Min();

                foreach (var ticket in workshopDto.TicketTypes ?? Enumerable.Empty<TicketTypeRequestDto>())
                {
                    if (ticket.DurationMinutes < 1)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Ticket \"{ticket.Name}\" phải có DurationMinutes >= 1.");

                    var totalActivityMinutes = (ticket.WorkshopActivities ?? new List<WorkshopActivityRequestDto>())
                        .Select(a =>
                        {
                            if (a.DurationMinutes < 1)
                                throw CustomExceptionFactory.CreateBadRequestError(
                                    $"Hoạt động \"{a.Activity}\" phải có DurationMinutes >= 1.");
                            return a.DurationMinutes;
                        })
                        .Sum();

                    if (totalActivityMinutes > ticket.DurationMinutes)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Tổng thời lượng hoạt động ({totalActivityMinutes} phút) vượt quá DurationMinutes " +
                            $"({ticket.DurationMinutes} phút) của ticket \"{ticket.Name}\".");

                    if (ticket.DurationMinutes > minSessionLen)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Thời lượng ticket \"{ticket.Name}\" ({ticket.DurationMinutes} phút) " +
                            $"vượt quá thời lượng session ngắn nhất ({minSessionLen} phút).");

                    if (totalActivityMinutes > minSessionLen)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Tổng thời lượng hoạt động ({totalActivityMinutes} phút) của ticket \"{ticket.Name}\" " +
                            $"vượt quá thời lượng session ngắn nhất ({minSessionLen} phút).");
                }

                var workshopRequest = new WorkshopRequest
                {
                    Name = workshopDto.Name,
                    Description = workshopDto.Description,
                    Content = workshopDto.Content,
                    Status = workshopDto.Status,
                    CraftVillageId = craftVillageRequest.Id,
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow,
                    CreatedBy = currentUserId.ToString(),
                    LastUpdatedBy = currentUserId.ToString(),

                    TicketTypes = new List<WorkshopTicketTypeRequest>(),
                    RecurringRules = new List<WorkshopRecurringRuleRequest>(),
                    Exceptions = new List<WorkshopExceptionRequest>()
                };

                foreach (var ticketDto in workshopDto.TicketTypes ?? Enumerable.Empty<TicketTypeRequestDto>())
                {
                    var ticket = new WorkshopTicketTypeRequest
                    {
                        Type = ticketDto.Type,
                        Name = ticketDto.Name,
                        Price = ticketDto.Price,
                        IsCombo = ticketDto.IsCombo,
                        DurationMinutes = ticketDto.DurationMinutes,
                        Content = ticketDto.Content,

                        WorkshopActivities = new List<WorkshopActivityRequest>()
                    };

                    if (ticketDto.WorkshopActivities != null && ticketDto.WorkshopActivities.Any())
                    {
                        foreach (var actDto in ticketDto.WorkshopActivities)
                        {
                            ticket.WorkshopActivities.Add(new WorkshopActivityRequest
                            {
                                Activity = actDto.Activity,
                                Description = actDto.Description,
                                DurationMinutes = actDto.DurationMinutes,
                                ActivityOrder = actDto.ActivityOrder
                            });
                        }
                    }

                    workshopRequest.TicketTypes.Add(ticket);
                }

                foreach (var ruleDto in workshopDto.RecurringRules ?? Enumerable.Empty<RecurringRuleRequestDto>())
                {
                    var rule = new WorkshopRecurringRuleRequest
                    {
                        DaysOfWeek = ruleDto.DaysOfWeek,
                        Sessions = new List<WorkshopSessionRequest>()
                    };

                    foreach (var sessionDto in ruleDto.Sessions ?? Enumerable.Empty<SessionRequestDto>())
                    {
                        rule.Sessions.Add(new WorkshopSessionRequest
                        {
                            StartTime = sessionDto.StartTime,
                            EndTime = sessionDto.EndTime,
                            Capacity = sessionDto.Capacity
                        });
                    }

                    workshopRequest.RecurringRules.Add(rule);
                }

                foreach (var exDto in workshopDto.Exceptions ?? Enumerable.Empty<WorkshopExceptionRequestDto>())
                {
                    workshopRequest.Exceptions.Add(new WorkshopExceptionRequest
                    {
                        Date = exDto.Date,
                        Reason = exDto.Reason,
                        IsActive = exDto.IsActive
                    });
                }

                craftVillageRequest.Workshop = workshopRequest;
            }

            await _unitOfWork.CraftVillageRequestRepository.AddAsync(craftVillageRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            // Gửi email cho moderator
            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                    new[] { moderator.Email },
                    "Yêu cầu duyệt làng nghề",
                    $"Người dùng {user.FullName} đã gửi yêu cầu đăng ký làng nghề"
                );
            }

            var response = MapToCraftVillageRequestResponseDto(craftVillageRequest, user);
            return response;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CraftVillageRequestResponseDto> UpdateCraftVillageRequestAsync(
        Guid id,
        UpdateCraftVillageRequestDto model,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var hasPermission = _userContextService.HasAnyRole(AppRole.USER);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(currentUserId), cancellationToken);
            if (user == null)
                throw CustomExceptionFactory.CreateNotFoundError("User");

            var craftVillageRequest = await _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("craftVillageRequest");

            if (craftVillageRequest.Status != CraftVillageRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật yêu cầu đã xử lý");

            if (craftVillageRequest == null)
                throw CustomExceptionFactory.CreateNotFoundError("CraftVillageRequest");

            if (craftVillageRequest.OwnerId != user.Id)
                throw CustomExceptionFactory.CreateForbiddenError();

            var district = await _unitOfWork.DistrictRepository
               .ActiveEntities
               .Where(d => d.Id == model.DistrictId)
               .FirstOrDefaultAsync()
               ?? throw CustomExceptionFactory.CreateNotFoundError("district");

            craftVillageRequest.Name = model.Name;
            craftVillageRequest.Description = model.Description;
            craftVillageRequest.Content = model.Content;
            craftVillageRequest.Address = model.Address;
            craftVillageRequest.Latitude = model.Latitude;
            craftVillageRequest.Longitude = model.Longitude;
            craftVillageRequest.OpenTime = model.OpenTime;
            craftVillageRequest.CloseTime = model.CloseTime;
            craftVillageRequest.DistrictId = model.DistrictId;
            craftVillageRequest.PhoneNumber = model.PhoneNumber;
            craftVillageRequest.Email = model.Email;
            craftVillageRequest.Website = model.Website;
            craftVillageRequest.WorkshopsAvailable = model.WorkshopsAvailable;
            craftVillageRequest.SignatureProduct = model.SignatureProduct;
            craftVillageRequest.YearsOfHistory = model.YearsOfHistory;
            craftVillageRequest.IsRecognizedByUnesco = model.IsRecognizedByUnesco;

            craftVillageRequest.LastUpdatedTime = DateTimeOffset.UtcNow;
            craftVillageRequest.LastUpdatedBy = currentUserId;

            _unitOfWork.CraftVillageRequestRepository.Update(craftVillageRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var response = MapToCraftVillageRequestResponseDto(craftVillageRequest, user);
            return response;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<CraftVillageRequestResponseDto>> GetCraftVillageRequestsAsync(
        CraftVillageRequestStatus? status,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<CraftVillageRequest> baseQuery =
                _unitOfWork.CraftVillageRequestRepository
                    .ActiveEntities
                    .AsNoTracking();

            if (status.HasValue)
                baseQuery = baseQuery.Where(x => x.Status == status.Value);

            var query = baseQuery
                .Include(x => x.Workshop)
                    .ThenInclude(w => w.TicketTypes)
                        .ThenInclude(tt => tt.WorkshopActivities)
                .Include(x => x.Workshop)
                    .ThenInclude(w => w.RecurringRules)
                        .ThenInclude(rr => rr.Sessions)
                .Include(x => x.Workshop)
                    .ThenInclude(w => w.Exceptions)
                .AsSplitQuery();

            var requests = await query.ToListAsync(cancellationToken);

            var response = new List<CraftVillageRequestResponseDto>(requests.Count);
            foreach (var request in requests)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);

                var dto = MapToCraftVillageRequestResponseDto(request, user);
                dto.Medias = request.Medias.ToMediaDto();

                dto.Workshop = MapWorkshopRequestToResponseDto(request.Workshop);

                response.Add(dto);
            }

            return response;
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CraftVillageRequestResponseDto> ReviewCraftVillageRequestAsync(
        Guid requestId,
        ReviewCraftVillageRequestDto model,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .Include(x => x.Workshop)
                    .ThenInclude(wr => wr.Exceptions)
                .Include(x => x.Workshop)
                    .ThenInclude(wr => wr.RecurringRules)
                        .ThenInclude(wr => wr.Sessions)
                .Include(x => x.Workshop)
                    .ThenInclude(wr => wr.TicketTypes)
                        .ThenInclude(wr => wr.WorkshopActivities)
                .FirstOrDefaultAsync(x => x.Id == requestId);
            if (request == null)
                throw CustomExceptionFactory.CreateBadRequestError("CraftVillage request");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
                throw CustomExceptionFactory.CreateBadRequestError("User");

            if (request.Status != CraftVillageRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật yêu cầu đã xử lý");

            // Cập nhật trạng thái yêu cầu
            request.Status = model.Status;
            request.RejectionReason = model.RejectionReason;
            request.ReviewedAt = DateTimeOffset.UtcNow;
            request.ReviewedBy = currentUserId;
            request.LastUpdatedTime = DateTimeOffset.UtcNow;
            request.LastUpdatedBy = currentUserId;

            Guid? createdWorkshopId = null;

            if (model.Status == CraftVillageRequestStatus.Approved)
            {
                var location = new Location
                {
                    Name = request.Name,
                    Description = request.Description,
                    Content = request.Content,
                    LocationType = LocationType.CraftVillage,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Address = request.Address,
                    DistrictId = request.DistrictId,
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId
                };
                await _unitOfWork.LocationRepository.AddAsync(location);

                var craftVillage = new CraftVillage
                {
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Website = request.Website,
                    LocationId = location.Id,
                    OwnerId = request.OwnerId,
                    WorkshopsAvailable = request.WorkshopsAvailable,
                    SignatureProduct = request.SignatureProduct,
                    YearsOfHistory = request.YearsOfHistory,
                    IsRecognizedByUnesco = request.IsRecognizedByUnesco,
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId
                };
                await _unitOfWork.CraftVillageRepository.AddAsync(craftVillage);

                if (request.Medias != null && request.Medias.Any())
                {
                    foreach (var media in request.Medias)
                    {
                        var locationMedia = new LocationMedia
                        {
                            LocationId = location.Id,
                            MediaUrl = media.MediaUrl,
                            FileName = Path.GetFileName(media.MediaUrl),
                            FileType = Path.GetExtension(media.MediaUrl),
                            SizeInBytes = 0,
                            IsThumbnail = media.IsThumbnail,
                            CreatedTime = DateTimeOffset.UtcNow,
                            LastUpdatedTime = DateTimeOffset.UtcNow,
                            CreatedBy = currentUserId,
                            LastUpdatedBy = currentUserId
                        };
                        await _unitOfWork.LocationMediaRepository.AddAsync(locationMedia);
                    }
                }

                var role = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.CRAFT_VILLAGE_OWNER);
                if (role == null)
                    throw CustomExceptionFactory.CreateBadRequestError("CraftVillage role");

                var hasRole = await _unitOfWork.UserRoleRepository.RoleExistsForUserAsync(request.OwnerId, role.Id);
                if (!hasRole)
                {
                    var userRole = new UserRole { UserId = request.OwnerId, RoleId = role.Id };
                    await _unitOfWork.UserRoleRepository.AddAsync(userRole);
                }

                if (request.WorkshopsAvailable && request.Workshop != null)
                {
                    var wReq = request.Workshop;

                    var workshop = new Workshop
                    {
                        Name = wReq.Name,
                        Description = wReq.Description,
                        Content = wReq.Content,
                        Status = wReq.Status,
                        CraftVillageId = craftVillage.Id,
                        CreatedTime = DateTimeOffset.UtcNow,
                        LastUpdatedTime = DateTimeOffset.UtcNow,
                        CreatedBy = currentUserId,
                        LastUpdatedBy = currentUserId
                    };
                    await _unitOfWork.WorkshopRepository.AddAsync(workshop);
                    createdWorkshopId = workshop.Id;

                    if (wReq.TicketTypes != null && wReq.TicketTypes.Any())
                    {
                        foreach (var tt in wReq.TicketTypes)
                        {
                            var ticket = new WorkshopTicketType
                            {
                                WorkshopId = workshop.Id,
                                Type = tt.Type,
                                Name = tt.Name,
                                Price = tt.Price,
                                IsCombo = tt.IsCombo,
                                DurationMinutes = tt.DurationMinutes,
                                Content = tt.Content,
                                CreatedTime = DateTimeOffset.UtcNow,
                                LastUpdatedTime = DateTimeOffset.UtcNow,
                                CreatedBy = currentUserId,
                                LastUpdatedBy = currentUserId
                            };
                            await _unitOfWork.WorkshopTicketTypeRepository.AddAsync(ticket);

                            if (tt.WorkshopActivities != null && tt.WorkshopActivities.Any())
                            {
                                foreach (var act in tt.WorkshopActivities.OrderBy(a => a.ActivityOrder))
                                {
                                    var activity = new WorkshopActivity
                                    {
                                        WorkshopTicketTypeId = ticket.Id,
                                        Activity = act.Activity,
                                        Description = act.Description ?? string.Empty,
                                        DurationMinutes = act.DurationMinutes,
                                        ActivityOrder = act.ActivityOrder,
                                        CreatedTime = DateTimeOffset.UtcNow,
                                        LastUpdatedTime = DateTimeOffset.UtcNow,
                                        CreatedBy = currentUserId,
                                        LastUpdatedBy = currentUserId
                                    };
                                    await _unitOfWork.WorkshopActivityRepository.AddAsync(activity);
                                }
                            }
                        }
                    }

                    if (wReq.RecurringRules != null && wReq.RecurringRules.Any())
                    {
                        foreach (var rr in wReq.RecurringRules)
                        {
                            var rule = new WorkshopRecurringRule
                            {
                                WorkshopId = workshop.Id,
                                DaysOfWeek = rr.DaysOfWeek ?? new List<DayOfWeek>(),
                                CreatedTime = DateTimeOffset.UtcNow,
                                LastUpdatedTime = DateTimeOffset.UtcNow,
                                CreatedBy = currentUserId,
                                LastUpdatedBy = currentUserId
                            };
                            await _unitOfWork.WorkshopRecurringRuleRepository.AddAsync(rule);

                            if (rr.Sessions != null && rr.Sessions.Any())
                            {
                                foreach (var s in rr.Sessions)
                                {
                                    var sessionRule = new WorkshopSessionRule
                                    {
                                        RecurringRuleId = rule.Id,
                                        StartTime = s.StartTime,
                                        EndTime = s.EndTime,
                                        Capacity = s.Capacity,
                                        CreatedTime = DateTimeOffset.UtcNow,
                                        LastUpdatedTime = DateTimeOffset.UtcNow,
                                        CreatedBy = currentUserId,
                                        LastUpdatedBy = currentUserId
                                    };
                                    await _unitOfWork.WorkshopSessionRuleRepository.AddAsync(sessionRule);
                                }
                            }
                        }
                    }

                    if (wReq.Exceptions != null && wReq.Exceptions.Any())
                    {
                        foreach (var ex in wReq.Exceptions)
                        {
                            var exMain = new WorkshopException
                            {
                                WorkshopId = workshop.Id,
                                Date = ex.Date,
                                Reason = ex.Reason,
                                IsActive = ex.IsActive,
                                CreatedTime = DateTimeOffset.UtcNow,
                                LastUpdatedTime = DateTimeOffset.UtcNow,
                                CreatedBy = currentUserId,
                                LastUpdatedBy = currentUserId
                            };
                            await _unitOfWork.WorkshopExceptionRepository.AddAsync(exMain);
                        }
                    }

                    var today = DateTime.UtcNow.Date;
                    var generationHorizonDays = 90;
                    var horizonEnd = today.AddDays(generationHorizonDays);

                    if (wReq.RecurringRules != null && wReq.RecurringRules.Any())
                    {
                        var exceptionDates = (wReq.Exceptions ?? new List<WorkshopExceptionRequest>())
                                             .Where(e => e.IsActive)
                                             .Select(e => e.Date.Date)
                                             .ToHashSet();

                        foreach (var rr in wReq.RecurringRules)
                        {
                            if (rr.Sessions == null || rr.Sessions.Count == 0) continue;

                            var days = rr.DaysOfWeek ?? new List<DayOfWeek>();
                            if (days.Count == 0) continue;

                            var occurrenceDates = ExpandDatesByDaysOfWeek(today, horizonEnd, days)
                                .Where(d => !exceptionDates.Contains(d));

                            foreach (var d in occurrenceDates)
                            {
                                foreach (var s in rr.Sessions)
                                {
                                    DateTime start = d.Add(s.StartTime);
                                    DateTime end = (s.EndTime > s.StartTime)
                                                        ? d.Add(s.EndTime)
                                                        : d.AddDays(1).Add(s.EndTime);

                                    if (end <= start) end = start.AddMinutes(1);

                                    bool exists = await _unitOfWork.WorkshopScheduleRepository.ActiveEntities
                                        .AnyAsync(x => x.WorkshopId == workshop.Id &&
                                                       x.StartTime == start &&
                                                       x.EndTime == end, cancellationToken);
                                    if (exists) continue;

                                    var schedule = new WorkshopSchedule
                                    {
                                        WorkshopId = workshop.Id,
                                        StartTime = start,
                                        EndTime = end,
                                        Capacity = s.Capacity > 0 ? s.Capacity : 1,
                                        CurrentBooked = 0,
                                        Status = ScheduleStatus.Active,
                                        Notes = null,
                                        CreatedTime = DateTimeOffset.UtcNow,
                                        LastUpdatedTime = DateTimeOffset.UtcNow,
                                        CreatedBy = currentUserId,
                                        LastUpdatedBy = currentUserId
                                    };

                                    await _unitOfWork.WorkshopScheduleRepository.AddAsync(schedule);
                                }
                            }
                        }
                    }

                    if (wReq.Medias != null && wReq.Medias.Any())
                    {
                        foreach (var media in wReq.Medias)
                        {
                            var workshopMedia = new WorkshopMedia
                            {
                                WorkshopId = workshop.Id,
                                MediaUrl = media.MediaUrl,
                                FileName = Path.GetFileName(media.MediaUrl),
                                FileType = Path.GetExtension(media.MediaUrl),
                                SizeInBytes = 0,
                                IsThumbnail = media.IsThumbnail,
                                CreatedTime = DateTimeOffset.UtcNow,
                                LastUpdatedTime = DateTimeOffset.UtcNow,
                                CreatedBy = currentUserId,
                                LastUpdatedBy = currentUserId
                            };
                            await _unitOfWork.WorkshopMediaRepository.AddAsync(workshopMedia);
                        }
                    }

                }
                user.VerificationToken = string.Empty;
                user.ResetToken = string.Empty;
                user.VerificationTokenExpires = currentTime.AddYears(-1);
                user.ResetTokenExpires = currentTime.AddYears(-1);
                _unitOfWork.UserRepository.Update(user);
            }

            _unitOfWork.CraftVillageRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            // Email thông báo
            if (model.Status == CraftVillageRequestStatus.Approved)
            {
                await _emailService.SendEmailAsync(
                    new[] { user.Email },
                    "Yêu cầu duyệt làng nghề được chấp nhận",
                    "Yêu cầu duyệt làng nghề được chấp nhận"
                );
            }
            else if (model.Status == CraftVillageRequestStatus.Rejected)
            {
                await _emailService.SendEmailAsync(
                    new[] { user.Email },
                    "Yêu cầu duyệt làng nghề đã bị từ chối",
                    model.RejectionReason ?? "Yêu cầu bị từ chối"
                );
            }

            var response = MapToCraftVillageRequestResponseDto(request, user);
            response.OwnerEmail = user.Email;
            response.OwnerFullName = user.FullName;

            return response;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CraftVillageRequestResponseDto> GetCraftVillageRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .AsNoTracking()
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.TicketTypes)
                        .ThenInclude(tt => tt.WorkshopActivities)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.RecurringRules)
                        .ThenInclude(rr => rr.Sessions)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.Exceptions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

            if (request == null)
                throw CustomExceptionFactory.CreateNotFoundError("CraftVillage request");

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);

            var response = MapToCraftVillageRequestResponseDto(request, user);
            response.OwnerEmail = user?.Email ?? string.Empty;
            response.OwnerFullName = user?.FullName ?? string.Empty;
            response.Medias = request.Medias.ToMediaDto();
            response.Workshop = MapWorkshopRequestToResponseDto(request.Workshop);

            return response;
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<PagedResult<CraftVillageRequestResponseDto>> GetMyCraftVillageRequestsAsync(
        CraftVillageRequestStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var baseQuery = _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .Where(r => r.OwnerId == currentUserId)
                .AsNoTracking()
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.TicketTypes)
                        .ThenInclude(tt => tt.WorkshopActivities)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.RecurringRules)
                        .ThenInclude(rr => rr.Sessions)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.Exceptions)
                .AsSplitQuery();

            if (status.HasValue)
                baseQuery = baseQuery.Where(x => x.Status == status.Value);

            var query = baseQuery.OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var owner = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId, cancellationToken);

            var response = new List<CraftVillageRequestResponseDto>(items.Count);
            foreach (var request in items)
            {
                var dto = MapToCraftVillageRequestResponseDto(request, owner);
                dto.OwnerEmail = owner?.Email ?? string.Empty;
                dto.OwnerFullName = owner?.FullName ?? string.Empty;
                dto.Medias = request.Medias.ToMediaDto();
                dto.Workshop = MapWorkshopRequestToResponseDto(request.Workshop);
                response.Add(dto);
            }

            return new PagedResult<CraftVillageRequestResponseDto>
            {
                Items = response,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CraftVillageRequestResponseDto> GetLatestCraftVillageRequestsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var latestRequest = await _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .AsNoTracking()
                .Where(x => x.OwnerId == userId)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.TicketTypes)
                        .ThenInclude(tt => tt.WorkshopActivities)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.RecurringRules)
                        .ThenInclude(rr => rr.Sessions)
                .Include(r => r.Workshop)
                    .ThenInclude(w => w.Exceptions)
                .AsSplitQuery()
                .OrderByDescending(x => x.CreatedTime)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestRequest == null)
                return null;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(latestRequest.OwnerId, cancellationToken);

            var response = MapToCraftVillageRequestResponseDto(latestRequest, user);
            response.OwnerEmail = user?.Email ?? string.Empty;
            response.OwnerFullName = user?.FullName ?? string.Empty;
            response.Medias = latestRequest.Medias.ToMediaDto();
            response.Workshop = MapWorkshopRequestToResponseDto(latestRequest.Workshop);

            return response;
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> DeleteCraftVillageRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentUserIdString = currentUserId.ToString();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("CraftVillageRequest");

            if (request.Status != CraftVillageRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa yêu cầu đã xử lý");

            if (request.OwnerId != currentUserId)
                throw CustomExceptionFactory.CreateForbiddenError();

            request.IsDeleted = true;
            request.LastUpdatedTime = currentTime;
            request.LastUpdatedBy = currentUserIdString;

            _unitOfWork.CraftVillageRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    private TourGuideRequestResponseDto MapToTourGuideRequestResponseDto(TourGuideRequest tourGuideRequest, User user)
    {
        return new TourGuideRequestResponseDto
        {
            Id = tourGuideRequest.Id,
            UserId = tourGuideRequest.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Introduction = tourGuideRequest.Introduction,
            Price = tourGuideRequest.Price,
            Status = tourGuideRequest.Status,
            RejectionReason = tourGuideRequest.RejectionReason,
            Certifications = tourGuideRequest.Certifications.Select(c => new CertificationDto
            {
                Name = c.Name,
                CertificateUrl = c.CertificateUrl
            }).ToList()
        };
    }

    private CraftVillageRequestResponseDto MapToCraftVillageRequestResponseDto(CraftVillageRequest request, User user)
    {
        var statusText = _enumService.GetEnumDisplayName<CraftVillageRequestStatus>(request.Status) ?? string.Empty;
        return new CraftVillageRequestResponseDto
        {
            Id = request.Id,
            OwnerId = request.OwnerId,
            OwnerEmail = user?.Email ?? string.Empty,
            OwnerFullName = user?.FullName ?? string.Empty,
            Name = request.Name,
            Description = request.Description,
            Content = request.Content,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            DistrictId = request.DistrictId,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Website = request.Website,
            WorkshopsAvailable = request.WorkshopsAvailable,
            SignatureProduct = request.SignatureProduct,
            YearsOfHistory = request.YearsOfHistory,
            IsRecognizedByUnesco = request.IsRecognizedByUnesco,
            Status = request.Status,
            StatusText = statusText,
            RejectionReason = request.RejectionReason,
            ReviewedAt = request.ReviewedAt,
            ReviewedBy = request.ReviewedBy,
            Medias = request.Medias.ToMediaDto()
        };
    }

    private static WorkshopRequestResponseDto? MapWorkshopRequestToResponseDto(WorkshopRequest? src)
    {
        if (src is null) return null;

        return new WorkshopRequestResponseDto
        {
            Id = src.Id,
            CraftVillageRequestId = src.CraftVillageId,
            Name = src.Name,
            Description = src.Description,
            Content = src.Content,
            Status = src.Status,
            CreatedTime = src.CreatedTime,
            LastUpdatedTime = src.LastUpdatedTime,
            CreatedBy = src.CreatedBy,
            LastUpdatedBy = src.LastUpdatedBy,

            TicketTypes = (src.TicketTypes ?? Enumerable.Empty<WorkshopTicketTypeRequest>())
                .Select(tt => new TicketTypeRequestResponseDto
                {
                    Id = tt.Id,
                    Type = tt.Type,
                    Name = tt.Name,
                    Price = tt.Price,
                    IsCombo = tt.IsCombo,
                    DurationMinutes = tt.DurationMinutes,
                    Content = tt.Content,
                    WorkshopActivities = (tt.WorkshopActivities ?? Enumerable.Empty<WorkshopActivityRequest>())
                        .OrderBy(a => a.ActivityOrder)
                        .Select(a => new WorkshopActivityRequestResponseDto
                        {
                            Id = a.Id,
                            Activity = a.Activity,
                            Description = a.Description,
                            DurationMinutes = a.DurationMinutes,
                            ActivityOrder = a.ActivityOrder
                        }).ToList()
                }).ToList(),

            RecurringRules = (src.RecurringRules ?? Enumerable.Empty<WorkshopRecurringRuleRequest>())
                .Select(rr => new RecurringRuleRequestResponseDto
                {
                    Id = rr.Id,
                    DaysOfWeek = rr.DaysOfWeek ?? new List<DayOfWeek>(),
                    Sessions = (rr.Sessions ?? Enumerable.Empty<WorkshopSessionRequest>())
                        .Select(s => new SessionRequestResponseDto
                        {
                            Id = s.Id,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Capacity = s.Capacity
                        }).ToList()
                }).ToList(),

            Exceptions = (src.Exceptions ?? Enumerable.Empty<WorkshopExceptionRequest>())
                .Select(ex => new WorkshopExceptionRequestResponseDto
                {
                    Id = ex.Id,
                    Date = ex.Date,
                    Reason = ex.Reason,
                    IsActive = ex.IsActive
                }).ToList()
        };
    }

    // ngày trong một DayOfWeek trong 
    private static IEnumerable<DateTime> DatesForDayOfWeek(DateTime start, DateTime end, DayOfWeek dow)
    {
        start = start.Date; end = end.Date;
        // offset để tìm ngày đầu tiên có DayOfWeek = dow
        int offset = ((int)dow - (int)start.DayOfWeek + 7) % 7;
        var first = start.AddDays(offset);
        for (var d = first; d <= end; d = d.AddDays(7))
            yield return d;
    }

    private static IEnumerable<DateTime> ExpandDatesByDaysOfWeek(DateTime start, DateTime end, IEnumerable<DayOfWeek> days)
    {
        var set = new HashSet<DateTime>();
        foreach (var dow in days.Distinct())
            foreach (var d in DatesForDayOfWeek(start, end, dow))
                set.Add(d);
        return set.OrderBy(d => d);
    }

    private string GenerateRandomPassword()
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        var password = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[12];
            rng.GetBytes(bytes);
            foreach (var b in bytes)
            {
                password.Append(validChars[b % validChars.Length]);
            }
        }
        return password.ToString();
    }
}
