using AutoMapper;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
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

public interface ILocationService
{
    Task<LocationDataDetailModel?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<LocationDataModel>> GetAllLocationsAsync(CancellationToken cancellationToken);
    Task<LocationDataModel> AddLocationAsync(LocationCreateModel locationCreateModel, CancellationToken cancellationToken);
    Task UpdateLocationAsync(Guid id, LocationUpdateModel locationUpdateModel, CancellationToken cancellationToken);
    Task DeleteLocationAsync(Guid id, CancellationToken cancellationToken);
    Task<List<LocationDataModel>> GetNearestCuisineLocationsAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<List<LocationDataModel>> GetNearestHistoricalLocationsAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<List<LocationDataModel>> GetNearestCraftVillageLocationsAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<PagedResult<LocationDataModel>> GetPagedLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task UploadImageLocationAsync(Guid id, IFormFile image, CancellationToken cancellationToken);
    //Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(string? title, LocationType? type, Guid? districtId, HeritageRank? heritageRank, int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string title, Guid typeId, CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetFavoriteLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task AddFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken);
    Task RemoveFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadMediaAsync(Guid id, UploadMediasDto uploadMediasDto, string? thumbnailFileName, CancellationToken cancellationToken);
    Task<LocationMediaResponse> AddLocationWithMediaAsync(LocationCreateWithMediaFileModel locationCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateLocationAsync(Guid id, LocationUpdateWithMediaFileModel locationUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadVideoAsync(
        Guid id,
        List<IFormFile> videoUploads,
        CancellationToken cancellationToken);

    Task<LocationDataDetailModel?> GetLocationByIdWithVideosAsync(Guid id, CancellationToken cancellationToken);

    Task<LocationDataModel> CreateBasicLocationAsync(LocationCreateModel model, CancellationToken cancellationToken);
    Task<LocationDataModel> AddCuisineDataAsync(Guid locationId, CuisineCreateModel? model, CancellationToken cancellationToken);
    Task<LocationDataModel> AddCraftVillageDataAsync(Guid locationId, CraftVillageCreateModel? model, CancellationToken cancellationToken);
    Task<LocationDataModel> AddHistoricalLocationDataAsync(Guid locationId, HistoricalLocationCreateModel? model, CancellationToken cancellationToken);
    Task<LocationDataModel> UpdateCuisineDataAsync(
        Guid locationId,
        CuisineUpdateDto dto,
        CancellationToken cancellationToken = default);
    Task<LocationDataModel> UpdateCraftVillageDataAsync(Guid locationId, CraftVillageUpdateDto dto, CancellationToken cancellationToken = default);
    Task<LocationDataModel> UpdateHistoricalLocationDataAsync(Guid locationId, HistoricalLocationUpdateDto dto, CancellationToken cancellationToken = default);
    Task<LocationDataModel> UpdateScenicSpotDataAsync(Guid locationId, LocationUpdateDto dto, CancellationToken cancellationToken = default);
}

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IEnumService _enumService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediaService _mediaService;

    public LocationService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, ICloudinaryService cloudinaryService, IEnumService enumService, IHttpContextAccessor httpContextAccessor, IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _cloudinaryService = cloudinaryService;
        _enumService = enumService;
        _httpContextAccessor = httpContextAccessor;
        _mediaService = mediaService;
    }

