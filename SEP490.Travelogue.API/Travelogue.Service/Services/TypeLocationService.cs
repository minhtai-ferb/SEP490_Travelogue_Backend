using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Caching;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeLocationModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITypeLocationService
{
    Task<TypeLocationDataModel?> GetTypeLocationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TypeLocationDataModel>> GetAllTypeLocationsAsync(CancellationToken cancellationToken);
    Task AddTypeLocationAsync(TypeLocationCreateModel typeLocationCreateModel, CancellationToken cancellationToken);
    Task UpdateTypeLocationAsync(Guid id, TypeLocationUpdateModel typeLocationUpdateModel, CancellationToken cancellationToken);
    Task DeleteTypeLocationAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TypeLocationDataModel>> GetPagedTypeLocationsAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TypeLocationService : ITypeLocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICacheService _cacheService;

    public TypeLocationService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        this._cacheService = cacheService;
    }

    public async Task AddTypeLocationAsync(TypeLocationCreateModel typeLocationCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(typeLocationCreateModel.Name).ToLower();

            var allCategories = await _unitOfWork.TypeLocationRepository
                .Entities
                .ToListAsync(cancellationToken);

            var matchedCategory = allCategories.FirstOrDefault(x =>
                Helper.RemoveVietnameseTone(x.Name).ToLower() == inputNameNormalized);

            if (matchedCategory != null)
            {
                if (!matchedCategory.IsDeleted)
                {
                    throw CustomExceptionFactory.CreateBadRequest("Danh mục đã tồn tại trong hệ thống.");
                }

                matchedCategory.IsDeleted = false;
                matchedCategory.Name = typeLocationCreateModel.Name;
                matchedCategory.LastUpdatedBy = currentUserId;
                matchedCategory.LastUpdatedTime = currentTime;

                _unitOfWork.BeginTransaction();
                _unitOfWork.TypeLocationRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newTypeLocation = _mapper.Map<TypeLocation>(typeLocationCreateModel);
            newTypeLocation.CreatedBy = currentUserId;
            newTypeLocation.LastUpdatedBy = currentUserId;
            newTypeLocation.CreatedTime = currentTime;
            newTypeLocation.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TypeLocationRepository.AddAsync(newTypeLocation);
            _unitOfWork.CommitTransaction();
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
            _unitOfWork.Dispose();
        }
    }

    public async Task DeleteTypeLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingTypeLocation = await _unitOfWork.TypeLocationRepository.GetByIdAsync(id, cancellationToken);

            if (existingTypeLocation == null || existingTypeLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type location");
            }

            var isInUsing = await _unitOfWork.LocationRepository.ActiveEntities.FirstOrDefaultAsync(e => e.TypeLocationId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            }

            existingTypeLocation.LastUpdatedBy = currentUserId;
            existingTypeLocation.DeletedBy = currentUserId;
            existingTypeLocation.DeletedTime = currentTime;
            existingTypeLocation.LastUpdatedTime = currentTime;
            existingTypeLocation.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeLocationRepository.Update(existingTypeLocation);
            _unitOfWork.CommitTransaction();
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<List<TypeLocationDataModel>> GetAllTypeLocationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeLocation = await _unitOfWork.TypeLocationRepository.GetAllAsync(cancellationToken);
            if (existingTypeLocation == null || existingTypeLocation.Count() == 0)
            {
                return new List<TypeLocationDataModel>();
            }

            return _mapper.Map<List<TypeLocationDataModel>>(existingTypeLocation);
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<TypeLocationDataModel?> GetTypeLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"typeLocation:{id}";
            var cachedTypeLocations = await _cacheService.GetAsync<TypeLocationDataModel>(cacheKey);

            if (cachedTypeLocations is not null)
            {
                return cachedTypeLocations;
            }

            var existingTypeLocation = await _unitOfWork.TypeLocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeLocation == null || existingTypeLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type location");
            }

            var result = _mapper.Map<TypeLocationDataModel>(existingTypeLocation);

            await _cacheService.SetAsync(cacheKey, result);

            return result;
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
            _unitOfWork.Dispose();
        }
    }

    public async Task UpdateTypeLocationAsync(Guid id, TypeLocationUpdateModel typeLocationUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingTypeLocation = await _unitOfWork.TypeLocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeLocation == null || existingTypeLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type location");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(typeLocationUpdateModel.Name).ToLower();

            var duplicateExists = _unitOfWork.TypeLocationRepository
                .ActiveEntities
                .Where(x => x.Id != id && !x.IsDeleted)
                .AsEnumerable()
                .Any(x => Helper.RemoveVietnameseTone(x.Name).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequest("Tên danh mục đã tồn tại trong hệ thống.");
            }

            _mapper.Map(typeLocationUpdateModel, existingTypeLocation);

            existingTypeLocation.LastUpdatedBy = currentUserId;
            existingTypeLocation.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeLocationRepository.Update(existingTypeLocation);
            _unitOfWork.CommitTransaction();
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
            _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<TypeLocationDataModel>> GetPagedTypeLocationsAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeLocationRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var typeExperienceDataModels = _mapper.Map<List<TypeLocationDataModel>>(pagedResult.Items);

            return new PagedResult<TypeLocationDataModel>
            {
                Items = typeExperienceDataModels,
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
            _unitOfWork.Dispose();
        }
    }
}