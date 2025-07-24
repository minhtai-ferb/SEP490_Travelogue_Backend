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
    Task<PagedResult<NewsDataModel>> GetPagedNewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<NewsDataModel>> GetPagedNewsWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<NewsMediaResponse> AddNewsWithMediaAsync(NewsCreateWithMediaFileModel newsCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateNewsAsync(Guid id, NewsUpdateWithMediaFileModel newsUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
    //Task<CraftVillageMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<NewsMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<NewsMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
        string? thumbnailSelected,
        CancellationToken cancellationToken);

    Task<List<NewsDataModel>> GetNewsByCategoryAsync(NewsCategory? category, CancellationToken cancellationToken);
}

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IEnumService _enumService;

    public NewsService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
        _enumService = enumService;
    }

    public async Task<NewsDataModel> AddNewsAsync(NewsCreateModel newsCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newNews = _mapper.Map<News>(newsCreateModel);
            newNews.CreatedBy = currentUserId;
            newNews.LastUpdatedBy = currentUserId;
            newNews.CreatedTime = currentTime;
            newNews.LastUpdatedTime = currentTime;

            await _unitOfWork.NewsRepository.AddAsync(newNews);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.NewsRepository.ActiveEntities
                .FirstOrDefault(l => l.Id == newNews.Id);

            return _mapper.Map<NewsDataModel>(result);
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

    public async Task DeleteNewsAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);

            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            existingNews.LastUpdatedBy = currentUserId;
            existingNews.DeletedBy = currentUserId;
            existingNews.DeletedTime = currentTime;
            existingNews.LastUpdatedTime = currentTime;
            existingNews.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.NewsRepository.Update(existingNews);
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
                item.CategoryName = _enumService.GetEnumDisplayName(item.NewsCategory);
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
                item.CategoryName = _enumService.GetEnumDisplayName(item.NewsCategory);
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
    ? _enumService.GetEnumDisplayName(result.NewsCategory.Value)
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
                        ? _enumService.GetEnumDisplayName(x.NewsCategory.Value)
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

    public async Task<NewsDataModel?> GetNewsByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = await _unitOfWork.NewsRepository.GetByNameAsync(name, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            return _mapper.Map<NewsDataModel>(existingNews);
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

    public async Task<PagedResult<NewsDataModel>> GetPagedNewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.NewsRepository.GetPageAsync(pageNumber, pageSize);

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
                item.CategoryName = _enumService.GetEnumDisplayName(item.NewsCategory);
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
                item.CategoryName = _enumService.GetEnumDisplayName(item.NewsCategory);
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

    public async Task UpdateNewsAsync(Guid id, NewsUpdateModel newsUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            _mapper.Map(newsUpdateModel, existingNews);

            existingNews.LastUpdatedBy = currentUserId;
            existingNews.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.NewsRepository.Update(existingNews);
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

    // có add kèm theo ảnh
    public async Task<NewsMediaResponse> AddNewsWithMediaAsync(NewsCreateWithMediaFileModel newsCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newNews = _mapper.Map<News>(newsCreateModel);
            newNews.CreatedBy = currentUserId;
            newNews.LastUpdatedBy = currentUserId;
            newNews.CreatedTime = currentTime;
            newNews.LastUpdatedTime = currentTime;

            await _unitOfWork.NewsRepository.AddAsync(newNews);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (newsCreateModel.ImageUploads != null && newsCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(newsCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < newsCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = newsCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newNewsMedia = new NewsMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        NewsId = newNews.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newNewsMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.NewsMediaRepository.AddAsync(newNewsMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && newsCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstNewsMedia = await _unitOfWork.NewsMediaRepository
                        .GetFirstByNewsIdAsync(newNews.Id);
                    if (firstNewsMedia != null)
                    {
                        firstNewsMedia.IsThumbnail = true;
                        _unitOfWork.NewsMediaRepository.Update(firstNewsMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new NewsMediaResponse
            {
                NewsId = newNews.Id,
                Title = newNews.Title,
                Media = mediaResponses
            };
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

    // có update ảnh kèm theo
    public async Task UpdateNewsAsync(
        Guid id,
        NewsUpdateWithMediaFileModel newsUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            _mapper.Map(newsUpdateModel, existingNews);

            existingNews.LastUpdatedBy = currentUserId;
            existingNews.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.NewsRepository.Update(existingNews);

            // xu ly anh
            var imageUploads = newsUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.NewsMediaRepository.Entities
                .Where(dm => dm.NewsId == existingNews.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin news
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.NewsMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (imageUploads == null || imageUploads.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            // Có ảnh mới -> Upload lên Cloudinary
            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];
                bool isThumbnail = false;

                // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
                if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newNewsMedia = new NewsMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    NewsId = existingNews.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.NewsMediaRepository.AddAsync(newNewsMedia);
                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    IsThumbnail = isThumbnail,
                    SizeInBytes = imageUpload.Length
                });

                // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
                if (isThumbnail)
                {
                    foreach (var media in allMedia)
                    {
                        media.IsThumbnail = false;
                        _unitOfWork.NewsMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.NewsMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.NewsMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
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
    }

    public async Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            if (deletedImages == null || deletedImages.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrlsDeleted = await _cloudinaryService.DeleteImagesAsync(deletedImages);

            if (!imageUrlsDeleted)
            {
                return false;
            }

            foreach (var imageUpload in deletedImages)
            {
                var newsMedia = await _unitOfWork.NewsMediaRepository
                    .Entities
                    .FirstOrDefaultAsync(m => m.NewsId == id && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

                if (newsMedia != null)
                {
                    //_unitOfWork.NewsMediaRepository.Remove(newsMedia);
                    newsMedia.IsDeleted = true;
                    newsMedia.DeletedTime = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveAsync();
            _unitOfWork.CommitTransaction();

            return true;
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<NewsMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            if (imageUploads == null || imageUploads.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var allMedia = _unitOfWork.NewsMediaRepository.Entities
                .Where(dm => dm.NewsId == existingNews.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin news
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new NewsMediaResponse
                {
                    NewsId = existingNews.Id,
                    Title = existingNews.Title,
                    Media = new List<MediaResponse>()
                };
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.NewsMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (imageUploads == null || imageUploads.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new NewsMediaResponse
                {
                    NewsId = existingNews.Id,
                    Title = existingNews.Title,
                    Media = new List<MediaResponse>()
                };
            }

            // Có ảnh mới -> Upload lên Cloudinary
            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];
                bool isThumbnail = false;

                // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
                if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newNewsMedia = new NewsMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    NewsId = existingNews.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.NewsMediaRepository.AddAsync(newNewsMedia);
                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    IsThumbnail = isThumbnail,
                    SizeInBytes = imageUpload.Length
                });

                // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
                if (isThumbnail)
                {
                    foreach (var media in allMedia)
                    {
                        media.IsThumbnail = false;
                        _unitOfWork.NewsMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.NewsMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.NewsMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new NewsMediaResponse
            {
                NewsId = existingNews.Id,
                Title = existingNews.Title,
                Media = mediaResponses
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<NewsMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingNews = await _unitOfWork.NewsRepository.GetByIdAsync(id, cancellationToken);
            if (existingNews == null || existingNews.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("khách sạn");
            }

            if (imageUploads == null || !imageUploads.Any())
            {
                throw CustomExceptionFactory.CreateNotFoundError("medias");
            }

            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];

                var newNewsMedia = new NewsMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    NewsId = existingNews.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.NewsMediaRepository.AddAsync(newNewsMedia);

                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = imageUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    SizeInBytes = imageUpload.Length
                });
            }

            await _unitOfWork.SaveAsync();
            _unitOfWork.CommitTransaction();

            return new NewsMediaResponse
            {
                NewsId = existingNews.Id,
                Title = existingNews.Title,
                Media = mediaResponses
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    //public async Task<List<NewsDataModel>> GetAllNewsAdminAsync()
    //{
    //    try
    //    {
    //        var currentUserId = _userContextService.TryGetCurrentUserId();

    //        if (string.IsNullOrEmpty(currentUserId))
    //        {
    //            var allNews = await _unitOfWork.NewsRepository.GetAllAsync();
    //            var allNewsDataModels = _mapper.Map<List<NewsDataModel>>(allNews);
    //            await EnrichNewsDataModelsAsync(allNewsDataModels, new CancellationToken());
    //            return allNewsDataModels;
    //        }

    //        // 3. Nếu có userId → tìm user
    //        var userId = Guid.Parse(currentUserId);
    //        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, CancellationToken.None);

    //        // 4. Nếu không tìm được user (trường hợp hiếm) → fallback: trả toàn bộ
    //        if (user == null)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("user");
    //        }

    //        // 5. Lấy role
    //        var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(userId);
    //        var roleNames = roles.Select(r => r.Name).ToList();
    //        var roleIds = roles.Select(r => r.Id).ToList();

    //        List<News> newss;

    //        // 6. Admin toàn quyền
    //        if (roleNames.Equals(AppRole.ADMIN))
    //        {
    //            newss = (await _unitOfWork.NewsRepository.GetAllAsync()).ToList();
    //        }
    //        // 7. Admin huyện (dựa vào RoleDistrict)
    //        else
    //        {
    //            // Lấy các DistrictId mà user được phân quyền quản lý
    //            var allowedDistrictIds = await _unitOfWork.RoleDistrictRepository.ActiveEntities
    //                .Where(rd => roleIds.Contains(rd.RoleId))
    //                .Select(rd => rd.DistrictId)
    //                .Distinct()
    //                .ToListAsync();

    //            if (allowedDistrictIds.Any())
    //            {
    //                // Là admin huyện → chỉ được lấy các News trong danh sách huyện đó
    //                newss = await _unitOfWork.NewsRepository.ActiveEntities
    //                    .Where(l => l.DistrictId.HasValue && allowedDistrictIds.Contains(l.DistrictId.Value))
    //                    .ToListAsync();
    //            }
    //            else
    //            {
    //                // Không có quyền theo huyện nào → xem là người dùng thường
    //                newss = (await _unitOfWork.NewsRepository.GetAllAsync()).ToList();
    //            }
    //        }

    //        var newsDataModels = _mapper.Map<List<NewsDataModel>>(newss);
    //        await EnrichNewsDataModelsAsync(newsDataModels, new CancellationToken());
    //        return newsDataModels;
    //    }
    //    catch (CustomException)
    //    {
    //        throw;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //    }
    //}

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

    //private async Task EnrichNewsDataModelsAsync(List<NewsDataModel> newsDataModels, CancellationToken cancellationToken)
    //{
    //    var newsIds = newsDataModels.Select(x => x.Id).ToList();
    //    var districtIds = newsDataModels.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId.Value).Distinct().ToList();
    //    var typeNewsIds = newsDataModels.Where(x => x.TypeNewsId.HasValue).Select(x => x.TypeNewsId.Value).Distinct().ToList();

    //    var allMedias = await _unitOfWork.NewsMediaRepository
    //        .ActiveEntities
    //        .Where(m => newsIds.Contains(m.NewsId) && !m.FileType.Contains("video"))
    //        .ToListAsync(cancellationToken);

    //    var districtNames = await _unitOfWork.DistrictRepository
    //        .ActiveEntities
    //        .Where(d => districtIds.Contains(d.Id))
    //        .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

    //    var typeNewsNames = await _unitOfWork.TypeNewsRepository
    //        .ActiveEntities
    //        .Where(t => typeNewsIds.Contains(t.Id))
    //        .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

    //    foreach (var news in newsDataModels)
    //    {
    //        news.Medias = _mapper.Map<List<MediaResponse>>(allMedias
    //            .Where(m => m.NewsId == news.Id)
    //            .ToList());

    //        if (news.DistrictId.HasValue && districtNames.TryGetValue(news.DistrictId.Value, out var districtName))
    //        {
    //            news.DistrictName = districtName;
    //        }

    //        if (news.TypeNewsId.HasValue && typeNewsNames.TryGetValue(news.TypeNewsId.Value, out var typeNewsName))
    //        {
    //            news.TypeNewsName = typeNewsName;
    //        }
    //    }
    //}
}