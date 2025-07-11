using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CraftVillageModels;
using Travelogue.Service.BusinessModels.CuisineModels;
using Travelogue.Service.BusinessModels.HistoricalLocationModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ICraftVillageService
{
    Task<LocationDataDetailModel?> GetCraftVillageByLocationIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<LocationDataModel>> GetAllCraftVillagesAsync(CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetPagedCraftVillagesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task AddCraftVillageAsync(CraftVillageCreateModel craftVillageCreateModel, CancellationToken cancellationToken);
    Task AddCraftVillageAsync(Guid locationId, CraftVillageCreateModel craftVillageCreateModel, CancellationToken cancellationToken);
    Task UpdateCraftVillageAsync(Guid id, CraftVillageUpdateModel craftVillageUpdateModel, CancellationToken cancellationToken);
    Task DeleteCraftVillageAsync(Guid id, CancellationToken cancellationToken);
    // Task<CraftVillageMediaResponse> AddCraftVillageWithMediaAsync(CraftVillageCreateWithMediaFileModel craftVillageCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    // Task UpdateCraftVillageAsync(Guid id, CraftVillageUpdateWithMediaFileModel craftVillageUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<PagedResult<CraftVillageDataModel>> GetPagedCraftVillagesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task<CraftVillageMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    // Task<CraftVillageMediaResponse> UploadMediaAsync(
    //     Guid id,
    //     List<IFormFile>? imageUploads,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken);
}

public class CraftVillageService : ICraftVillageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public CraftVillageService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        this._cloudinaryService = cloudinaryService;
    }

    public async Task AddCraftVillageAsync(CraftVillageCreateModel craftVillageCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newCraftVillage = _mapper.Map<CraftVillage>(craftVillageCreateModel);
            newCraftVillage.CreatedBy = currentUserId;
            newCraftVillage.LastUpdatedBy = currentUserId;
            newCraftVillage.CreatedTime = currentTime;
            newCraftVillage.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.CraftVillageRepository.AddAsync(newCraftVillage);
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

    public async Task AddCraftVillageAsync(Guid locationId, CraftVillageCreateModel craftVillageCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            var newCraftVillage = _mapper.Map<CraftVillage>(craftVillageCreateModel);
            newCraftVillage.LocationId = locationId;
            newCraftVillage.CreatedBy = currentUserId;
            newCraftVillage.LastUpdatedBy = currentUserId;
            newCraftVillage.CreatedTime = currentTime;
            newCraftVillage.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.CraftVillageRepository.AddAsync(newCraftVillage);
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

    public async Task DeleteCraftVillageAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingCraftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(id, cancellationToken);

            if (existingCraftVillage == null || existingCraftVillage.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("craftVillage");
            }

            //var isInUsing = await _unitOfWork.LocationCraftVillageSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // nếu đang được suggest thì xóa đi
            await _unitOfWork.LocationCraftVillageSuggestionRepository.ActiveEntities
                .Where(s => s.CraftVillageId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            await _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(s => s.LocationId == existingCraftVillage.LocationId)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingCraftVillage.LastUpdatedBy = currentUserId;
            existingCraftVillage.DeletedBy = currentUserId;
            existingCraftVillage.DeletedTime = currentTime;
            existingCraftVillage.LastUpdatedTime = currentTime;
            existingCraftVillage.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CraftVillageRepository.Update(existingCraftVillage);
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

    public async Task<List<LocationDataModel>> GetAllCraftVillagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.LocationTypes)
                .Where(l => l.LocationTypes.Any(lt => lt.Type == LocationType.CraftVillage))
                .ToListAsync(cancellationToken);

            if (existingLocations == null || !existingLocations.Any())
                throw CustomExceptionFactory.CreateNotFoundError("craft village");

            // Ánh xạ dữ liệu sang LocationDataModel
            var locationDataModels = _mapper.Map<List<LocationDataModel>>(existingLocations);

            // Lấy thông tin bổ sung cho từng location
            foreach (var locationData in locationDataModels)
            {
                locationData.Medias = await GetMediaWithoutVideoByIdAsync(locationData.Id, cancellationToken);
                locationData.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationData.DistrictId ?? Guid.Empty);
                locationData.Categories = await _unitOfWork.LocationRepository.GetAllCategoriesAsync(locationData.Id);
                //locationData.HeritageRankName = _enumService.GetEnumDisplayName(locationData.HeritageRank);
            }

            return locationDataModels;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<LocationDataDetailModel?> GetCraftVillageByLocationIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetWithIncludeAsync(id, include => include
                .Include(l => l.LocationTypes)
            );

            if (existingLocation == null || existingLocation.IsDeleted)
                throw CustomExceptionFactory.CreateNotFoundError("location");

            var locationTypes = existingLocation.LocationTypes.Select(t => t.Type).ToHashSet();

            if (!locationTypes.Contains(LocationType.HistoricalSite))
                throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");

            var locationDataModel = _mapper.Map<LocationDataDetailModel>(existingLocation);

            if (locationTypes.Contains(LocationType.Cuisine))
            {
                var cuisine = await _unitOfWork.CuisineRepository.GetByLocationId(existingLocation.Id, cancellationToken);
                locationDataModel.Cuisine = cuisine != null ? _mapper.Map<CuisineDataModel>(cuisine) : null;
            }

            if (locationTypes.Contains(LocationType.CraftVillage))
            {
                var craftVillage = await _unitOfWork.CraftVillageRepository.GetByLocationId(existingLocation.Id, cancellationToken);
                locationDataModel.CraftVillage = craftVillage != null ? _mapper.Map<CraftVillageDataModel>(craftVillage) : null;
            }

            if (locationTypes.Contains(LocationType.HistoricalSite))
            {
                var historicalLocation = await _unitOfWork.HistoricalLocationRepository.GetByLocationId(existingLocation.Id, cancellationToken);
                locationDataModel.HistoricalLocation = historicalLocation != null ? _mapper.Map<HistoricalLocationDataModel>(historicalLocation) : null;
            }

            locationDataModel.Medias = await GetMediaWithoutVideoByIdAsync(id, cancellationToken);
            locationDataModel.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationDataModel.DistrictId);
            locationDataModel.Categories = await _unitOfWork.LocationRepository.GetAllCategoriesAsync(locationDataModel.Id, cancellationToken);

            return locationDataModel;
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

    public async Task<PagedResult<CraftVillageDataModel>> GetPagedCraftVillagesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.CraftVillageRepository.GetPageAsync(pageNumber, pageSize);

            var craftVillageDataModels = _mapper.Map<List<CraftVillageDataModel>>(pagedResult.Items);

            foreach (var craftVillage in craftVillageDataModels)
            {
                craftVillage.Medias = await GetMediaByIdAsync(craftVillage.CraftVillageId, cancellationToken);
            }

            return new PagedResult<CraftVillageDataModel>
            {
                Items = craftVillageDataModels,
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

    public async Task<PagedResult<LocationDataModel>> GetPagedCraftVillagesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken, LocationType.CraftVillage);

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(pagedResult.Items);

            foreach (var locationData in locationDataModels)
            {
                locationData.Medias = await GetMediaWithoutVideoByIdAsync(locationData.Id, cancellationToken);
                locationData.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationData.DistrictId ?? Guid.Empty);
                locationData.Categories = await _unitOfWork.LocationRepository.GetAllCategoriesAsync(locationData.Id);
            }

            return new PagedResult<LocationDataModel>
            {
                Items = locationDataModels,
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

    public async Task UpdateCraftVillageAsync(Guid id, CraftVillageUpdateModel craftVillageUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingCraftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(id, cancellationToken);
            if (existingCraftVillage == null || existingCraftVillage.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("craftVillage");
            }

            _mapper.Map(craftVillageUpdateModel, existingCraftVillage);

            existingCraftVillage.LastUpdatedBy = currentUserId;
            existingCraftVillage.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CraftVillageRepository.Update(existingCraftVillage);
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

    // public async Task<CraftVillageMediaResponse> UploadMediaAsync(
    //     Guid id,
    //     List<IFormFile>? imageUploads,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var existingCraftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(id, cancellationToken);
    //         if (existingCraftVillage == null || existingCraftVillage.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("craftVillage");
    //         }

    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("images");
    //         }

    //         var allMedia = _unitOfWork.CraftVillageMediaRepository.Entities
    //             .Where(dm => dm.CraftVillageId == existingCraftVillage.Id).ToList();

    //         // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin craftVillage
    //         if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new CraftVillageMediaResponse
    //             {
    //                 CraftVillageId = existingCraftVillage.Id,
    //                 CraftVillageName = existingCraftVillage.Location?.Name,
    //                 Media = new List<MediaResponse>()
    //             };
    //         }

    //         bool isThumbnailUpdated = false;

    //         // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
    //         if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
    //         {
    //             foreach (var media in allMedia)
    //             {
    //                 media.IsThumbnail = media.MediaUrl == thumbnailSelected;
    //                 _unitOfWork.CraftVillageMediaRepository.Update(media);
    //             }
    //             isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
    //         }

    //         // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new CraftVillageMediaResponse
    //             {
    //                 CraftVillageId = existingCraftVillage.Id,
    //                 CraftVillageName = existingCraftVillage.Location?.Name,
    //                 Media = new List<MediaResponse>()
    //             };
    //         }

    //         // Có ảnh mới -> Upload lên Cloudinary
    //         var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
    //         var mediaResponses = new List<MediaResponse>();

    //         for (int i = 0; i < imageUploads.Count; i++)
    //         {
    //             var imageUpload = imageUploads[i];
    //             bool isThumbnail = false;

    //             // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
    //             if (!string.IsNullOrEmpty(thumbnailSelected) && !Helper.IsValidUrl(thumbnailSelected))
    //             {
    //                 isThumbnail = imageUpload.FileName == thumbnailSelected;
    //             }

    //             var newCraftVillageMedia = new CraftVillageMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 CraftVillageId = existingCraftVillage.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 IsThumbnail = isThumbnail,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.CraftVillageMediaRepository.AddAsync(newCraftVillageMedia);
    //             mediaResponses.Add(new MediaResponse
    //             {
    //                 MediaUrl = imageUrls[i],
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 IsThumbnail = isThumbnail,
    //                 SizeInBytes = imageUpload.Length
    //             });

    //             // Nếu ảnh mới được chọn làm thumbnail -> Cập nhật tất cả ảnh cũ về IsThumbnail = false
    //             if (isThumbnail)
    //             {
    //                 foreach (var media in allMedia)
    //                 {
    //                     media.IsThumbnail = false;
    //                     _unitOfWork.CraftVillageMediaRepository.Update(media);
    //                 }
    //                 isThumbnailUpdated = true;
    //             }
    //         }

    //         // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
    //         if (!isThumbnailUpdated && mediaResponses.Count > 0)
    //         {
    //             var firstMedia = mediaResponses.First();
    //             var firstMediaEntity = await _unitOfWork.CraftVillageMediaRepository.ActiveEntities
    //                 .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

    //             if (firstMediaEntity != null)
    //             {
    //                 firstMediaEntity.IsThumbnail = true;
    //                 _unitOfWork.CraftVillageMediaRepository.Update(firstMediaEntity);
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return new CraftVillageMediaResponse
    //         {
    //             CraftVillageId = existingCraftVillage.Id,
    //             CraftVillageName = existingCraftVillage.Location?.Name,
    //             Media = mediaResponses
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         await _unitOfWork.RollBackAsync();
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await _unitOfWork.RollBackAsync();
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid craftVillageId, CancellationToken cancellationToken)
    {
        try
        {
            var existingCraftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(craftVillageId, cancellationToken);

            var craftVillageMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(em => em.LocationId == existingCraftVillage!.LocationId)
                .ToListAsync(cancellationToken);

            return craftVillageMedias.Select(x => new MediaResponse
            {
                MediaUrl = x.MediaUrl,
                FileName = x.FileName ?? string.Empty,
                IsThumbnail = x.IsThumbnail,
                FileType = x.FileType,
                SizeInBytes = x.SizeInBytes,
                CreatedTime = x.CreatedTime
            }).ToList();
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    private async Task<List<MediaResponse>> GetMediaWithoutVideoByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var locationMedias = await _unitOfWork.LocationMediaRepository
            .ActiveEntities
            .Where(em => em.LocationId == locationId)
            .Where(em => !EF.Functions.Like(em.FileType, "%video%"))
            .ToListAsync(cancellationToken);

        return locationMedias.Select(x => new MediaResponse
        {
            MediaUrl = x.MediaUrl,
            FileName = x.FileName ?? string.Empty,
            FileType = x.FileType,
            IsThumbnail = x.IsThumbnail,
            SizeInBytes = x.SizeInBytes,
            CreatedTime = x.CreatedTime
        }).ToList();
    }
}