    public async Task<LocationDataModel> AddLocationAsync(LocationCreateModel locationCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newLocation = _mapper.Map<Location>(locationCreateModel);

            await _unitOfWork.LocationRepository.AddAsync(newLocation);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.LocationRepository.ActiveEntities
                .FirstOrDefault(l => l.Id == newLocation.Id);

            return _mapper.Map<LocationDataModel>(result);
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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LocationDataModel> CreateBasicLocationAsync(LocationCreateModel model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentUserIdGuid = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var newLocation = _mapper.Map<Location>(model);
            newLocation.CreatedBy = currentUserId;
            newLocation.LastUpdatedBy = currentUserId;
            newLocation.CreatedTime = currentTime;
            newLocation.LastUpdatedTime = currentTime;

            await _unitOfWork.LocationRepository.AddAsync(newLocation);
            await _unitOfWork.SaveAsync();

            List<LocationMedia> locationMedias = new();
            if (model.MediaDtos.Any())
            {
                locationMedias = model.MediaDtos.Select(media => new LocationMedia
                {
                    Id = Guid.NewGuid(),
                    LocationId = newLocation.Id,
                    MediaUrl = media.MediaUrl,
                    IsThumbnail = media.IsThumbnail,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                }).ToList();

                await _unitOfWork.LocationMediaRepository.AddRangeAsync(locationMedias);
                await _unitOfWork.SaveAsync();
            }

            await transaction.CommitAsync(cancellationToken);

            var district = newLocation.DistrictId.HasValue
                ? await _unitOfWork.DistrictRepository.GetByIdAsync(newLocation.DistrictId.Value, cancellationToken)
                : null;

            var createdByName = await _unitOfWork.UserRepository
                .ActiveEntities
                .Where(u => u.Id == currentUserIdGuid)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            var lastUpdatedByNameName = await _unitOfWork.UserRepository
                .ActiveEntities
                .Where(u => u.Id == currentUserIdGuid)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            var response = new LocationDataModel
            {
                Id = newLocation.Id,
                Name = newLocation.Name,
                Description = newLocation.Description,
                Content = newLocation.Content,
                Address = newLocation.Address,
                Latitude = newLocation.Latitude,
                Longitude = newLocation.Longitude,
                OpenTime = newLocation.OpenTime,
                CloseTime = newLocation.CloseTime,
                Category = "",
                DistrictId = newLocation.DistrictId,
                DistrictName = district?.Name,
                CreatedTime = newLocation.CreatedTime,
                LastUpdatedTime = newLocation.LastUpdatedTime,
                CreatedBy = newLocation.CreatedBy,
                LastUpdatedBy = newLocation.LastUpdatedBy,
                CreatedByName = createdByName,
                LastUpdatedByName = lastUpdatedByNameName,
                Medias = locationMedias.Select(m => new MediaResponse
                {
                    MediaUrl = m.MediaUrl,
                    IsThumbnail = m.IsThumbnail
                }).ToList()
            };

            return response;
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


    public async Task<LocationDataModel> AddCuisineDataAsync(Guid locationId, CuisineCreateModel? model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (model == null)
            {
                throw new InvalidOperationException("Cuisine data is missing for type 'Cuisine'.");
            }

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            var isCuisine = location.LocationType == LocationType.Cuisine;
            if (!isCuisine)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location is not of type Cuisine.");
            }

            // Add Cuisine data
            var cuisine = new Cuisine
            {
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                CuisineType = model.CuisineType,
                LocationId = location.Id
            };
            await _unitOfWork.CuisineRepository.AddAsync(cuisine);

            await _unitOfWork.SaveAsync();

            var response = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            await transaction.CommitAsync(cancellationToken);
            return response;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<LocationDataModel> UpdateCuisineDataAsync(
        Guid locationId,
        CuisineUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw CustomExceptionFactory.CreateBadRequestError("Cuisine data cannot be null.");

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.Cuisine)
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
                throw CustomExceptionFactory.CreateBadRequestError("Location not found.");

            if (location.LocationType != LocationType.Cuisine)
                throw CustomExceptionFactory.CreateBadRequestError("The specified location is not a Cuisine.");

            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Content = dto.Content;
            location.Address = dto.Address;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.OpenTime = dto.OpenTime;
            location.CloseTime = dto.CloseTime;
            location.DistrictId = dto.DistrictId;
            location.LocationType = LocationType.Cuisine;
            location.LastUpdatedTime = DateTime.UtcNow;

            if (location.Cuisine == null)
            {
                location.Cuisine = new Cuisine
                {
                    LocationId = locationId,
                    CuisineType = dto.CuisineType,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    Website = dto.Website,
                    SignatureProduct = dto.SignatureProduct,
                    CookingMethod = dto.CookingMethod,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow
                };
                await _unitOfWork.CuisineRepository.AddAsync(location.Cuisine);
            }
            else
            {
                var cuisine = location.Cuisine;
                cuisine.CuisineType = dto.CuisineType;
                cuisine.PhoneNumber = dto.PhoneNumber;
                cuisine.Email = dto.Email;
                cuisine.Website = dto.Website;
                cuisine.SignatureProduct = dto.SignatureProduct;
                cuisine.CookingMethod = dto.CookingMethod;
                cuisine.LastUpdatedTime = DateTime.UtcNow;
            }

            await UpdateLocationMediasAsync(locationId, dto.MediaDtos, cancellationToken);

            await _unitOfWork.SaveAsync();

            await transaction.CommitAsync(cancellationToken);

            var model = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            return model;
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

    public async Task<LocationDataModel> AddCraftVillageDataAsync(Guid locationId, CraftVillageCreateModel? model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (model == null)
            {
                throw new InvalidOperationException("CraftVillage data is missing for type CraftVillage.");
            }

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            var isCraftVillage = location.LocationType == LocationType.CraftVillage;
            if (!isCraftVillage)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location is not of type Craft Village.");
            }

            var craftVillage = new CraftVillage
            {
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                WorkshopsAvailable = model.WorkshopsAvailable,
                LocationId = location.Id
            };

            await _unitOfWork.CraftVillageRepository.AddAsync(craftVillage);

            await _unitOfWork.SaveAsync();

            var response = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            await transaction.CommitAsync(cancellationToken);
            return response;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<LocationDataModel> UpdateCraftVillageDataAsync(Guid locationId, CraftVillageUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Craft village data cannot be null.");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentUserIdGuid = Guid.Parse(_userContextService.GetCurrentUserId());
            // Find the Location with its associated CraftVillage
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.CraftVillage)
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location not found.");
            }

            // Verify that the location is a CraftVillage
            if (location.LocationType != LocationType.CraftVillage)
            {
                throw CustomExceptionFactory.CreateBadRequestError("The specified location is not a Craft Village.");
            }

            // Update Location properties
            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Content = dto.Content;
            location.Address = dto.Address;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.OpenTime = dto.OpenTime;
            location.CloseTime = dto.CloseTime;
            location.DistrictId = dto.DistrictId;
            location.LocationType = LocationType.CraftVillage;
            location.LastUpdatedTime = DateTime.UtcNow;
            location.LastUpdatedBy = currentUserId;

            // Update or create CraftVillage
            if (location.CraftVillage == null)
            {
                location.CraftVillage = new CraftVillage
                {
                    LocationId = locationId,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    Website = dto.Website,
                    WorkshopsAvailable = dto.WorkshopsAvailable,
                    SignatureProduct = dto.SignatureProduct,
                    YearsOfHistory = dto.YearsOfHistory,
                    IsRecognizedByUnesco = dto.IsRecognizedByUnesco,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow
                };
                await _unitOfWork.CraftVillageRepository.AddAsync(location.CraftVillage);
            }
            else
            {
                location.CraftVillage.PhoneNumber = dto.PhoneNumber;
                location.CraftVillage.Email = dto.Email;
                location.CraftVillage.Website = dto.Website;
                location.CraftVillage.WorkshopsAvailable = dto.WorkshopsAvailable;
                location.CraftVillage.SignatureProduct = dto.SignatureProduct;
                location.CraftVillage.YearsOfHistory = dto.YearsOfHistory;
                location.CraftVillage.IsRecognizedByUnesco = dto.IsRecognizedByUnesco;
                location.CraftVillage.LastUpdatedTime = DateTime.UtcNow;
            }

            await UpdateLocationMediasAsync(locationId, dto.MediaDtos, cancellationToken);

            await _unitOfWork.SaveAsync();

            await transaction.CommitAsync(cancellationToken);

            var createdByName = await _unitOfWork.UserRepository
               .ActiveEntities
               .Where(u => u.Id == currentUserIdGuid)
               .Select(u => u.FullName)
               .FirstOrDefaultAsync();

            var lastUpdatedByNameName = await _unitOfWork.UserRepository
                .ActiveEntities
                .Where(u => u.Id == currentUserIdGuid)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            var model = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                CreatedTime = location.CreatedTime,
                LastUpdatedTime = location.LastUpdatedTime,
                CreatedBy = location.CreatedBy,
                LastUpdatedBy = location.LastUpdatedBy,
                CreatedByName = createdByName,
                LastUpdatedByName = lastUpdatedByNameName,
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };
            return model;
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

    public async Task<LocationDataModel> AddHistoricalLocationDataAsync(Guid locationId, HistoricalLocationCreateModel? model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (model == null)
            {
                throw new InvalidOperationException("HistoricalLocation data is missing for type 'HistoricalSite'.");
            }

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            var isHistoricalLocation = location.LocationType == LocationType.HistoricalSite;
            if (!isHistoricalLocation)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location is not of type HistoricalSite.");
            }

