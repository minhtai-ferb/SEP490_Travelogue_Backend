
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeEventModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITypeEventService
{
    Task<TypeEventDataModel?> GetTypeEventByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TypeEventDataModel>> GetAllTypeEventsAsync(CancellationToken cancellationToken);
    Task AddTypeEventAsync(TypeEventCreateModel typeEventCreateModel, CancellationToken cancellationToken);
    Task UpdateTypeEventAsync(Guid id, TypeEventUpdateModel typeEventUpdateModel, CancellationToken cancellationToken);
    Task DeleteTypeEventAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TypeEventDataModel>> GetPagedTypeEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<TypeEventDataModel>> GetPagedTypeEventsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TypeEventService : ITypeEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TypeEventService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task AddTypeEventAsync(TypeEventCreateModel typeEventCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(typeEventCreateModel.TypeName).ToLower();

            var allCategories = await _unitOfWork.TypeEventRepository
                .Entities
                .ToListAsync(cancellationToken);

            var matchedCategory = allCategories.FirstOrDefault(x =>
                Helper.RemoveVietnameseTone(x.TypeName).ToLower() == inputNameNormalized);

            if (matchedCategory != null)
            {
                if (!matchedCategory.IsDeleted)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Danh mục đã tồn tại trong hệ thống.");
                }

                matchedCategory.IsDeleted = false;
                matchedCategory.TypeName = typeEventCreateModel.TypeName;
                matchedCategory.LastUpdatedBy = currentUserId;
                matchedCategory.LastUpdatedTime = currentTime;

                _unitOfWork.BeginTransaction();
                _unitOfWork.TypeEventRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newTypeEvent = _mapper.Map<TypeEvent>(typeEventCreateModel);
            newTypeEvent.CreatedBy = currentUserId;
            newTypeEvent.LastUpdatedBy = currentUserId;
            newTypeEvent.CreatedTime = currentTime;
            newTypeEvent.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TypeEventRepository.AddAsync(newTypeEvent);
            _unitOfWork.CommitTransaction();
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

    public async Task DeleteTypeEventAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingTypeEvent = await _unitOfWork.TypeEventRepository.GetByIdAsync(id, cancellationToken);

            if (existingTypeEvent == null || existingTypeEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var isInUsing = await _unitOfWork.EventRepository.ActiveEntities.FirstOrDefaultAsync(e => e.TypeEventId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            existingTypeEvent.LastUpdatedBy = currentUserId;
            existingTypeEvent.DeletedBy = currentUserId;
            existingTypeEvent.DeletedTime = currentTime;
            existingTypeEvent.LastUpdatedTime = currentTime;
            existingTypeEvent.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeEventRepository.Update(existingTypeEvent);
            _unitOfWork.CommitTransaction();
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

    public async Task<List<TypeEventDataModel>> GetAllTypeEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeEvent = await _unitOfWork.TypeEventRepository.GetAllAsync(cancellationToken);
            if (existingTypeEvent == null || existingTypeEvent.Count() == 0)
            {
                return new List<TypeEventDataModel>();
            }

            return _mapper.Map<List<TypeEventDataModel>>(existingTypeEvent);
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

    public async Task<TypeEventDataModel?> GetTypeEventByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeEvent = await _unitOfWork.TypeEventRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeEvent == null || existingTypeEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeEventDataModel>(existingTypeEvent);
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

    public async Task<TypeEventDataModel?> GetTypeEventByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeEvent = await _unitOfWork.TypeEventRepository.GetByNameAsync(name, cancellationToken);
            if (existingTypeEvent == null || existingTypeEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeEventDataModel>(existingTypeEvent);
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

    public async Task<PagedResult<TypeEventDataModel>> GetPagedTypeEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeEventRepository.GetPageAsync(pageNumber, pageSize);

            var typeEventDataModels = _mapper.Map<List<TypeEventDataModel>>(pagedResult.Items);

            return new PagedResult<TypeEventDataModel>
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<TypeEventDataModel>> GetPagedTypeEventsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeEventRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var typeEventDataModels = _mapper.Map<List<TypeEventDataModel>>(pagedResult.Items);

            return new PagedResult<TypeEventDataModel>
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task UpdateTypeEventAsync(Guid id, TypeEventUpdateModel typeEventUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingTypeEvent = await _unitOfWork.TypeEventRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeEvent == null || existingTypeEvent.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(typeEventUpdateModel.TypeName).ToLower();

            var duplicateExists = _unitOfWork.TypeEventRepository
                 .ActiveEntities
                 .Where(x => x.Id != id && !x.IsDeleted)
                 .AsEnumerable()
                 .Any(x => Helper.RemoveVietnameseTone(x.TypeName).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tên danh mục đã tồn tại trong hệ thống.");
            }

            _mapper.Map(typeEventUpdateModel, existingTypeEvent);

            existingTypeEvent.LastUpdatedBy = currentUserId;
            existingTypeEvent.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeEventRepository.Update(existingTypeEvent);
            _unitOfWork.CommitTransaction();
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