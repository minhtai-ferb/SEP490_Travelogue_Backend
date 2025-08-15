using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.NewsModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface INewsService
{
    Task<NewsDataDetailModel?> GetNewsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<NewsDataModel>> GetAllNewsAsync(CancellationToken cancellationToken);
    Task<NewsDataModel> AddNewsAsync(NewsCreateModel newsCreateModel, CancellationToken cancellationToken);
    Task UpdateNewsAsync(Guid id, NewsUpdateModel newsUpdateModel, CancellationToken cancellationToken);
    Task DeleteNewsAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<NewsDataModel>> GetPagedNewsWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<NewsDataModel>> GetNewsByCategoryAsync(NewsCategory? category, CancellationToken cancellationToken);
    Task<PagedResult<NewsDataModel>> GetPagedEventWithFilterAsync(string? title, Guid? locationId, Boolean? isHighlighted, int? month, int? year, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<NewsDataModel>> GetPagedNewsWithFilterAsync(string? title, Guid? locationId, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<NewsDataModel>> GetPagedExperienceWithFilterAsync(string? title, Guid? locationId, TypeExperience? typeExperience, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IEnumService _enumService;
    private readonly IMediaService _mediaService;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService, IEnumService enumService, IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
        _enumService = enumService;
        _mediaService = mediaService;
    }

    public async Task<NewsDataModel> AddNewsAsync(NewsCreateModel newsCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var isExperience = newsCreateModel.NewsCategory == NewsCategory.Experience;
            if (newsCreateModel.TypeExperience.HasValue && !isExperience)
                throw CustomExceptionFactory.CreateBadRequestError("TypeExperience chỉ được set khi NewsCategory = Experience.");
            if (isExperience && !newsCreateModel.TypeExperience.HasValue)
                throw CustomExceptionFactory.CreateBadRequestError("Experience bắt buộc phải có TypeExperience.");
            if (newsCreateModel.NewsCategory == 0)
            {
                throw CustomExceptionFactory.CreateBadRequestError(
                    "Trường 'Loại tin tức' là bắt buộc."
                );
            }

            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;
            Location? location = null;

            if (newsCreateModel.LocationId.HasValue)
            {
                location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .FirstOrDefaultAsync(l => l.Id == newsCreateModel.LocationId.Value, cancellationToken);

                if (location == null)
                {
                    throw CustomExceptionFactory.CreateNotFoundError("location");
                }
            }

            if (newsCreateModel.TypeExperience.HasValue && newsCreateModel.NewsCategory != NewsCategory.Experience)
            {
                throw CustomExceptionFactory.CreateBadRequestError("TypeExperience can only be set for Experience news category.");
            }

            var newNews = _mapper.Map<News>(newsCreateModel);
            newNews.CreatedBy = currentUserId;
            newNews.LastUpdatedBy = currentUserId;
            newNews.CreatedTime = currentTime;
            newNews.LastUpdatedTime = currentTime;

            await _unitOfWork.NewsRepository.AddAsync(newNews);
            await _unitOfWork.SaveAsync();

            List<NewsMedia> newsMedias = new List<NewsMedia>();
            if (newsCreateModel.MediaDtos != null && newsCreateModel.MediaDtos.Count > 0)
            {
                foreach (var mediaDto in newsCreateModel.MediaDtos)
                {
                    var newNewsMedia = new NewsMedia
                    {
                        Id = Guid.NewGuid(),
                        NewsId = newNews.Id,
                        MediaUrl = mediaDto.MediaUrl,
                        IsThumbnail = mediaDto.IsThumbnail,
                        CreatedBy = currentUserId,
                        CreatedTime = currentTime,
                        LastUpdatedTime = currentTime,
                        LastUpdatedBy = currentUserId
                    };

                    newsMedias.Add(newNewsMedia);
                }
                await _unitOfWork.NewsMediaRepository.AddRangeAsync(newsMedias);
                await _unitOfWork.SaveAsync();
            }

            await transaction.CommitAsync(cancellationToken);


            var response = new NewsDataModel
            {
                Id = newNews.Id,
                Title = newNews.Title,
                Description = newNews.Description,
                Content = newNews.Content,
                LocationId = location?.Id,
                LocationName = location?.Name,
                NewsCategory = newNews.NewsCategory.Value,
                CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(newNews.NewsCategory.Value),
                StartDate = newNews.StartDate,
                EndDate = newNews.EndDate,
                IsHighlighted = newNews.IsHighlighted,
                TypeExperience = newNews.TypeExperience,
                TypeExperienceText = newNews.TypeExperience.HasValue
                    ? _enumService.GetEnumDisplayName<TypeExperience>(newNews.TypeExperience.Value)
                    : string.Empty,
                CreatedTime = newNews.CreatedTime,
                LastUpdatedTime = newNews.LastUpdatedTime,
                Medias = newsMedias.Select(x => new MediaResponse
                {
                    MediaUrl = x.MediaUrl,
                    FileName = x.FileName ?? string.Empty,
                    IsThumbnail = x.IsThumbnail,
                    CreatedTime = x.CreatedTime
                }).ToList()
            };

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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task DeleteNewsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            // 1) Lấy bản ghi
            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            // 2) Soft delete News
            existingNews.IsDeleted = true;
            existingNews.DeletedBy = currentUserId;
            existingNews.DeletedTime = currentTime;
            existingNews.LastUpdatedBy = currentUserId;
            existingNews.LastUpdatedTime = currentTime;

            _unitOfWork.NewsRepository.Update(existingNews);
            await _unitOfWork.SaveAsync();

            // 3) Soft delete Media liên quan (nếu có)
            var relatedMedias = await _unitOfWork.NewsMediaRepository
                .ActiveEntities
                .Where(m => m.NewsId == existingNews.Id)
                .ToListAsync(cancellationToken);

            if (relatedMedias.Count > 0)
            {
                foreach (var m in relatedMedias)
                {
                    m.IsDeleted = true;
                    m.LastUpdatedBy = currentUserId;
                    m.LastUpdatedTime = currentTime;
                }

                _unitOfWork.NewsMediaRepository.UpdateRange(relatedMedias);
                await _unitOfWork.SaveAsync();
            }

            // 4) Commit
            await transaction.CommitAsync(cancellationToken);
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
        finally
        {
            //_unitOfWork.Dispose();
        }
    }

    public async Task<List<NewsDataModel>> GetAllNewsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = await _unitOfWork.NewsRepository.GetAllAsync(cancellationToken);
            if (existingNews == null || existingNews.Count() == 0)
            {
                return new List<NewsDataModel>();
            }

            var result = _mapper.Map<List<NewsDataModel>>(existingNews);

            foreach (var item in result)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);
                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            return result;
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

    public async Task<List<NewsDataModel>> GetNewsByCategoryAsync(NewsCategory? category, CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = await _unitOfWork.NewsRepository
                .GetAllAsync(cancellationToken);

            if (existingNews == null || !existingNews.Any())
            {
                return new List<NewsDataModel>();
            }

            var filteredNews = category.HasValue
                ? existingNews.Where(x => x.NewsCategory == category.Value).ToList()
                : existingNews.ToList();

            var result = _mapper.Map<List<NewsDataModel>>(filteredNews);

            foreach (var item in result)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);
                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            return result;
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

    public async Task<PagedResult<NewsDataModel>> GetPagedEventWithFilterAsync(string? title, Guid? locationId, Boolean? isHighlighted, int? month, int? year, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsRepository.GetPageWithSearchAsync(
                title, locationId, isHighlighted, pageNumber, pageSize, cancellationToken);

            var eventItems = pagedResult.Items = pagedResult.Items
            .Where(a => a.NewsCategory == NewsCategory.Event)
            .ToList();

            var newsDataModels = _mapper.Map<List<NewsDataModel>>(pagedResult.Items);

            foreach (var item in newsDataModels)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);

                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            if (year.HasValue && month.HasValue &&
                year.Value > 0 && month.Value > 0)
            {
                newsDataModels = newsDataModels.Where(a =>
                    a.StartDate.HasValue &&
                    a.EndDate.HasValue &&
                    a.StartDate.Value.Year == year.Value &&
                    (a.StartDate.Value.Month == month.Value ||
                     a.EndDate.Value.Month == month.Value)
                ).ToList();
            }

            var result = new PagedResult<NewsDataModel>
            {
                Items = newsDataModels,
                TotalCount = eventItems.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
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

    public async Task<PagedResult<NewsDataModel>> GetPagedNewsWithFilterAsync(string? title, Guid? locationId, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsRepository.GetPageWithSearchAsync(
                title, locationId, isHighlighted, pageNumber, pageSize, cancellationToken);

            var newItems = pagedResult.Items = pagedResult.Items.Where(a =>
                a.NewsCategory == NewsCategory.News
            ).ToList();

            var newsDataModels = _mapper.Map<List<NewsDataModel>>(pagedResult.Items);

            foreach (var item in newsDataModels)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);

                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            var result = new PagedResult<NewsDataModel>
            {
                Items = newsDataModels,
                TotalCount = newItems.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
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

    public async Task<PagedResult<NewsDataModel>> GetPagedExperienceWithFilterAsync(string? title, Guid? locationId, TypeExperience? typeExperience, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsRepository.GetPageWithSearchAsync(
                title, locationId, isHighlighted, typeExperience, pageNumber, pageSize, cancellationToken);

            var experienceItems = pagedResult.Items = pagedResult.Items.Where(a =>
                a.NewsCategory == NewsCategory.Experience
            ).ToList();

            if (locationId.HasValue)
            {
                pagedResult.Items = pagedResult.Items.Where(a => a.LocationId == locationId.Value).ToList();
            }

            var newsDataModels = _mapper.Map<List<NewsDataModel>>(pagedResult.Items);

            foreach (var item in newsDataModels)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);

                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            var result = new PagedResult<NewsDataModel>
            {
                Items = newsDataModels,
                TotalCount = experienceItems.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
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

    public async Task<NewsDataDetailModel?> GetNewsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = await _unitOfWork.NewsRepository
                .ActiveEntities
                .Where(x => x.Id == id)
                .Include(x => x.Location)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            var result = _mapper.Map<NewsDataDetailModel>(existingNews);

            result.LocationName = existingNews.Location?.Name;

            result.CategoryName = result.NewsCategory.HasValue
                ? _enumService.GetEnumDisplayName<NewsCategory>(result.NewsCategory.Value)
                : null;

            result.Medias = await GetMediaByIdAsync(result.Id, cancellationToken);

            result.LocationName = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Where(x => x.Id == result.LocationId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken);

            var relatedNewsQuery = _unitOfWork.NewsRepository
                .ActiveEntities
                .Where(x => !x.IsDeleted && x.Id != id);

            if (existingNews.LocationId.HasValue)
            {
                relatedNewsQuery = relatedNewsQuery.Where(x => x.LocationId == existingNews.LocationId);
            }

            var relatedNews = await relatedNewsQuery
                .OrderByDescending(x => x.CreatedTime)
                .Take(5)
                .ToListAsync(cancellationToken);

            var relatedNewsModels = new List<RelatedNewsModel>();
            foreach (var x in relatedNews)
            {
                var media = await GetMediaByIdAsync(x.Id, cancellationToken);

                relatedNewsModels.Add(new RelatedNewsModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    LocationName = x.Location?.Name,
                    CategoryName = x.NewsCategory.HasValue
                        ? _enumService.GetEnumDisplayName<NewsCategory>(x.NewsCategory.Value)
                        : null,
                    Medias = media
                });
            }

            result.RelatedNews = relatedNewsModels.ToList();

            return result;
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

    public async Task UpdateNewsAsync(Guid id, NewsUpdateModel newsUpdateModel, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            Location? location = null;
            if (newsUpdateModel.LocationId.HasValue)
            {
                location = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(l => l.Id == newsUpdateModel.LocationId.Value, cancellationToken);

                if (location == null)
                {
                    throw CustomExceptionFactory.CreateNotFoundError("location");
                }
            }

            var isExperience = newsUpdateModel.NewsCategory == NewsCategory.Experience;
            if (newsUpdateModel.TypeExperience.HasValue && !isExperience)
                throw CustomExceptionFactory.CreateBadRequestError("TypeExperience chỉ được set khi NewsCategory = Experience.");
            if (isExperience && !newsUpdateModel.TypeExperience.HasValue)
                throw CustomExceptionFactory.CreateBadRequestError("Experience bắt buộc phải có TypeExperience.");
            if (newsUpdateModel.NewsCategory != existingNews.NewsCategory)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật NewsCategory. Vui lòng tạo mới nếu cần thay đổi loại tin tức.");
            }
            if (newsUpdateModel.NewsCategory == 0)
            {
                throw CustomExceptionFactory.CreateBadRequestError(
                    "Trường 'Loại tin tức' là bắt buộc."
                );
            }

            var keepCreatedBy = existingNews.CreatedBy;
            var keepCreatedTime = existingNews.CreatedTime;

            _mapper.Map(newsUpdateModel, existingNews);

            existingNews.CreatedBy = keepCreatedBy;
            existingNews.CreatedTime = keepCreatedTime;
            existingNews.LastUpdatedBy = currentUserId;
            existingNews.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.NewsRepository.Update(existingNews);
            await _unitOfWork.SaveAsync();

            if (newsUpdateModel.MediaDtos != null)
            {
                var existingMedias = await _unitOfWork.NewsMediaRepository
                    .ActiveEntities
                    .Where(m => m.NewsId == existingNews.Id)
                    .ToListAsync(cancellationToken);

                // Danh sách url mới từ client
                var incomingUrls = newsUpdateModel.MediaDtos.Select(m => m.MediaUrl).ToHashSet();

                // (1) Xóa mềm những media không còn trong incomingUrls
                var toDelete = existingMedias
                    .Where(em => !incomingUrls.Contains(em.MediaUrl))
                    .ToList();

                foreach (var m in toDelete)
                {
                    m.IsDeleted = true;
                    m.LastUpdatedBy = currentUserId;
                    m.LastUpdatedTime = _timeService.SystemTimeNow;
                }
                if (toDelete.Count > 0)
                {
                    _unitOfWork.NewsMediaRepository.UpdateRange(toDelete);
                    await _unitOfWork.SaveAsync();
                }

                // (2) Cập nhật thumbnail cho media trùng url nhưng thay đổi IsThumbnail
                var incomingByUrl = newsUpdateModel.MediaDtos.ToDictionary(m => m.MediaUrl, m => m);
                var toUpdate = existingMedias
                    .Where(em => incomingByUrl.ContainsKey(em.MediaUrl) &&
                                 em.IsThumbnail != incomingByUrl[em.MediaUrl].IsThumbnail)
                    .ToList();

                foreach (var m in toUpdate)
                {
                    m.IsThumbnail = incomingByUrl[m.MediaUrl].IsThumbnail;
                    m.LastUpdatedBy = currentUserId;
                    m.LastUpdatedTime = _timeService.SystemTimeNow;
                }
                if (toUpdate.Count > 0)
                {
                    _unitOfWork.NewsMediaRepository.UpdateRange(toUpdate);
                    await _unitOfWork.SaveAsync();
                }

                // (3) Thêm mới những media có url chưa tồn tại
                var existingUrls = existingMedias.Select(em => em.MediaUrl).ToHashSet();
                var toAdd = newsUpdateModel.MediaDtos
                    .Where(m => !existingUrls.Contains(m.MediaUrl))
                    .Select(m => new NewsMedia
                    {
                        Id = Guid.NewGuid(),
                        NewsId = existingNews.Id,
                        MediaUrl = m.MediaUrl,
                        IsThumbnail = m.IsThumbnail,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                        LastUpdatedTime = _timeService.SystemTimeNow
                    })
                    .ToList();

                if (toAdd.Count > 0)
                {
                    await _unitOfWork.NewsMediaRepository.AddRangeAsync(toAdd);
                    await _unitOfWork.SaveAsync();
                }
            }
            await transaction.CommitAsync(cancellationToken);
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
        finally
        {
            // _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<NewsDataModel>> GetPagedNewsWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsRepository.GetPageWithSearchAsync(title, pageNumber, pageSize, cancellationToken);

            var newsDataModels = _mapper.Map<List<NewsDataModel>>(pagedResult.Items);

            var result = new PagedResult<NewsDataModel>
            {
                Items = newsDataModels,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            foreach (var item in result.Items)
            {
                item.Medias = await GetMediaByIdAsync(item.Id, cancellationToken);
                item.LocationName = await _unitOfWork.LocationRepository
                    .ActiveEntities
                    .Where(x => x.Id == item.LocationId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                item.CategoryName = _enumService.GetEnumDisplayName<NewsCategory>(item.NewsCategory);
                item.TypeExperienceText = _enumService.GetEnumDisplayName<TypeExperience>(item.TypeExperience);
            }

            return result;
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

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid newsId, CancellationToken cancellationToken)
    {
        var newsMedias = await _unitOfWork.NewsMediaRepository
            .ActiveEntities
            .Where(em => em.NewsId == newsId)
            .ToListAsync(cancellationToken);

        return newsMedias.Select(x => new MediaResponse
        {
            MediaUrl = x.MediaUrl,
            FileName = x.FileName ?? string.Empty,
            IsThumbnail = x.IsThumbnail,
            FileType = x.FileType,
            SizeInBytes = x.SizeInBytes,
            CreatedTime = x.CreatedTime
        }).ToList();
    }

}