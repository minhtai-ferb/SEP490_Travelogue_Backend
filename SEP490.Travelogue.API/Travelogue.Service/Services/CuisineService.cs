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

public interface ICuisineService
{
    Task<LocationDataDetailModel?> GetCuisineByLocationIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<LocationDataModel>> GetAllCuisinesAsync(CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetPagedCuisinesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task AddCuisineAsync(CuisineCreateModel cuisineCreateModel, CancellationToken cancellationToken);
    Task UpdateCuisineAsync(Guid id, CuisineUpdateModel cuisineUpdateModel, CancellationToken cancellationToken);
    Task DeleteCuisineAsync(Guid id, CancellationToken cancellationToken);
    // Task<PagedResult<CuisineDataModel>> GetPagedCuisinesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
    // Task<CuisineMediaResponse> AddCuisineWithMediaAsync(CuisineCreateWithMediaFileModel cuisineCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    // Task UpdateCuisineAsync(Guid id, CuisineUpdateWithMediaFileModel cuisineUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
}

public class CuisineService : ICuisineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;

    public CuisineService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task AddCuisineAsync(CuisineCreateModel cuisineCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newCuisine = _mapper.Map<Cuisine>(cuisineCreateModel);
            newCuisine.CreatedBy = currentUserId;
            newCuisine.LastUpdatedBy = currentUserId;
            newCuisine.CreatedTime = currentTime;
            newCuisine.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.CuisineRepository.AddAsync(newCuisine);
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

    public async Task DeleteCuisineAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);

            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            //var isInUsing = await _unitOfWork.LocationCraftVillageSuggestionRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // nếu đang được suggess thì xóa đi
            await _unitOfWork.LocationCuisineSuggestionRepository.ActiveEntities
                .Where(s => s.CuisineId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            await _unitOfWork.LocationMediaRepository.ActiveEntities
               .Where(s => s.LocationId == existingCuisine.LocationId)
               .ForEachAsync(s => s.IsDeleted = true, cancellationToken);

            //if (isInUsing)
            //{
            //    throw CustomExceptionFactory.CreateBadRequest(ResponseMessages.BE_USED);
            //}

            existingCuisine.LastUpdatedBy = currentUserId;
            existingCuisine.DeletedBy = currentUserId;
            existingCuisine.DeletedTime = currentTime;
            existingCuisine.LastUpdatedTime = currentTime;
            existingCuisine.IsDeleted = true;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CuisineRepository.Update(existingCuisine);
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

    public async Task<List<LocationDataModel>> GetAllCuisinesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingLocations = await _unitOfWork.LocationRepository.ActiveEntities
                .Include(l => l.LocationTypes)
                .Where(l => l.LocationTypes.Any(lt => lt.Type == LocationType.Cuisine))
                .ToListAsync(cancellationToken);

            if (existingLocations == null || !existingLocations.Any())
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");

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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LocationDataDetailModel?> GetCuisineByLocationIdAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task<PagedResult<CuisineDataModel>> GetPagedCuisinesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.CuisineRepository.GetPageAsync(pageNumber, pageSize);

            var cuisineDataModels = _mapper.Map<List<CuisineDataModel>>(pagedResult.Items);

            foreach (var cuisine in cuisineDataModels)
            {
                cuisine.Medias = await GetMediaByIdAsync(cuisine.Id, cancellationToken);
            }

            return new PagedResult<CuisineDataModel>
            {
                Items = cuisineDataModels,
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

    public async Task<PagedResult<LocationDataModel>> GetPagedCuisinesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken, LocationType.Cuisine);

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

    public async Task UpdateCuisineAsync(Guid id, CuisineUpdateModel cuisineUpdateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
            }

            _mapper.Map(cuisineUpdateModel, existingCuisine);

            existingCuisine.LastUpdatedBy = currentUserId;
            existingCuisine.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.BeginTransaction();
            _unitOfWork.CuisineRepository.Update(existingCuisine);
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

    // public async Task<CuisineMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken)
    // {
    //     _unitOfWork.BeginTransaction();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var existingCuisine = await _unitOfWork.CuisineRepository.GetWithIncludeAsync(id, c => c.Include(c => c.Location));
    //         if (existingCuisine == null || existingCuisine.IsDeleted)
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("nhà hàng");
    //         }

    //         if (imageUploads == null || !imageUploads.Any())
    //         {
    //             throw CustomExceptionFactory.CreateNotFoundError("medias");
    //         }

    //         var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
    //         var mediaResponses = new List<MediaResponse>();

    //         for (int i = 0; i < imageUploads.Count; i++)
    //         {
    //             var imageUpload = imageUploads[i];

    //             var newCuisineMedia = new CuisineMedia
    //             {
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 CuisineId = existingCuisine.Id,
    //                 MediaUrl = imageUrls[i],
    //                 SizeInBytes = imageUpload.Length,
    //                 CreatedBy = currentUserId,
    //                 CreatedTime = _timeService.SystemTimeNow,
    //                 LastUpdatedBy = currentUserId,
    //             };

    //             await _unitOfWork.CuisineMediaRepository.AddAsync(newCuisineMedia);

    //             mediaResponses.Add(new MediaResponse
    //             {
    //                 MediaUrl = imageUrls[i],
    //                 FileName = imageUpload.FileName,
    //                 FileType = imageUpload.ContentType,
    //                 SizeInBytes = imageUpload.Length
    //             });
    //         }

    //         await _unitOfWork.SaveAsync();
    //         _unitOfWork.CommitTransaction();

    //         return new CuisineMediaResponse
    //         {
    //             CuisineId = existingCuisine.Id,
    //             CuisineName = existingCuisine.Location.Name,
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

    // có add kèm theo ảnh

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid cuisineId, CancellationToken cancellationToken)
    {
        try
        {
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(cuisineId, cancellationToken);

            var cuisineMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(em => em.LocationId == existingCuisine!.LocationId)
                .ToListAsync(cancellationToken);

            return cuisineMedias.Select(x => new MediaResponse
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

    public async Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingCuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(id, cancellationToken);
            if (existingCuisine == null || existingCuisine.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("cuisine");
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
                var cuisineMedia = await _unitOfWork.LocationMediaRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(m => m.LocationId == existingCuisine.LocationId && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

                if (cuisineMedia != null)
                {
                    cuisineMedia.IsDeleted = true;
                    cuisineMedia.DeletedTime = DateTime.UtcNow;
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
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