            // Add HistoricalLocation data
            var historicalLocation = new HistoricalLocation
            {
                HeritageRank = model.HeritageRank,
                EstablishedDate = model.EstablishedDate,
                LocationId = locationId,
                TypeHistoricalLocation = model.TypeHistoricalLocation
            };
            await _unitOfWork.HistoricalLocationRepository.AddAsync(historicalLocation);

            await _unitOfWork.SaveAsync();

            var response = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            await transaction.CommitAsync(cancellationToken);
            return response;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<List<LocationDataModel>> GetNearestCuisineLocationsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location"); ;

            double currentLatitude = location.Latitude;
            double currentLongitude = location.Longitude;

            // Validate input coordinates
            if (currentLatitude < -90 || currentLatitude > 90)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid latitude value.");
            if (currentLongitude < -180 || currentLongitude > 180)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid longitude value.");

            var cuisineLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Where(l => l.LocationType == LocationType.Cuisine && l.Cuisine != null)
                .ToListAsync(cancellationToken);

            var locationsWithDistance = cuisineLocations
                .Select(location => new
                {
                    Location = location,
                    Distance = CalculateHaversineDistance(currentLatitude, currentLongitude, location.Latitude, location.Longitude)
                })
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToList();

            // Process async operations for additional data
            var result = new List<LocationDataModel>();
            foreach (var item in locationsWithDistance)
            {
                var locationData = new LocationDataModel
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Address = item.Location.Address,
                    Latitude = item.Location.Latitude,
                    Longitude = item.Location.Longitude,
                    OpenTime = item.Location.OpenTime,
                    CloseTime = item.Location.CloseTime,
                    Description = item.Location.Description,
                    Content = item.Location.Content,
                    DistrictId = item.Location.DistrictId,
                    Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(item.Location.Id, cancellationToken),
                    DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(item.Location.DistrictId ?? Guid.Empty),
                    Medias = await GetMediaWithoutVideoByIdAsync(item.Location.Id, cancellationToken),
                };
                result.Add(locationData);
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<List<LocationDataModel>> GetNearestCraftVillageLocationsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            double currentLatitude = location.Latitude;
            double currentLongitude = location.Longitude;

            // Validate input coordinates
            if (currentLatitude < -90 || currentLatitude > 90)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid latitude value.");
            if (currentLongitude < -180 || currentLongitude > 180)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid longitude value.");

