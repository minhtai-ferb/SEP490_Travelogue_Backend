using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourTypeModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourTypeService
{
    Task<TourTypeDataModel?> GetTourTypeByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TourTypeDataModel>> GetAllTourTypesAsync(CancellationToken cancellationToken);
    Task AddTourTypeAsync(TourTypeCreateModel typeEventCreateModel, CancellationToken cancellationToken);
    Task UpdateTourTypeAsync(Guid id, TourTypeUpdateModel typeEventUpdateModel, CancellationToken cancellationToken);
    Task DeleteTourTypeAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TourTypeDataModel>> GetPagedTourTypesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<TourTypeDataModel>> GetPagedTourTypesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TourTypeService : ITourTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TourTypeService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task AddTourTypeAsync(TourTypeCreateModel typeEventCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(typeEventCreateModel.Name).ToLower();

            var allCategories = await _unitOfWork.TourTypeRepository
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
                _unitOfWork.TourTypeRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newTourType = _mapper.Map<TourType>(typeEventCreateModel);
            newTourType.CreatedBy = currentUserId;
            newTourType.LastUpdatedBy = currentUserId;
            newTourType.CreatedTime = currentTime;
            newTourType.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TourTypeRepository.AddAsync(newTourType);
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

    public async Task DeleteTourTypeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingTourType = await _unitOfWork.TourTypeRepository.GetByIdAsync(id, cancellationToken);

            if (existingTourType == null || existingTourType.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var isInUsing = await _unitOfWork.TourRepository.ActiveEntities.FirstOrDefaultAsync(e => e.TourTypeId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            existingTourType.LastUpdatedBy = currentUserId;
            existingTourType.DeletedBy = currentUserId;
            existingTourType.DeletedTime = currentTime;
            existingTourType.LastUpdatedTime = currentTime;
            existingTourType.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TourTypeRepository.Update(existingTourType);
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

    public async Task<List<TourTypeDataModel>> GetAllTourTypesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTourType = await _unitOfWork.TourTypeRepository.GetAllAsync(cancellationToken);
            if (existingTourType == null || existingTourType.Count() == 0)
            {
                return new List<TourTypeDataModel>();
            }

            return _mapper.Map<List<TourTypeDataModel>>(existingTourType);
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

    public async Task<TourTypeDataModel?> GetTourTypeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingTourType = await _unitOfWork.TourTypeRepository.GetByIdAsync(id, cancellationToken);
            if (existingTourType == null || existingTourType.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TourTypeDataModel>(existingTourType);
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

    public async Task<TourTypeDataModel?> GetTourTypeByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingTourType = await _unitOfWork.TourTypeRepository.GetByNameAsync(name, cancellationToken);
            if (existingTourType == null || existingTourType.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TourTypeDataModel>(existingTourType);
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

    public async Task<PagedResult<TourTypeDataModel>> GetPagedTourTypesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TourTypeRepository.GetPageAsync(pageNumber, pageSize);

            var typeEventDataModels = _mapper.Map<List<TourTypeDataModel>>(pagedResult.Items);

            return new PagedResult<TourTypeDataModel>
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

    public async Task<PagedResult<TourTypeDataModel>> GetPagedTourTypesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TourTypeRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var typeEventDataModels = _mapper.Map<List<TourTypeDataModel>>(pagedResult.Items);

            return new PagedResult<TourTypeDataModel>
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

    public async Task UpdateTourTypeAsync(Guid id, TourTypeUpdateModel typeEventUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingTourType = await _unitOfWork.TourTypeRepository.GetByIdAsync(id, cancellationToken);
            if (existingTourType == null || existingTourType.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(typeEventUpdateModel.Name).ToLower();

            var duplicateExists = _unitOfWork.TourTypeRepository
                 .ActiveEntities
                 .Where(x => x.Id != id && !x.IsDeleted)
                 .AsEnumerable()
                 .Any(x => Helper.RemoveVietnameseTone(x.Name).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tên danh mục đã tồn tại trong hệ thống.");
            }

            _mapper.Map(typeEventUpdateModel, existingTourType);

            existingTourType.LastUpdatedBy = currentUserId;
            existingTourType.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TourTypeRepository.Update(existingTourType);
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