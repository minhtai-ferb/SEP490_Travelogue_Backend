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
using Travelogue.Service.BusinessModels.HotelModels;
using Travelogue.Service.BusinessModels.LocationModels;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IHistoricalLocationService
{
    Task<LocationDataDetailModel?> GetHistoricalLocationByLocationIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<LocationDataModel>> GetAllHistoricalLocationsAsync(CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetPagedHistoricalLocationsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task AddHistoricalLocationAsync(HistoricalLocationCreateModel historicalLocationCreateModel, CancellationToken cancellationToken);
    // Task UpdateHistoricalLocationAsync(Guid id, HistoricalLocationUpdateModel historicalLocationUpdateModel, CancellationToken cancellationToken);
    // Task DeleteHistoricalLocationAsync(Guid id, CancellationToken cancellationToken);
}

public class HistoricalLocationService : IHistoricalLocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public HistoricalLocationService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<LocationDataModel>> GetAllHistoricalLocationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetAllAsync(cancellationToken);
            if (existingLocation == null || !existingLocation.Any())
            {
                return new List<LocationDataModel>();
            }

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(existingLocation);

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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LocationDataDetailModel?> GetHistoricalLocationByLocationIdAsync(Guid id, CancellationToken cancellationToken)
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

            // Chỉ lấy dữ liệu nếu Location có type tương ứng
            if (locationTypes.Contains(LocationType.Hotel))
            {
                var hotel = await _unitOfWork.HotelRepository.GetByLocationId(existingLocation.Id, cancellationToken);
                locationDataModel.Hotel = hotel != null ? _mapper.Map<HotelDataModel>(hotel) : null;
            }

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
    }

    public async Task<PagedResult<LocationDataModel>> GetPagedHistoricalLocationsWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

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

    public async Task UpdateHistoricalLocationAsync(Guid id, HistoricalLocationUpdateModel historicalLocationUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingHistoricalLocation == null || existingHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");
            }

            _mapper.Map(historicalLocationUpdateModel, existingHistoricalLocation);

            existingHistoricalLocation.LastUpdatedBy = currentUserId;
            existingHistoricalLocation.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.HistoricalLocationRepository.Update(existingHistoricalLocation);
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

    public async Task AddHistoricalLocationAsync(HistoricalLocationCreateModel historicalLocationCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var locationHistoricalLocation = _mapper.Map<Location>(historicalLocationCreateModel);
            locationHistoricalLocation.CreatedBy = currentUserId;
            locationHistoricalLocation.LastUpdatedBy = currentUserId;
            locationHistoricalLocation.CreatedTime = currentTime;
            locationHistoricalLocation.LastUpdatedTime = currentTime;
            await _unitOfWork.LocationRepository.AddAsync(locationHistoricalLocation);

            var newHistoricalLocation = _mapper.Map<HistoricalLocation>(historicalLocationCreateModel);
            newHistoricalLocation.CreatedBy = currentUserId;
            newHistoricalLocation.LastUpdatedBy = currentUserId;
            newHistoricalLocation.CreatedTime = currentTime;
            newHistoricalLocation.LastUpdatedTime = currentTime;
            newHistoricalLocation.LocationId = locationHistoricalLocation.Id;
            await _unitOfWork.HistoricalLocationRepository.AddAsync(newHistoricalLocation);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
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

    public async Task DeleteHistoricalLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByIdAsync(id, cancellationToken);

            if (existingHistoricalLocation == null || existingHistoricalLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");
            }

            //var isInUsing = await _unitOfWork.LocationHistoricalLocationSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            await _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(s => s.LocationId == existingHistoricalLocation.LocationId)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingHistoricalLocation.LastUpdatedBy = currentUserId;
            existingHistoricalLocation.DeletedBy = currentUserId;
            existingHistoricalLocation.DeletedTime = currentTime;
            existingHistoricalLocation.LastUpdatedTime = currentTime;
            existingHistoricalLocation.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.HistoricalLocationRepository.Update(existingHistoricalLocation);
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


    // public async Task<HistoricalLocationMediaResponse> UploadMediaAsync(
    //     Guid id,
    //     List<IFormFile>? imageUploads,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByIdAsync(id, cancellationToken);
    //         if (existingHistoricalLocation == null || existingHistoricalLocation.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");
    //         }

    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("images");
    //         }

    //         var allMedia = _unitOfWork.HistoricalLocationMediaRepository.Entities
    //             .Where(dm => dm.HistoricalLocationId == existingHistoricalLocation.Id).ToList();

    //         // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin historicalLocation
    //         if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new HistoricalLocationMediaResponse
    //             {
    //                 HistoricalLocationId = existingHistoricalLocation.Id,
    //                 // HistoricalLocationName = existingHistoricalLocation.Name,
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
    //                 _unitOfWork.HistoricalLocationMediaRepository.Update(media);
    //             }
    //             isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
    //         }

    //         // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return new HistoricalLocationMediaResponse
    //             {
    //                 HistoricalLocationId = existingHistoricalLocation.Id,
    //                 // HistoricalLocationName = existingHistoricalLocation.Name,
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

    //             var newHistoricalLocationMedia = new HistoricalLocationMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 HistoricalLocationId = existingHistoricalLocation.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 IsThumbnail = isThumbnail,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.HistoricalLocationMediaRepository.AddAsync(newHistoricalLocationMedia);
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
    //                     _unitOfWork.HistoricalLocationMediaRepository.Update(media);
    //                 }
    //                 isThumbnailUpdated = true;
    //             }
    //         }

    //         // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
    //         if (!isThumbnailUpdated && mediaResponses.Count > 0)
    //         {
    //             var firstMedia = mediaResponses.First();
    //             var firstMediaEntity = await _unitOfWork.HistoricalLocationMediaRepository.ActiveEntities
    //                 .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

    //             if (firstMediaEntity != null)
    //             {
    //                 firstMediaEntity.IsThumbnail = true;
    //                 _unitOfWork.HistoricalLocationMediaRepository.Update(firstMediaEntity);
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return new HistoricalLocationMediaResponse
    //         {
    //             HistoricalLocationId = existingHistoricalLocation.Id,
    //             // HistoricalLocationName = existingHistoricalLocation.Name,
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

    // có add ảnh kèm theo
    // public async Task<HistoricalLocationMediaResponse> AddHistoricalLocationWithMediaAsync(HistoricalLocationCreateWithMediaFileModel historicalLocationCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var currentTime = _timeService.SystemTimeNow;

    //         //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), historicalLocationCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
    //         //if (!checkRole)
    //         //{
    //         //    throw CustomExceptionFactory.CreateForbiddenError();
    //         //}

    //         var newHistoricalLocation = _mapper.Map<HistoricalLocation>(historicalLocationCreateModel);
    //         newHistoricalLocation.CreatedBy = currentUserId;
    //         newHistoricalLocation.LastUpdatedBy = currentUserId;
    //         newHistoricalLocation.CreatedTime = currentTime;
    //         newHistoricalLocation.LastUpdatedTime = currentTime;

    //         await _unitOfWork.HistoricalLocationRepository.AddAsync(newHistoricalLocation);
    //         await _unitOfWork.SaveAsync();

    //         var mediaResponses = new List<MediaResponse>();

    //         if (historicalLocationCreateModel.ImageUploads != null && historicalLocationCreateModel.ImageUploads.Count > 0)
    //         {
    //             var imageUrls = await _cloudinaryService.UploadImagesAsync(historicalLocationCreateModel.ImageUploads);

    //             bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
    //             bool thumbnailSet = false;

    //             for (int i = 0; i < historicalLocationCreateModel.ImageUploads.Count; i++)
    //             {
    //                 var imageFile = historicalLocationCreateModel.ImageUploads[i];
    //                 var imageUrl = imageUrls[i];

    //                 var newHistoricalLocationMedia = new HistoricalLocationMedia
    //                 {
    //                     FileName = imageFile.FileName,
    //                     FileType = imageFile.ContentType,
    //                     HistoricalLocationId = newHistoricalLocation.Id,
    //                     MediaUrl = imageUrl,
    //                     SizeInBytes = imageFile.Length,
    //                     CreatedBy = currentUserId,
    //                     CreatedTime = _timeService.SystemTimeNow,
    //                     LastUpdatedBy = currentUserId,
    //                 };

    //                 // Chọn ảnh làm thumbnail
    //                 if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
    //                 {
    //                     newHistoricalLocationMedia.IsThumbnail = true;
    //                     thumbnailSet = true;
    //                 }

    //                 await _unitOfWork.HistoricalLocationMediaRepository.AddAsync(newHistoricalLocationMedia);

    //                 mediaResponses.Add(new MediaResponse
    //                 {
    //                     MediaUrl = imageUrl,
    //                     FileName = imageFile.FileName,
    //                     FileType = imageFile.ContentType,
    //                     SizeInBytes = imageFile.Length
    //                 });
    //             }

    //             // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
    //             if (!thumbnailSet && historicalLocationCreateModel.ImageUploads.Count > 0)
    //             {
    //                 var firstMedia = mediaResponses.First();
    //                 var firstHistoricalLocationMedia = await _unitOfWork.HistoricalLocationMediaRepository
    //                     .GetFirstByHistoricalLocationIdAsync(newHistoricalLocation.Id);
    //                 if (firstHistoricalLocationMedia != null)
    //                 {
    //                     firstHistoricalLocationMedia.IsThumbnail = true;
    //                     _unitOfWork.HistoricalLocationMediaRepository.Update(firstHistoricalLocationMedia);
    //                 }
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return new HistoricalLocationMediaResponse
    //         {
    //             HistoricalLocationId = newHistoricalLocation.Id,
    //             // HistoricalLocationName = newHistoricalLocation.Name,
    //             Media = mediaResponses
    //         };
    //     }
    //     catch (CustomException)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    // có update ảnh kèm theo
    // public async Task UpdateHistoricalLocationAsync(
    //     Guid id,
    //     HistoricalLocationUpdateWithMediaFileModel historicalLocationUpdateModel,
    //     string? thumbnailSelected,
    //     CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();

    //         //var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), historicalLocationUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
    //         //if (!checkRole)
    //         //{
    //         //    throw CustomExceptionFactory.CreateForbiddenError();
    //         //}

    //         var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByIdAsync(id, cancellationToken);
    //         if (existingHistoricalLocation == null || existingHistoricalLocation.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");
    //         }

    //         _mapper.Map(historicalLocationUpdateModel, existingHistoricalLocation);

    //         existingHistoricalLocation.LastUpdatedBy = currentUserId;
    //         existingHistoricalLocation.LastUpdatedTime = _timeService.SystemTimeNow;

    //         _unitOfWork.HistoricalLocationRepository.Update(existingHistoricalLocation);

    //         // xu ly anh
    //         var imageUploads = historicalLocationUpdateModel.ImageUploads;
    //         var allMedia = _unitOfWork.HistoricalLocationMediaRepository.ActiveEntities
    //             .Where(dm => dm.HistoricalLocationId == existingHistoricalLocation.Id).ToList();

    //         // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin historicalLocation
    //         if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return;
    //         }

    //         bool isThumbnailUpdated = false;

    //         // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
    //         if (!string.IsNullOrEmpty(thumbnailSelected) && Helper.IsValidUrl(thumbnailSelected))
    //         {
    //             foreach (var media in allMedia)
    //             {
    //                 media.IsThumbnail = media.MediaUrl == thumbnailSelected;
    //                 _unitOfWork.HistoricalLocationMediaRepository.Update(media);
    //             }
    //             isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
    //         }

    //         // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
    //         if (imageUploads == null || imageUploads.Count == 0)
    //         {
    //             await _unitOfWork.SaveAsync();
    //             await transaction.CommitAsync(cancellationToken);
    //             return;
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

    //             var newHistoricalLocationMedia = new HistoricalLocationMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 HistoricalLocationId = existingHistoricalLocation.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 IsThumbnail = isThumbnail,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.HistoricalLocationMediaRepository.AddAsync(newHistoricalLocationMedia);
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
    //                     _unitOfWork.HistoricalLocationMediaRepository.Update(media);
    //                 }
    //                 isThumbnailUpdated = true;
    //             }
    //         }

    //         // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
    //         if (!isThumbnailUpdated && mediaResponses.Count > 0)
    //         {
    //             var firstMedia = mediaResponses.First();
    //             var firstMediaEntity = await _unitOfWork.HistoricalLocationMediaRepository.ActiveEntities
    //                 .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

    //             if (firstMediaEntity != null)
    //             {
    //                 firstMediaEntity.IsThumbnail = true;
    //                 _unitOfWork.HistoricalLocationMediaRepository.Update(firstMediaEntity);
    //             }
    //         }

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);
    //     }
    //     catch (CustomException)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync(cancellationToken);
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    // }

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid historicalLocationId, CancellationToken cancellationToken)
    {
        try
        {
            var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByIdAsync(historicalLocationId, cancellationToken);

            var historicalLocationMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(em => em.LocationId == existingHistoricalLocation!.LocationId)
                .ToListAsync(cancellationToken);

            return historicalLocationMedias.Select(x => new MediaResponse
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    //public async Task<HistoricalLocationDataModel?> GetHistoricalLocationByNameAsync(string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var existingHistoricalLocation = await _unitOfWork.HistoricalLocationRepository.GetByNameAsync(name, cancellationToken);
    //        if (existingHistoricalLocation == null || existingHistoricalLocation.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("historicalLocation");
    //        }

    //        return _mapper.Map<HistoricalLocationDataModel>(existingHistoricalLocation);
    //    }
    //    catch (CustomException)
    //    {
    //        throw;
    //    }
    //    catch (Exception)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError();
    //    }
    //    finally
    //    {
    //       //  _unitOfWork.Dispose();
    //    }
    //}
}