            var craftVillageLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.CraftVillage)
                .Where(l => l.LocationType == LocationType.CraftVillage && l.CraftVillage != null)
                .ToListAsync(cancellationToken);

            var locationsWithDistance = craftVillageLocations
                .Select(location => new
                {
                    Location = location,
                    Distance = CalculateHaversineDistance(currentLatitude, currentLongitude, location.Latitude, location.Longitude)
                })
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToList();

            var result = new List<LocationDataModel>();
            foreach (var item in locationsWithDistance)
            {
                var locationData = new LocationDataModel
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Address = item.Location.Address,
                    Latitude = item.Location.Latitude,
                    Longitude = item.Location.Longitude,
                    OpenTime = item.Location.OpenTime,
                    CloseTime = item.Location.CloseTime,
                    Description = item.Location.Description,
                    Content = item.Location.Content,
                    DistrictId = item.Location.DistrictId,
                    Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(item.Location.Id, cancellationToken),
                    DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(item.Location.DistrictId ?? Guid.Empty),
                    Medias = await GetMediaWithoutVideoByIdAsync(item.Location.Id, cancellationToken),
                };
                result.Add(locationData);
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

    public async Task<List<LocationDataModel>> GetNearestHistoricalLocationsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            double currentLatitude = location.Latitude;
            double currentLongitude = location.Longitude;

            if (currentLatitude < -90 || currentLatitude > 90)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid latitude value.");
            if (currentLongitude < -180 || currentLongitude > 180)
                throw CustomExceptionFactory.CreateBadRequestError("Invalid longitude value.");

            var historicalLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.HistoricalLocation)
                .Where(l => l.LocationType == LocationType.HistoricalSite && l.HistoricalLocation != null)
                .ToListAsync(cancellationToken);

            var locationsWithDistance = historicalLocations
                .Select(location => new
                {
                    Location = location,
                    Distance = CalculateHaversineDistance(currentLatitude, currentLongitude, location.Latitude, location.Longitude)
                })
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToList();

            var result = new List<LocationDataModel>();
            foreach (var item in locationsWithDistance)
            {
                var locationData = new LocationDataModel
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Address = item.Location.Address,
                    Latitude = item.Location.Latitude,
                    Longitude = item.Location.Longitude,
                    OpenTime = item.Location.OpenTime,
                    CloseTime = item.Location.CloseTime,
                    Description = item.Location.Description,
                    Content = item.Location.Content,
                    DistrictId = item.Location.DistrictId,
                    Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(item.Location.Id, cancellationToken),
                    DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(item.Location.DistrictId ?? Guid.Empty),
                    Medias = await GetMediaWithoutVideoByIdAsync(item.Location.Id, cancellationToken),
                };
                result.Add(locationData);
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

    // Haversine formula to calculate distance between two points on Earth
    private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public async Task<LocationDataModel> UpdateHistoricalLocationDataAsync(Guid locationId, HistoricalLocationUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Craft village data cannot be null.");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.HistoricalLocation)

                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location not found.");
            }

            if (location.LocationType != LocationType.HistoricalSite)
            {
                throw CustomExceptionFactory.CreateBadRequestError("The specified location is not a Craft Village.");
            }

            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Content = dto.Content;
            location.Address = dto.Address;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.OpenTime = dto.OpenTime;
            location.CloseTime = dto.CloseTime;
            location.DistrictId = dto.DistrictId;
            location.LocationType = LocationType.HistoricalSite;
            location.LastUpdatedTime = DateTime.UtcNow;

            if (location.HistoricalLocation == null)
            {
                location.HistoricalLocation = new HistoricalLocation
                {
                    LocationId = locationId,
                    HeritageRank = dto.HeritageRank,
                    EstablishedDate = dto.EstablishedDate,
                    TypeHistoricalLocation = dto.TypeHistoricalLocation,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow
                };
                await _unitOfWork.HistoricalLocationRepository.AddAsync(location.HistoricalLocation);
            }
            else
            {
                location.HistoricalLocation.HeritageRank = dto.HeritageRank;
                location.HistoricalLocation.EstablishedDate = dto.EstablishedDate;
                location.HistoricalLocation.TypeHistoricalLocation = dto.TypeHistoricalLocation;
                location.HistoricalLocation.LastUpdatedTime = DateTime.UtcNow;
            }

            await UpdateLocationMediasAsync(locationId, dto.MediaDtos, cancellationToken);

            await _unitOfWork.SaveAsync();

            await transaction.CommitAsync(cancellationToken);

            var model = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            return model;
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

    public async Task<LocationDataModel> UpdateScenicSpotDataAsync(Guid locationId, LocationUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Craft village data cannot be null.");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var location = await _unitOfWork.LocationRepository
                .ActiveEntities
                .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

            if (location == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location not found.");
            }

            if (location.LocationType != LocationType.ScenicSpot)
            {
                throw CustomExceptionFactory.CreateBadRequestError("The specified location is not a Craft Village.");
            }

            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Content = dto.Content;
            location.Address = dto.Address;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.OpenTime = dto.OpenTime;
            location.CloseTime = dto.CloseTime;
            location.DistrictId = dto.DistrictId;
            location.LocationType = LocationType.ScenicSpot;
            location.LastUpdatedTime = DateTime.UtcNow;

            await UpdateLocationMediasAsync(locationId, dto.MediaDtos, cancellationToken);

            await _unitOfWork.SaveAsync();

            var model = new LocationDataModel
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Content = location.Content,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OpenTime = location.OpenTime,
                CloseTime = location.CloseTime,
                Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id),
                DistrictId = location.DistrictId,
                DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(location.DistrictId ?? Guid.Empty),
                Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken),
            };

            await transaction.CommitAsync(cancellationToken);
            return model;
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

    public async Task DeleteLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            // Load Location with related entities
            var existingLocation = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Include(l => l.Cuisine)
                .Include(l => l.CraftVillage)
                .Include(l => l.HistoricalLocation)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var isInUsing = await _unitOfWork.NewsRepository.ActiveEntities
                .AnyAsync(e => e.LocationId == id, cancellationToken);

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            if (existingLocation.Cuisine != null)
            {
                existingLocation.Cuisine.IsDeleted = true;
                existingLocation.Cuisine.DeletedBy = currentUserId;
                existingLocation.Cuisine.DeletedTime = currentTime;
                existingLocation.Cuisine.LastUpdatedBy = currentUserId;
                existingLocation.Cuisine.LastUpdatedTime = currentTime;
                _unitOfWork.CuisineRepository.Update(existingLocation.Cuisine);
            }

            if (existingLocation.CraftVillage != null)
            {
                existingLocation.CraftVillage.IsDeleted = true;
                existingLocation.CraftVillage.DeletedBy = currentUserId;
                existingLocation.CraftVillage.DeletedTime = currentTime;
                existingLocation.CraftVillage.LastUpdatedBy = currentUserId;
                existingLocation.CraftVillage.LastUpdatedTime = currentTime;
                _unitOfWork.CraftVillageRepository.Update(existingLocation.CraftVillage);
            }

            if (existingLocation.HistoricalLocation != null)
            {
                existingLocation.HistoricalLocation.IsDeleted = true;
                existingLocation.HistoricalLocation.DeletedBy = currentUserId;
                existingLocation.HistoricalLocation.DeletedTime = currentTime;
                existingLocation.HistoricalLocation.LastUpdatedBy = currentUserId;
                existingLocation.HistoricalLocation.LastUpdatedTime = currentTime;
                _unitOfWork.HistoricalLocationRepository.Update(existingLocation.HistoricalLocation);
            }

            existingLocation.IsDeleted = true;
            existingLocation.DeletedBy = currentUserId;
            existingLocation.DeletedTime = currentTime;
            existingLocation.LastUpdatedBy = currentUserId;
            existingLocation.LastUpdatedTime = currentTime;

            _unitOfWork.LocationRepository.Update(existingLocation);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
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

    public async Task<List<LocationDataModel>> GetAllLocationsAsync(CancellationToken cancellationToken)
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
                locationData.Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(locationData.Id);

                //locationData.HeritageRankName = _enumService.GetEnumDisplayName(locationData.HeritageRank);
            }

            return locationDataModels;
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

    public async Task<LocationDataDetailModel?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var cuisine = await _unitOfWork.CuisineRepository.GetByLocationId(existingLocation.Id, cancellationToken);
            var craftVillage = await _unitOfWork.CraftVillageRepository.GetByLocationId(existingLocation.Id, cancellationToken);
            var historicalLocation = await _unitOfWork.HistoricalLocationRepository.GetByLocationId(existingLocation.Id, cancellationToken);

    var locationDataModel = _mapper.Map<LocationDataDetailModel>(existingLocation);

            if (cuisine != null)
            {
                locationDataModel.Cuisine = _mapper.Map<CuisineDataModel>(cuisine);
                // locationDataModel.Cuisine.CuisineId = cuisine.Id;
            }

            if (craftVillage != null)
            {
                locationDataModel.CraftVillage = _mapper.Map<CraftVillageDataModel>(craftVillage);
                // locationDataModel.CraftVillage.CraftVillageId = craftVillage.Id;
            }

            if (historicalLocation != null)
            {
                locationDataModel.HistoricalLocation = _mapper.Map<HistoricalLocationDataModel>(historicalLocation);
                // locationDataModel.HistoricalLocation.HistoricalLocationId = historicalLocation.Id;
                locationDataModel.HistoricalLocation.HeritageRankName = _enumService.GetEnumDisplayName<HeritageRank>(historicalLocation.HeritageRank) ?? string.Empty;
            }

            locationDataModel.Medias = await GetMediaWithoutVideoByIdAsync(id, cancellationToken);
            locationDataModel.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationDataModel.DistrictId);
            locationDataModel.Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(locationDataModel.Id, cancellationToken);

            return locationDataModel;
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

    public async Task<LocationDataDetailModel?> GetLocationByIdWithVideosAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var cuisine = await _unitOfWork.CuisineRepository.GetByLocationId(existingLocation.Id, cancellationToken);
            var craftVillage = await _unitOfWork.CraftVillageRepository.GetByLocationId(existingLocation.Id, cancellationToken);
            var historicalLocation = await _unitOfWork.HistoricalLocationRepository.GetByLocationId(existingLocation.Id, cancellationToken);

            var locationDataModel = _mapper.Map<LocationDataDetailModel>(existingLocation);

            locationDataModel.Cuisine = cuisine != null ? _mapper.Map<CuisineDataModel>(cuisine) : null;
            locationDataModel.CraftVillage = craftVillage != null ? _mapper.Map<CraftVillageDataModel>(craftVillage) : null;
            locationDataModel.HistoricalLocation = historicalLocation != null ? _mapper.Map<HistoricalLocationDataModel>(historicalLocation) : null;

            locationDataModel.Medias = await GetMediaWithoutVideoByIdAsync(id, cancellationToken);
            locationDataModel.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationDataModel.DistrictId);
            locationDataModel.Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(locationDataModel.Id, cancellationToken);

            return locationDataModel;
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

    public async Task UpdateLocationAsync(Guid id, LocationUpdateModel locationUpdateModel, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
                throw CustomExceptionFactory.CreateNotFoundError("location");

            var typeSet = locationUpdateModel.Types.ToHashSet();
            var errors = new List<string>();

            if (locationUpdateModel.Cuisine != null && !typeSet.Contains(LocationType.Cuisine))
                errors.Add("Cuisine data is provided but 'Cuisine' type is missing.");
            if (locationUpdateModel.CraftVillage != null && !typeSet.Contains(LocationType.CraftVillage))
                errors.Add("CraftVillage data is provided but 'CraftVillage' type is missing.");
            if (locationUpdateModel.HistoricalLocation != null && !typeSet.Contains(LocationType.HistoricalSite))
                errors.Add("HistoricalLocation data is provided but 'HistoricalSite' type is missing.");

            if (typeSet.Contains(LocationType.Cuisine) && locationUpdateModel.Cuisine == null)
                errors.Add("Type 'Cuisine' is specified but Cuisine data is missing.");
            if (typeSet.Contains(LocationType.CraftVillage) && locationUpdateModel.CraftVillage == null)
                errors.Add("Type 'CraftVillage' is specified but CraftVillage data is missing.");
            if (typeSet.Contains(LocationType.HistoricalSite) && locationUpdateModel.HistoricalLocation == null)
                errors.Add("Type 'HistoricalSite' is specified but HistoricalLocation data is missing.");

            if (errors.Count > 0)
                throw new InvalidOperationException(string.Join(" | ", errors));

            _mapper.Map(locationUpdateModel, existingLocation);
            existingLocation.LastUpdatedBy = currentUserId;
            existingLocation.LastUpdatedTime = currentTime;

            // Cập nhật lại LocationTypes
            // existingLocation.LocationTypes.Clear();
            // foreach (var type in typeSet)
            // {
            //     existingLocation.LocationTypes.Add(new LocationTypeMapping
            //     {
            //         LocationId = existingLocation.Id,
            //         Type = type
            //     });
            // }

            // Cập nhật dữ liệu phụ
            if (locationUpdateModel.CraftVillage != null)
            {
                if (existingLocation.CraftVillage == null)
                    existingLocation.CraftVillage = new CraftVillage { LocationId = existingLocation.Id };

                existingLocation.CraftVillage.PhoneNumber = locationUpdateModel.CraftVillage.PhoneNumber;
                existingLocation.CraftVillage.Email = locationUpdateModel.CraftVillage.Email;
                existingLocation.CraftVillage.Website = locationUpdateModel.CraftVillage.Website;
                existingLocation.CraftVillage.WorkshopsAvailable = locationUpdateModel.CraftVillage.WorkshopsAvailable;
            }
            else
            {
                existingLocation.CraftVillage = null;
            }

            if (locationUpdateModel.Cuisine != null)
            {
                if (existingLocation.Cuisine == null)
                    existingLocation.Cuisine = new Cuisine { LocationId = existingLocation.Id };

                existingLocation.Cuisine.PhoneNumber = locationUpdateModel.Cuisine.PhoneNumber;
                existingLocation.Cuisine.Email = locationUpdateModel.Cuisine.Email;
                existingLocation.Cuisine.Website = locationUpdateModel.Cuisine.Website;
                existingLocation.Cuisine.CuisineType = locationUpdateModel.Cuisine.CuisineType;
            }
            else
            {
                existingLocation.Cuisine = null;
            }

            if (locationUpdateModel.HistoricalLocation != null)
            {
                if (existingLocation.HistoricalLocation == null)
                    existingLocation.HistoricalLocation = new HistoricalLocation { LocationId = existingLocation.Id };

                existingLocation.HistoricalLocation.HeritageRank = locationUpdateModel.HistoricalLocation.HeritageRank;
                existingLocation.HistoricalLocation.EstablishedDate = locationUpdateModel.HistoricalLocation.EstablishedDate;
            }
            else
            {
                existingLocation.HistoricalLocation = null;
            }

            _unitOfWork.LocationRepository.Update(existingLocation);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
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

    // có add kèm theo ảnh
    public async Task<LocationMediaResponse> AddLocationWithMediaAsync(LocationCreateWithMediaFileModel locationCreateModel, string? thumbnailSelected, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newLocation = _mapper.Map<Location>(locationCreateModel);
            newLocation.CreatedBy = currentUserId;
            newLocation.LastUpdatedBy = currentUserId;
            newLocation.CreatedTime = currentTime;
            newLocation.LastUpdatedTime = currentTime;

            await _unitOfWork.LocationRepository.AddAsync(newLocation);
            await _unitOfWork.SaveAsync();

            var mediaResponses = new List<MediaResponse>();

            if (locationCreateModel.ImageUploads != null && locationCreateModel.ImageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(locationCreateModel.ImageUploads);

                bool isAutoSelectThumbnail = string.IsNullOrEmpty(thumbnailSelected);
                bool thumbnailSet = false;

                for (int i = 0; i < locationCreateModel.ImageUploads.Count; i++)
                {
                    var imageFile = locationCreateModel.ImageUploads[i];
                    var imageUrl = imageUrls[i];

                    var newLocationMedia = new LocationMedia
                    {
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        LocationId = newLocation.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageFile.Length,
                        CreatedBy = currentUserId,
                        CreatedTime = _timeService.SystemTimeNow,
                        LastUpdatedBy = currentUserId,
                    };

                    // Chọn ảnh làm thumbnail
                    if ((isAutoSelectThumbnail && i == 0) || (!isAutoSelectThumbnail && imageFile.FileName == thumbnailSelected))
                    {
                        newLocationMedia.IsThumbnail = true;
                        thumbnailSet = true;
                    }

                    await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageFile.FileName,
                        FileType = imageFile.ContentType,
                        SizeInBytes = imageFile.Length
                    });
                }

                // Trường hợp người dùng chọn ảnh thumbnail nhưng không tìm thấy ảnh khớp
                if (!thumbnailSet && locationCreateModel.ImageUploads.Count > 0)
                {
                    var firstMedia = mediaResponses.First();
                    var firstLocationMedia = await _unitOfWork.LocationMediaRepository
                        .GetFirstByLocationIdAsync(newLocation.Id);
                    if (firstLocationMedia != null)
                    {
                        firstLocationMedia.IsThumbnail = true;
                        _unitOfWork.LocationMediaRepository.Update(firstLocationMedia);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new LocationMediaResponse
            {
                LocationId = newLocation.Id,
                LocationName = newLocation.Name,
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
    public async Task UpdateLocationAsync(
        Guid id,
        LocationUpdateWithMediaFileModel locationUpdateModel,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            _mapper.Map(locationUpdateModel, existingLocation);

            existingLocation.LastUpdatedBy = currentUserId;
            existingLocation.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.LocationRepository.Update(existingLocation);

            // xu ly anh
            var imageUploads = locationUpdateModel.ImageUploads;
            var allMedia = _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(dm => dm.LocationId == existingLocation.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin location
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.LocationMediaRepository.Update(media);
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
                if (!string.IsNullOrEmpty(thumbnailSelected) && !IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newLocationMedia = new LocationMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    LocationId = existingLocation.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);
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
                        _unitOfWork.LocationMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.LocationMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.LocationMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            // Xử lý phần ảnh
            //var imageUploads = locationUpdateModel.ImageUploads;
            //if (imageUploads == null || imageUploads.Count == 0)
            //{
            //    throw CustomExceptionFactory.CreateNotFoundError("images");
            //}

            //var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);

            //// true -> chọn ảnh cũ làm thumbnail, false -> chọn ảnh mới làm thumbnail
            //var isSelectOldThumbnail = string.IsNullOrEmpty(thumbnailSelected) || IsValidUrl(thumbnailSelected);

            //var mediaResponses = new List<MediaResponse>();

            //if (isSelectOldThumbnail)
            //{
            //    var allMedia = _unitOfWork.LocationMediaRepository.Entities
            //        .Where(dm => dm.LocationId == existingLocation.Id).ToList();
            //    if (!string.IsNullOrEmpty(thumbnailSelected))
            //    {
            //        foreach (var media in allMedia)
            //        {
            //            media.IsThumbnail = media.MediaUrl == thumbnailSelected;
            //            _unitOfWork.LocationMediaRepository.Update(media);
            //        }
            //    }
            //}

            //for (int i = 0; i < imageUploads.Count; i++)
            //{
            //    var imageUpload = imageUploads[i];
            //    bool isThumbnail = false;
            //    if (!isSelectOldThumbnail)
            //    {
            //        isThumbnail = imageUpload.FileName == thumbnailSelected
            //            || (thumbnailSelected == null && i == 0);
            //    }

            //    var newLocationMedia = new LocationMedia
            //    {
            //        FileName = imageUpload.FileName,
            //        FileType = imageUpload.ContentType,
            //        LocationId = existingLocation.Id,
            //        MediaUrl = imageUrls[i],
            //        SizeInBytes = imageUpload.Length,
            //        IsThumbnail = isThumbnail,
            //        CreatedBy = currentUserId,
            //        CreatedTime = _timeService.SystemTimeNow,
            //        LastUpdatedBy = currentUserId,
            //    };

            //    await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);

            //    mediaResponses.Add(new MediaResponse
            //    {
            //        MediaUrl = imageUrls[i],
            //        FileName = imageUpload.FileName,
            //        FileType = imageUpload.ContentType,
            //        IsThumbnail = isThumbnail,
            //        SizeInBytes = imageUpload.Length
            //    });
            //}

            //await _unitOfWork.SaveAsync();
            //await transaction.CommitAsync(cancellationToken);

            //return new LocationMediaResponse
            //{
            //    LocationId = existingLocation.Id,
            //    LocationName = existingLocation.Name,
            //    Media = mediaResponses
            //};
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

    public async Task<PagedResult<LocationDataModel>> GetPagedLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageAsync(pageNumber, pageSize);

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(pagedResult.Items);

            foreach (var location in locationDataModels)
            {
                location.Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken);
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(string? title, LocationType? type, Guid? districtId, HeritageRank? heritageRank, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(title, type, districtId, heritageRank, pageNumber, pageSize, cancellationToken);

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(pagedResult.Items);

            foreach (var locationData in locationDataModels)
            {
                locationData.Medias = await GetMediaWithoutVideoByIdAsync(locationData.Id, cancellationToken);
                locationData.DistrictName = await _unitOfWork.DistrictRepository.GetDistrictNameById(locationData.DistrictId ?? Guid.Empty);
                locationData.Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(locationData.Id);
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
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task AddFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var existingFavorite = await _unitOfWork.FavoriteLocationRepository.Entities
                .FirstOrDefaultAsync(fl => fl.UserId == Guid.Parse(currentUserId) && fl.LocationId == locationId, cancellationToken);

            if (existingFavorite != null)
            {
                if (existingFavorite.IsDeleted)
                {
                    existingFavorite.IsDeleted = false;
                    existingFavorite.LastUpdatedBy = currentUserId;
                    existingFavorite.LastUpdatedTime = currentTime;

                    _unitOfWork.FavoriteLocationRepository.Update(existingFavorite);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return;
                }
                else
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Location is already in favorites.");
                }
            }

            var favoriteLocation = new FavoriteLocation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(currentUserId),
                LocationId = locationId,
                IsDeleted = false,
                CreatedBy = currentUserId,
                LastUpdatedBy = currentUserId,
                CreatedTime = currentTime,
                LastUpdatedTime = currentTime
            };

            await _unitOfWork.FavoriteLocationRepository.AddAsync(favoriteLocation);
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

    public async Task<PagedResult<LocationDataModel>> GetFavoriteLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var totalCount = await _unitOfWork.FavoriteLocationRepository.ActiveEntities
                .Where(fl => fl.UserId == currentUserId && !fl.IsDeleted)
                .CountAsync(cancellationToken);

            var favoriteLocations = await _unitOfWork.FavoriteLocationRepository.ActiveEntities
                .Where(fl => fl.UserId == currentUserId && !fl.IsDeleted)
                .OrderByDescending(fl => fl.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(fl => fl.Location)
                .ToListAsync(cancellationToken);

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(favoriteLocations);

            foreach (var location in locationDataModels)
            {
                location.Medias = await GetMediaWithoutVideoByIdAsync(location.Id, cancellationToken);
            }

            return new PagedResult<LocationDataModel>
            {
                Items = locationDataModels,
                TotalCount = totalCount,
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
    }

    public async Task RemoveFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var favoriteLocation = await _unitOfWork.FavoriteLocationRepository.Entities
                .FirstOrDefaultAsync(fl => fl.UserId == currentUserId && fl.LocationId == locationId, cancellationToken);

            if (favoriteLocation == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("locations");
            }

            if (favoriteLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Location đã bị xóa");
            }

            favoriteLocation.IsDeleted = true;
            favoriteLocation.LastUpdatedBy = currentUserId.ToString();
            favoriteLocation.LastUpdatedTime = _timeService.SystemTimeNow;

            _unitOfWork.FavoriteLocationRepository.Update(favoriteLocation);
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

    public async Task<LocationMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile> imageUploads,
        CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            if (imageUploads == null || imageUploads.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < imageUploads.Count; i++)
            {
                var imageUpload = imageUploads[i];

                var newLocationMedia = new LocationMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    LocationId = existingLocation.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);

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

            return new LocationMediaResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
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

    public async Task<LocationMediaResponse> UploadVideoAsync(
        Guid id,
        List<IFormFile> videoUploads,
        CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            if (videoUploads == null || videoUploads.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("videos");
            }

            var videoUrls = await _cloudinaryService.UploadVideoAsync(videoUploads);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < videoUploads.Count; i++)
            {
                var imageUpload = videoUploads[i];

                var newLocationMedia = new LocationMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    LocationId = existingLocation.Id,
                    MediaUrl = videoUrls[i],
                    SizeInBytes = imageUpload.Length,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);

                mediaResponses.Add(new MediaResponse
                {
                    MediaUrl = videoUrls[i],
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    SizeInBytes = imageUpload.Length
                });
            }

            await _unitOfWork.SaveAsync();
            _unitOfWork.CommitTransaction();

            return new LocationMediaResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
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

    public async Task<LocationMediaResponse> UploadMediaAsync(
        Guid id,
        UploadMediasDto uploadMediasDto,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            if (uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var allMedia = _unitOfWork.LocationMediaRepository.Entities
                .Where(dm => dm.LocationId == existingLocation.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin location
            if ((uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new LocationMediaResponse
                {
                    LocationId = existingLocation.Id,
                    LocationName = existingLocation.Name,
                    Media = new List<MediaResponse>()
                };
            }

            bool isThumbnailUpdated = false;

            // Nếu có thumbnailSelected và nó là link (ảnh cũ) -> Cập nhật ảnh cũ làm thumbnail
            if (!string.IsNullOrEmpty(thumbnailSelected) && IsValidUrl(thumbnailSelected))
            {
                foreach (var media in allMedia)
                {
                    media.IsThumbnail = media.MediaUrl == thumbnailSelected;
                    _unitOfWork.LocationMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new LocationMediaResponse
                {
                    LocationId = existingLocation.Id,
                    LocationName = existingLocation.Name,
                    Media = new List<MediaResponse>()
                };
            }

            // Có ảnh mới -> Upload lên Cloudinary
            // var imageUrls = await _cloudinaryService.UploadImagesAsync(uploadMediasDto.Files);
            var imageUrls = await _mediaService.UploadMultipleImagesAsync(uploadMediasDto.Files);
            var mediaResponses = new List<MediaResponse>();

            for (int i = 0; i < uploadMediasDto.Files.Count; i++)
            {
                var imageUpload = uploadMediasDto.Files[i];
                bool isThumbnail = false;

                // Nếu thumbnailSelected là tên file -> Đặt ảnh mới làm thumbnail
                if (!string.IsNullOrEmpty(thumbnailSelected) && !IsValidUrl(thumbnailSelected))
                {
                    isThumbnail = imageUpload.FileName == thumbnailSelected;
                }

                var newLocationMedia = new LocationMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    LocationId = existingLocation.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);
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
                        _unitOfWork.LocationMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.LocationMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.LocationMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new LocationMediaResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
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

    public async Task<LocationMediaResponse> UpdateLocationAsync(
        Guid id,
        LocationUpdateModel locationUpdateModel,
        List<IFormFile>? imageUploads,
        string? selectedThumbnailUrl,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("district");
            }

            // Cập nhật thông tin Location
            _mapper.Map(locationUpdateModel, existingLocation);
            existingLocation.LastUpdatedBy = currentUserId;
            existingLocation.LastUpdatedTime = currentTime;
            _unitOfWork.LocationRepository.Update(existingLocation);

            // Lấy danh sách ảnh cũ
            var oldMediaList = _unitOfWork.LocationMediaRepository.Entities
                .Where(dm => dm.LocationId == existingLocation.Id)
                .ToList();

            List<MediaResponse> mediaResponses = new();
            string? thumbnailUrl = selectedThumbnailUrl;

            // Kiểm tra xem selectedThumbnailUrl có nằm trong danh sách ảnh cũ không
            bool isOldThumbnailValid = oldMediaList.Any(m => m.MediaUrl == selectedThumbnailUrl);

            // Nếu không có ảnh mới được upload và thumbnail vẫn hợp lệ, giữ nguyên
            if (imageUploads == null || imageUploads.Count == 0)
            {
                if (!isOldThumbnailValid)
                {
                    thumbnailUrl = oldMediaList.FirstOrDefault()?.MediaUrl; // Giữ ảnh đầu tiên trong ảnh cũ làm thumbnail
                }
            }

            // Xóa ảnh cũ nếu nó không được chọn làm thumbnail
            foreach (var oldMedia in oldMediaList)
            {
                if (oldMedia.MediaUrl != thumbnailUrl)
                {
                    await _cloudinaryService.DeleteImageAsync(oldMedia.MediaUrl);
                    _unitOfWork.LocationMediaRepository.Remove(oldMedia);
                }
                else
                {
                    oldMedia.IsThumbnail = true; // Giữ lại ảnh cũ làm thumbnail
                    _unitOfWork.LocationMediaRepository.Update(oldMedia);
                }
            }

            // Upload ảnh mới (nếu có)
            if (imageUploads != null && imageUploads.Count > 0)
            {
                var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);

                for (int i = 0; i < imageUploads.Count; i++)
                {
                    var imageUpload = imageUploads[i];
                    var imageUrl = imageUrls[i];

                    bool isThumbnail = (thumbnailUrl == null && i == 0) || imageUrl == selectedThumbnailUrl;

                    var newLocationMedia = new LocationMedia
                    {
                        FileName = imageUpload.FileName,
                        FileType = imageUpload.ContentType,
                        LocationId = existingLocation.Id,
                        MediaUrl = imageUrl,
                        SizeInBytes = imageUpload.Length,
                        IsThumbnail = isThumbnail,
                        CreatedBy = currentUserId,
                        CreatedTime = currentTime,
                        LastUpdatedBy = currentUserId
                    };

                    await _unitOfWork.LocationMediaRepository.AddAsync(newLocationMedia);

                    mediaResponses.Add(new MediaResponse
                    {
                        MediaUrl = imageUrl,
                        FileName = imageUpload.FileName,
                        FileType = imageUpload.ContentType,
                        SizeInBytes = imageUpload.Length,
                        IsThumbnail = isThumbnail
                    });

                    if (isThumbnail)
                    {
                        thumbnailUrl = imageUrl;
                    }
                }
            }

            // chỉ có một ảnh duy nhất làm thumbnail trong database
            var allMedia = _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(dm => dm.LocationId == existingLocation.Id);
            foreach (var media in allMedia)
            {
                media.IsThumbnail = media.MediaUrl == thumbnailUrl;
                _unitOfWork.LocationMediaRepository.Update(media);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new LocationMediaResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
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

    public async Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            if (deletedImages == null || deletedImages.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var imageUrlsDeleted = await _mediaService.DeleteImagesAsync(deletedImages);

            if (!imageUrlsDeleted)
            {
                return false;
            }

            foreach (var imageUpload in deletedImages)
            {
                var locationMedia = await _unitOfWork.LocationMediaRepository
                    .Entities
                    .FirstOrDefaultAsync(m => m.LocationId == id && m.MediaUrl.Contains(imageUpload) && !m.IsDeleted, cancellationToken);

                if (locationMedia != null)
                {
                    locationMedia.IsDeleted = true;
                    locationMedia.DeletedTime = DateTime.UtcNow;
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

    #region private methods 

    private static bool IsValidUrl(string url)
    {
        try
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
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

    private async Task<List<MediaResponse>> GetMediaByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var locationMedias = await _unitOfWork.LocationMediaRepository
            .ActiveEntities
            .Where(em => em.LocationId == locationId)
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

    private async Task EnrichLocationDataModelsAsync(List<LocationDataModel> locationDataModels, CancellationToken cancellationToken)
    {
        var locationIds = locationDataModels.Select(x => x.Id).ToList();
        var districtIds = locationDataModels.Where(x => x.DistrictId.HasValue).Select(x => x.DistrictId.Value).Distinct().ToList();

        var allMedias = await _unitOfWork.LocationMediaRepository
            .ActiveEntities
            .Where(m => locationIds.Contains(m.LocationId) && !m.FileType.Contains("video"))
            .ToListAsync(cancellationToken);

        var districtNames = await _unitOfWork.DistrictRepository
            .ActiveEntities
            .Where(d => districtIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        foreach (var location in locationDataModels)
        {
            location.Medias = _mapper.Map<List<MediaResponse>>(allMedias
                .Where(m => m.LocationId == location.Id)
                .ToList());

            if (location.DistrictId.HasValue && districtNames.TryGetValue(location.DistrictId.Value, out var districtName))
            {
                location.DistrictName = districtName;
            }

            location.Category = await _unitOfWork.LocationRepository.GetCategoryNameAsync(location.Id);

            // location.HeritageRankName = _enumService.GetEnumDisplayName(location.HeritageRank);
        }
    }

    private async Task UpdateLocationMediasAsync(Guid locationId, List<MediaDto> mediaDtos, CancellationToken cancellationToken = default)
    {
        try
        {
            if (mediaDtos == null || !mediaDtos.Any())
                return;

            var existingMedias = await _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(m => m.LocationId == locationId)
                .ToListAsync(cancellationToken);

            foreach (var mediaDto in mediaDtos)
            {
                string fileName = Path.GetFileName(new Uri(mediaDto.MediaUrl).LocalPath);
                string fileType = Path.GetExtension(fileName).TrimStart('.');

                var existingMedia = existingMedias.FirstOrDefault(m => m.MediaUrl == mediaDto.MediaUrl);

                if (existingMedia != null)
                {
                    if (existingMedia.IsThumbnail != mediaDto.IsThumbnail)
                    {
                        existingMedia.IsThumbnail = mediaDto.IsThumbnail;
                        _unitOfWork.LocationMediaRepository.Update(existingMedia);
                    }
                }
                else
                {
                    var newMedia = new LocationMedia
                    {
                        LocationId = locationId,
                        MediaUrl = mediaDto.MediaUrl,
                        FileName = fileName,
                        FileType = fileType,
                        SizeInBytes = 0,
                        IsThumbnail = mediaDto.IsThumbnail,
                        CreatedTime = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.LocationMediaRepository.AddAsync(newMedia);
                }
            }

            var allMediasAfterUpdate = await _unitOfWork.LocationMediaRepository.ActiveEntities
                .Where(m => m.LocationId == locationId)
                .ToListAsync(cancellationToken);

            var thumbnails = allMediasAfterUpdate.Where(m => m.IsThumbnail).ToList();

            if (!thumbnails.Any())
            {
                // kh có ảnh nào là thumbnail, chọn ảnh đầu tiên
                var first = allMediasAfterUpdate.FirstOrDefault();
                if (first != null)
                {
                    first.IsThumbnail = true;
                    _unitOfWork.LocationMediaRepository.Update(first);
                }
            }
            else if (thumbnails.Count > 1)
            {
                // giữ lại 1 ảnh đầu tiên làm thumbnail
                foreach (var extraThumb in thumbnails.Skip(1))
                {
                    extraThumb.IsThumbnail = false;
                    _unitOfWork.LocationMediaRepository.Update(extraThumb);
                }
            }
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

    #endregion
}
