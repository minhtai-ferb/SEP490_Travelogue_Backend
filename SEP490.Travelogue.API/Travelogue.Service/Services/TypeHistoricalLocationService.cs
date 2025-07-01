using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeHistoricalLocationModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITypeHistoricalLocationService
{
    Task<TypeHistoricalLocationDataModel?> GetTypeHistoricalLocationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TypeHistoricalLocationDataModel>> GetAllTypeHistoricalLocationsAsync(CancellationToken cancellationToken);
    Task AddTypeHistoricalLocationAsync(TypeHistoricalLocationCreateModel typeEventCreateModel, CancellationToken cancellationToken);
    Task UpdateTypeHistoricalLocationAsync(Guid id, TypeHistoricalLocationUpdateModel typeEventUpdateModel, CancellationToken cancellationToken);
    Task DeleteTypeHistoricalLocationAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TypeHistoricalLocationDataModel>> GetPagedTypeHistoricalLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<TypeHistoricalLocationDataModel>> GetPagedTypeHistoricalLocationsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TypeHistoricalLocationService : ITypeHistoricalLocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TypeHistoricalLocationService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task AddTypeHistoricalLocationAsync(TypeHistoricalLocationCreateModel typeEventCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(typeEventCreateModel.Name).ToLower();

            var allCategories = await _unitOfWork.TypeHistoricalLocationRepository
                .Entities
                .ToListAsync(cancellationToken);

            var matchedCategory = allCategories.FirstOrDefault(x =>
                Helper.RemoveVietnameseTone(x.Name).ToLower() == inputNameNormalized);

            if (matchedCategory != null)
            {
                if (!matchedCategory.IsDeleted)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Danh mục đã tồn tại trong hệ thống.");
                }

                matchedCategory.IsDeleted = false;
                matchedCategory.Name = typeEventCreateModel.Name;
                matchedCategory.LastUpdatedBy = currentUserId;
                matchedCategory.LastUpdatedTime = currentTime;

                _unitOfWork.BeginTransaction();
                _unitOfWork.TypeHistoricalLocationRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newTypeHistoricalLocation = _mapper.Map<TypeHistoricalLocation>(typeEventCreateModel);
            newTypeHistoricalLocation.CreatedBy = currentUserId;
            newTypeHistoricalLocation.LastUpdatedBy = currentUserId;
            newTypeHistoricalLocation.CreatedTime = currentTime;
            newTypeHistoricalLocation.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TypeHistoricalLocationRepository.AddAsync(newTypeHistoricalLocation);
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
            //  _unitOfWork.Dispose();
        }
    }

    public async Task DeleteTypeHistoricalLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingTypeHistoricalLocation = await _unitOfWork.TypeHistoricalLocationRepository.GetByIdAsync(id, cancellationToken);

            if (existingTypeHistoricalLocation == null || existingTypeHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var isInUsing = await _unitOfWork.HistoricalLocationRepository.ActiveEntities.FirstOrDefaultAsync(e => e.TypeHistoricalLocationId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            existingTypeHistoricalLocation.LastUpdatedBy = currentUserId;
            existingTypeHistoricalLocation.DeletedBy = currentUserId;
            existingTypeHistoricalLocation.DeletedTime = currentTime;
            existingTypeHistoricalLocation.LastUpdatedTime = currentTime;
            existingTypeHistoricalLocation.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeHistoricalLocationRepository.Update(existingTypeHistoricalLocation);
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
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<List<TypeHistoricalLocationDataModel>> GetAllTypeHistoricalLocationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeHistoricalLocation = await _unitOfWork.TypeHistoricalLocationRepository.GetAllAsync(cancellationToken);
            if (existingTypeHistoricalLocation == null || existingTypeHistoricalLocation.Count() == 0)
            {
                return new List<TypeHistoricalLocationDataModel>();
            }

            return _mapper.Map<List<TypeHistoricalLocationDataModel>>(existingTypeHistoricalLocation);
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

    public async Task<TypeHistoricalLocationDataModel?> GetTypeHistoricalLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeHistoricalLocation = await _unitOfWork.TypeHistoricalLocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeHistoricalLocation == null || existingTypeHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeHistoricalLocationDataModel>(existingTypeHistoricalLocation);
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

    public async Task<TypeHistoricalLocationDataModel?> GetTypeHistoricalLocationByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeHistoricalLocation = await _unitOfWork.TypeHistoricalLocationRepository.GetByNameAsync(name, cancellationToken);
            if (existingTypeHistoricalLocation == null || existingTypeHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeHistoricalLocationDataModel>(existingTypeHistoricalLocation);
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

    public async Task<PagedResult<TypeHistoricalLocationDataModel>> GetPagedTypeHistoricalLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeHistoricalLocationRepository.GetPageAsync(pageNumber, pageSize);

            var typeEventDataModels = _mapper.Map<List<TypeHistoricalLocationDataModel>>(pagedResult.Items);

            return new PagedResult<TypeHistoricalLocationDataModel>
            {
                Items = typeEventDataModels,
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

    public async Task<PagedResult<TypeHistoricalLocationDataModel>> GetPagedTypeHistoricalLocationsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeHistoricalLocationRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var typeEventDataModels = _mapper.Map<List<TypeHistoricalLocationDataModel>>(pagedResult.Items);

            return new PagedResult<TypeHistoricalLocationDataModel>
            {
                Items = typeEventDataModels,
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

    public async Task UpdateTypeHistoricalLocationAsync(Guid id, TypeHistoricalLocationUpdateModel typeEventUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingTypeHistoricalLocation = await _unitOfWork.TypeHistoricalLocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeHistoricalLocation == null || existingTypeHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(typeEventUpdateModel.Name).ToLower();

            var duplicateExists = _unitOfWork.TypeHistoricalLocationRepository
                 .ActiveEntities
                 .Where(x => x.Id != id && !x.IsDeleted)
                 .AsEnumerable()
                 .Any(x => Helper.RemoveVietnameseTone(x.Name).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tên danh mục đã tồn tại trong hệ thống.");
            }

            _mapper.Map(typeEventUpdateModel, existingTypeHistoricalLocation);

            existingTypeHistoricalLocation.LastUpdatedBy = currentUserId;
            existingTypeHistoricalLocation.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeHistoricalLocationRepository.Update(existingTypeHistoricalLocation);
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
            //  _unitOfWork.Dispose();
        }
    }
}