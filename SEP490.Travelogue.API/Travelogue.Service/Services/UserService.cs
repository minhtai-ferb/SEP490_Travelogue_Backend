using AutoMapper;
using Microsoft.AspNetCore.Http;
using Travelogue.Repository.Bases;
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

public interface IUserService
{
    Task<UserResponseModel> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<UserResponseModel>> GetAllUsersAsync(string? searchFullName = null, CancellationToken cancellationToken = default);
    Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<UserResponseModel>> GetPagedUsersAsync(string? email, string? phoneNumber, string? fullName, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> BlockUserAsync(Guid userId);
    Task<bool> UnblockUserAsync(Guid userId);
    Task<bool> UpdateUserRolesAsync(Guid userId, List<Guid> roleIds);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid districtId, string roleName);
    Task<UserResponseModel> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken);
    Task<bool> RemoveUserFromRole(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task<bool> SendFeedbackAsync(FeedbackModel model, CancellationToken cancellationToken);
    //Task GetPagedUsersWithSearchAsync(int pageNumber, int pageSize, string email, string phoneNumber, string fullName, CancellationToken cancellationToken);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _timeService;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IEmailService _emailService;
    private readonly int YEARS_TO_BLOCK = 30;

    public UserService(IUnitOfWork unitOfWork, ITimeService timeService, IMapper mapper, IUserContextService userContextService, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _timeService = timeService;
        _mapper = mapper;
        _userContextService = userContextService;
        _emailService = emailService;
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
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<List<UserResponseModel>> GetAllUsersAsync(string? searchFullName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);

            if (!string.IsNullOrEmpty(searchFullName))
            {
                users = users.Where(u => u.FullName.Contains(searchFullName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (users == null || !users.Any())
            {
                return new List<UserResponseModel>();
            }

            var userDataModels = _mapper.Map<List<UserResponseModel>>(users);

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

            return userDataModels;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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

            // check role có district chưa
            var districtRoleExists = await _unitOfWork.RoleDistrictRepository.ExistsAsync(districtId, role.Id);
            if (!userRoleExists)
            {
                var roleDistrict = new RoleDistrict { DistrictId = districtId, RoleId = role.Id };
                await _unitOfWork.RoleDistrictRepository.AddAsync(roleDistrict);
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
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }
}
