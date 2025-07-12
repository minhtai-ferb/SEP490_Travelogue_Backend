using AutoMapper;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.RoleModels.Requests;
using Travelogue.Service.BusinessModels.RoleModels.Responses;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IRoleService
{
    Task<bool> CreateRole(RoleRequestModel request, CancellationToken cancellationToken);
    Task<RoleResponseModel> GetRoleById(Guid request, CancellationToken cancellationToken);
    Task<PagedResult<RoleResponseModel>> GetPagedRolesWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken);
}

public sealed class RoleService : IRoleService
{
    //private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        this._unitOfWork = unitOfWork;
        this._mapper = mapper;
        this._userContextService = userContextService;
        this._timeService = timeService;
    }

    public async Task<bool> CreateRole(RoleRequestModel request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var result = await _unitOfWork.RoleRepository
                .AddAsync(new Role(request.Name.ToLower())
                {
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });
            //if (!result.Succeeded)
            //{
            //    throw new CustomException(StatusCodes.Status409Conflict, ResponseCodeConstants.BAD_REQUEST, ResponseMessages.DUPLICATE);
            //}

            if (!request.DistrictId.HasValue)
            {
                return result != null;
            }

            var addRoleDistrict = await _unitOfWork.RoleDistrictRepository
                .AddAsync(new RoleDistrict
                {
                    RoleId = result.Id,
                    DistrictId = request.DistrictId.Value,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                });

            return result != null;
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

    public async Task<RoleResponseModel> GetRoleById(Guid request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(request, cancellationToken);
            if (role == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("role");
            }

            return _mapper.Map<RoleResponseModel>(role);
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

    public async Task<PagedResult<RoleResponseModel>> GetPagedRolesWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.RoleRepository.GetPageRoleAsync(pageNumber, pageSize, name, cancellationToken);

            var roleDataModels = _mapper.Map<List<RoleResponseModel>>(pagedResult.Items);

            foreach (var role in roleDataModels)
            {
                if (role.Id != null)
                {
                    role.DistrictId = await _unitOfWork.RoleDistrictRepository.GetDistrictIdByRoleId(role.Id);
                    role.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(role.DistrictId);
                }
                else
                {
                    role.DistrictId = Guid.Empty;
                    role.DistrictName = null;
                }
            }

            return new PagedResult<RoleResponseModel>
            {
                Items = roleDataModels,
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
}
