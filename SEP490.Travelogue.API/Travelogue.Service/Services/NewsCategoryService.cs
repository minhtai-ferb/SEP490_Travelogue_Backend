using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.NewsCategoryModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface INewsCategoryService
{
    Task<NewsCategoryDataModel?> GetNewsCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<NewsCategoryDataModel>> GetAllNewsCategoriesAsync(CancellationToken cancellationToken);
    Task AddNewsCategoryAsync(NewsCategoryCreateModel newsCreateModel, CancellationToken cancellationToken);
    Task UpdateNewsCategoryAsync(Guid id, NewsCategoryUpdateModel newsUpdateModel, CancellationToken cancellationToken);
    Task DeleteNewsCategoryAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<NewsCategoryDataModel>> GetPagedNewsCategoriesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<NewsCategoryDataModel>> GetPagedNewsCategoriesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class NewsCategoryService : INewsCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public NewsCategoryService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        this._cloudinaryService = cloudinaryService;
    }

    public async Task AddNewsCategoryAsync(NewsCategoryCreateModel newsCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var inputNameNormalized = Helper.RemoveVietnameseTone(newsCreateModel.Category).ToLower();

            var allCategories = await _unitOfWork.NewsCategoryRepository
                .Entities
                .ToListAsync(cancellationToken);

            var matchedCategory = allCategories.FirstOrDefault(x =>
                Helper.RemoveVietnameseTone(x.Category).ToLower() == inputNameNormalized);

            if (matchedCategory != null)
            {
                if (!matchedCategory.IsDeleted)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Danh mục đã tồn tại trong hệ thống.");
                }

                matchedCategory.IsDeleted = false;
                matchedCategory.Category = newsCreateModel.Category;
                matchedCategory.LastUpdatedBy = currentUserId;
                matchedCategory.LastUpdatedTime = currentTime;

                _unitOfWork.BeginTransaction();
                _unitOfWork.NewsCategoryRepository.Update(matchedCategory);
                _unitOfWork.CommitTransaction();
                return;
            }

            var newNewsCategory = _mapper.Map<NewsCategory>(newsCreateModel);
            newNewsCategory.CreatedBy = currentUserId;
            newNewsCategory.LastUpdatedBy = currentUserId;
            newNewsCategory.CreatedTime = currentTime;
            newNewsCategory.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.NewsCategoryRepository.AddAsync(newNewsCategory);
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

    public async Task DeleteNewsCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingNewsCategory = await _unitOfWork.NewsCategoryRepository.GetByIdAsync(id, cancellationToken);

            if (existingNewsCategory == null || existingNewsCategory.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news category");
            }

            var isInUsing = await _unitOfWork.NewsRepository.ActiveEntities.FirstOrDefaultAsync(e => e.NewsCategoryId == id, cancellationToken) != null;

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            existingNewsCategory.LastUpdatedBy = currentUserId;
            existingNewsCategory.DeletedBy = currentUserId;
            existingNewsCategory.DeletedTime = currentTime;
            existingNewsCategory.LastUpdatedTime = currentTime;
            existingNewsCategory.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.NewsCategoryRepository.Update(existingNewsCategory);
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

    public async Task<List<NewsCategoryDataModel>> GetAllNewsCategoriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingNewsCategory = await _unitOfWork.NewsCategoryRepository.GetAllAsync(cancellationToken);
            if (existingNewsCategory == null || existingNewsCategory.Count() == 0)
            {
                return new List<NewsCategoryDataModel>();
            }

            return _mapper.Map<List<NewsCategoryDataModel>>(existingNewsCategory);
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

    public async Task<NewsCategoryDataModel?> GetNewsCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingNewsCategory = await _unitOfWork.NewsCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingNewsCategory == null || existingNewsCategory.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news category");
            }

            return _mapper.Map<NewsCategoryDataModel>(existingNewsCategory);
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

    public async Task<NewsCategoryDataModel?> GetNewsCategoryByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingNewsCategory = await _unitOfWork.NewsCategoryRepository.GetByNameAsync(name, cancellationToken);
            if (existingNewsCategory == null || existingNewsCategory.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news category");
            }

            return _mapper.Map<NewsCategoryDataModel>(existingNewsCategory);
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

    public async Task<PagedResult<NewsCategoryDataModel>> GetPagedNewsCategoriesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsCategoryRepository.GetPageAsync(pageNumber, pageSize);

            var newsDataModels = _mapper.Map<List<NewsCategoryDataModel>>(pagedResult.Items);

            return new PagedResult<NewsCategoryDataModel>
            {
                Items = newsDataModels,
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

    public async Task<PagedResult<NewsCategoryDataModel>> GetPagedNewsCategoriesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsCategoryRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

            var newsDataModels = _mapper.Map<List<NewsCategoryDataModel>>(pagedResult.Items);

            return new PagedResult<NewsCategoryDataModel>
            {
                Items = newsDataModels,
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

    public async Task UpdateNewsCategoryAsync(Guid id, NewsCategoryUpdateModel newsUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingNewsCategory = await _unitOfWork.NewsCategoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingNewsCategory == null || existingNewsCategory.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news category");
            }

            var updatedNameNormalized = Helper.RemoveVietnameseTone(newsUpdateModel.Category).ToLower();

            var duplicateExists = _unitOfWork.NewsCategoryRepository
                 .ActiveEntities
                 .Where(x => x.Id != id && !x.IsDeleted)
                 .AsEnumerable()
                 .Any(x => Helper.RemoveVietnameseTone(x.Category).ToLower() == updatedNameNormalized);

            if (duplicateExists)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tên danh mục đã tồn tại trong hệ thống.");
            }

            // Cập nhật thông tin
            _mapper.Map(newsUpdateModel, existingNewsCategory);
            existingNewsCategory.LastUpdatedBy = currentUserId;
            existingNewsCategory.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.NewsCategoryRepository.Update(existingNewsCategory);
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