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
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.UserModels;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;
using Travelogue.Service.Commons.Const;
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
    Task<TourGuideRequestResponseDto> CreateTourGuideRequestAsync(CreateTourGuideRequestDto model, CancellationToken cancellationToken = default);
    Task<List<TourGuideRequestResponseDto>> GetTourGuideRequestsAsync(TourGuideRequestStatus? status, CancellationToken cancellationToken = default);
    Task<TourGuideRequestResponseDto> ReviewTourGuideRequestAsync(Guid requestId, ReviewTourGuideRequestDto model, CancellationToken cancellationToken = default);
    //Task GetPagedUsersWithSearchAsync(int pageNumber, int pageSize, string email, string phoneNumber, string fullName, CancellationToken cancellationToken);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _timeService;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IEmailService _emailService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly int YEARS_TO_BLOCK = 30;

    public UserService(IUnitOfWork unitOfWork, ITimeService timeService, IMapper mapper, IUserContextService userContextService, IEmailService emailService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _timeService = timeService;
        _mapper = mapper;
        _userContextService = userContextService;
        _emailService = emailService;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<UserResponseModel> CreateUserAsync(CreateUserDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "Email already exists");
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
                    throw new CustomException(
                        StatusCodes.Status400BadRequest,
                        ResponseCodeConstants.BAD_REQUEST,
                        "Invalid role");
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
                ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(model.RoleId, cancellationToken)
                ?? throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "Invalid role");

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
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("vai trò");
            }

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
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User");
            }

            var existingTourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(currentUserId);
            if (existingTourGuide != null)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "User is already a TourGuide");
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
                    ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User");
                var requestDto = new TourGuideRequestResponseDto
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Introduction = request.Introduction,
                    Price = request.Price,
                    Status = request.Status,
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

    public async Task<TourGuideRequestResponseDto> ReviewTourGuideRequestAsync(Guid requestId, ReviewTourGuideRequestDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.TourGuideRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "TourGuide request not found");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");
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
                    LastUpdatedBy = currentUserId
                };

                foreach (var cert in request.Certifications)
                {
                    tourGuide.Certifications.Add(new Certification
                    {
                        Name = cert.Name,
                        CertificateUrl = cert.CertificateUrl,
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
                    throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "TourGuide role not found");
                }

                var userRole = new UserRole
                {
                    UserId = request.UserId,
                    RoleId = role.Id
                };
                await _unitOfWork.UserRoleRepository.AddAsync(userRole);
            }

            _unitOfWork.TourGuideRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();

            await _emailService.SendEmailAsync(
                new[] { user.Email },
                $"Nâng cấp role thành công Tour guide",
                $"Tour guide"
            );

            var response = MapToTourGuideRequestResponseDto(request, user);
            response.Email = user.Email;
            response.FullName = user.FullName;
            // response.Certifications = _mapper.Map<List<CertificationDto>>(request.Certifications);

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
    }

    public async Task<CraftVillageRequestResponseDto> CreateCraftVillageRequestAsync(CreateCraftVillageRequestDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(currentUserId), cancellationToken);
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");
            }

            var existingCraftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(cv => cv.OwnerId == user.Id);
            if (existingCraftVillage != null)
            {
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "User is already associated with a CraftVillage");
            }

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
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                OwnerId = user.Id,
                WorkshopsAvailable = model.WorkshopsAvailable,
                SignatureProduct = model.SignatureProduct,
                YearsOfHistory = model.YearsOfHistory,
                IsRecognizedByUnesco = model.IsRecognizedByUnesco,
                Status = CraftVillageRequestStatus.Pending,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow,
                CreatedBy = currentUserId,
                LastUpdatedBy = currentUserId
            };

            await _unitOfWork.CraftVillageRequestRepository.AddAsync(craftVillageRequest);
            await _unitOfWork.SaveAsync();

            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                    new[] { moderator.Email },
                    "Yêu cầu duyệt làng nghề",
                    "Yêu cầu duyệt làng nghề"
                );
            }

            var response = MapToCraftVillageRequestResponseDto(craftVillageRequest, user);

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
    }

    public async Task<List<CraftVillageRequestResponseDto>> GetCraftVillageRequestsAsync(Guid? id, CraftVillageRequestStatus? status, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.CraftVillageRequestRepository
                .ActiveEntities
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(x => x.Id == id);
            }
            else if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }

            var requests = await query.ToListAsync(cancellationToken);
            var response = new List<CraftVillageRequestResponseDto>();

            foreach (var request in requests)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);
                var requestDto = MapToCraftVillageRequestResponseDto(request, user);
                // requestDto.OwnerEmail = user.Email ?? string.Empty;
                // requestDto.OwnerFullName = user.FullName ?? string.Empty;
                response.Add(requestDto);
            }

            return response;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<CraftVillageRequestResponseDto> ReviewCraftVillageRequestAsync(Guid requestId, ReviewCraftVillageRequestDto model, string moderatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var request = await _unitOfWork.CraftVillageRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "CraftVillage request not found");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");
            }

            request.Status = model.Status;
            request.ReviewedAt = DateTimeOffset.UtcNow;
            request.ReviewedBy = currentUserId;
            request.LastUpdatedTime = DateTimeOffset.UtcNow;
            request.LastUpdatedBy = currentUserId;
            if (model.Status == CraftVillageRequestStatus.Approved)
            {
                var location = new Location
                {
                    Name = request.Name,
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
                    CreatedBy = moderatorId,
                    LastUpdatedBy = moderatorId
                };

                await _unitOfWork.CraftVillageRepository.AddAsync(craftVillage);

                var role = await _unitOfWork.RoleRepository.GetByNameAsync("CraftVillage");
                if (role == null)
                {
                    throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BAD_REQUEST, "CraftVillage role not found");
                }

                var userRole = new UserRole
                {
                    UserId = request.OwnerId,
                    RoleId = role.Id
                };
                await _unitOfWork.UserRoleRepository.AddAsync(userRole);
            }

            _unitOfWork.CraftVillageRequestRepository.Update(request);
            await _unitOfWork.SaveAsync();

            await _emailService.SendEmailAsync(
                    new[] { user.Email },
                    "Yêu cầu duyệt làng nghề được chấp nhận",
                    "Yêu cầu duyệt làng nghề được chấp nhận"
                );

            var response = MapToCraftVillageRequestResponseDto(request, user);
            response.OwnerEmail = user.Email;
            response.OwnerFullName = user.FullName;

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
    }

    public async Task<CraftVillageRequestResponseDto> GetCraftVillageRequestAsync(string requestId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.CraftVillageRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "CraftVillage request not found");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.OwnerId, cancellationToken);
            var response = MapToCraftVillageRequestResponseDto(request, user);
            response.OwnerEmail = user?.Email ?? string.Empty;
            response.OwnerFullName = user?.FullName ?? string.Empty;

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

    public CraftVillageRequestResponseDto MapToCraftVillageRequestResponseDto(CraftVillageRequest request, User user)
    {
        return new CraftVillageRequestResponseDto
        {
            Id = request.Id,
            OwnerId = request.OwnerId,
            OwnerEmail = user.Email,
            OwnerFullName = user.FullName,
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
            RejectionReason = request.RejectionReason,
            ReviewedAt = request.ReviewedAt,
            ReviewedBy = request.ReviewedBy
        };
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
