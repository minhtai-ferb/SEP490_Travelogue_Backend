using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeExperienceModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITypeExperienceService
{
    Task<TypeExperienceDataModel?> GetTypeExperienceByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TypeExperienceDataModel>> GetAllTypeExperiencesAsync(CancellationToken cancellationToken);
    Task AddTypeExperienceAsync(TypeExperienceCreateModel typeExperienceCreateModel, CancellationToken cancellationToken);
    Task UpdateTypeExperienceAsync(Guid id, TypeExperienceUpdateModel typeExperienceUpdateModel, CancellationToken cancellationToken);
    Task DeleteTypeExperienceAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TypeExperienceDataModel>> GetPagedTypeExperiencesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<TypeExperienceDataModel>> GetPagedTypeExperiencesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TypeExperienceService : ITypeExperienceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TypeExperienceService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task AddTypeExperienceAsync(TypeExperienceCreateModel typeExperienceCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(typeExperienceCreateModel.TypeName).ToLower();

            var allCategories = await _unitOfWork.TypeExperienceRepository
                .Entities
                .ToListAsync(cancellationToken);

            var matchedCategory = allCategories.FirstOrDefault(x =>
                Helper.RemoveVietnameseTone(x.TypeName).ToLower() == inputNameNormalized);

            if (matchedCategory != null)
            {
                if (!matchedCategory.IsDeleted)
                {
                    throw CustomExceptionFactory.CreateBadRequest("Danh mục đã tồn tại trong hệ thống.");
                }

                matchedCategory.IsDeleted = false;
                matchedCategory.TypeName = typeExperienceCreateModel.TypeName;
                matchedCategory.LastUpdatedBy = currentUserId;
                matchedCategory.LastUpdatedTime = currentTime;

                _unitOfWork.BeginTransaction();
                _unitOfWork.TypeExperienceRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newTypeExperience = _mapper.Map<TypeExperience>(typeExperienceCreateModel);
            newTypeExperience.CreatedBy = currentUserId;
            newTypeExperience.LastUpdatedBy = currentUserId;
            newTypeExperience.CreatedTime = currentTime;
            newTypeExperience.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TypeExperienceRepository.AddAsync(newTypeExperience);
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

    public async Task DeleteTypeExperienceAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingTypeExperience = await _unitOfWork.TypeExperienceRepository.GetByIdAsync(id, cancellationToken);

            if (existingTypeExperience == null || existingTypeExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var isInUsing = await _unitOfWork.ExperienceRepository.ActiveEntities.FirstOrDefaultAsync(e => e.TypeExperienceId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            }

            existingTypeExperience.LastUpdatedBy = currentUserId;
            existingTypeExperience.DeletedBy = currentUserId;
            existingTypeExperience.DeletedTime = currentTime;
            existingTypeExperience.LastUpdatedTime = currentTime;
            existingTypeExperience.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeExperienceRepository.Update(existingTypeExperience);
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

    public async Task<List<TypeExperienceDataModel>> GetAllTypeExperiencesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeExperience = await _unitOfWork.TypeExperienceRepository.GetAllAsync(cancellationToken);
            if (existingTypeExperience == null || existingTypeExperience.Count() == 0)
            {
                return new List<TypeExperienceDataModel>();
            }

            return _mapper.Map<List<TypeExperienceDataModel>>(existingTypeExperience);
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

    public async Task<TypeExperienceDataModel?> GetTypeExperienceByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeExperience = await _unitOfWork.TypeExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeExperience == null || existingTypeExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeExperienceDataModel>(existingTypeExperience);
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

    public async Task<TypeExperienceDataModel?> GetTypeExperienceByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingTypeExperience = await _unitOfWork.TypeExperienceRepository.GetByNameAsync(name, cancellationToken);
            if (existingTypeExperience == null || existingTypeExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            return _mapper.Map<TypeExperienceDataModel>(existingTypeExperience);
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

    public async Task<PagedResult<TypeExperienceDataModel>> GetPagedTypeExperiencesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeExperienceRepository.GetPageAsync(pageNumber, pageSize);

            var typeExperienceDataModels = _mapper.Map<List<TypeExperienceDataModel>>(pagedResult.Items);

            return new PagedResult<TypeExperienceDataModel>
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

    public async Task<PagedResult<TypeExperienceDataModel>> GetPagedTypeExperiencesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.TypeExperienceRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var typeExperienceDataModels = _mapper.Map<List<TypeExperienceDataModel>>(pagedResult.Items);

            return new PagedResult<TypeExperienceDataModel>
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

    public async Task UpdateTypeExperienceAsync(Guid id, TypeExperienceUpdateModel typeExperienceUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingTypeExperience = await _unitOfWork.TypeExperienceRepository.GetByIdAsync(id, cancellationToken);
            if (existingTypeExperience == null || existingTypeExperience.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("type activity");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(typeExperienceUpdateModel.TypeName).ToLower();

            var duplicateExists = _unitOfWork.TypeExperienceRepository
                .ActiveEntities
                .Where(x => x.Id != id && !x.IsDeleted)
                .AsEnumerable()
                .Any(x => Helper.RemoveVietnameseTone(x.TypeName).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequest("Tên danh mục đã tồn tại trong hệ thống.");
            }

            _mapper.Map(typeExperienceUpdateModel, existingTypeExperience);

            existingTypeExperience.LastUpdatedBy = currentUserId;
            existingTypeExperience.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TypeExperienceRepository.Update(existingTypeExperience);
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
}