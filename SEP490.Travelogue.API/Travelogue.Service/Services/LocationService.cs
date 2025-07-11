using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Const;
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
    Task<PagedResult<LocationDataModel>> GetPagedLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task UploadImageLocationAsync(Guid id, IFormFile image, CancellationToken cancellationToken);
    //Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken);
    Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(string? title, Guid? typeId, Guid? districtId, HeritageRank? heritageRank, int pageNumber, int pageSize, CancellationToken cancellationToken);
    //Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string title, Guid typeId, CancellationToken cancellationToken);
    Task<bool> UpdateRecommendedCraftVillagesAsync(Guid locationId, List<Guid> craftVillageIds, CancellationToken cancellationToken);
    Task<bool> AddRecommendedCraftVillagesAsync(Guid locationId, List<Guid> craftVillageIds, CancellationToken cancellationToken);
    Task<LocationCraftVillageSuggestionDataResponse> GetRecommendedCraftVillagesAsync(Guid locationId, CancellationToken cancellationToken);

    Task<bool> UpdateRecommendedCuisinesAsync(Guid locationId, List<Guid> cuisineIds, CancellationToken cancellationToken);
    Task<bool> AddRecommendedCuisinesAsync(Guid locationId, List<Guid> cuisineIds, CancellationToken cancellationToken);
    Task<LocationCuisineSuggestionDataResponse> GetRecommendedCuisinesAsync(Guid locationId, CancellationToken cancellationToken);

    Task<PagedResult<LocationDataModel>> GetFavoriteLocationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task AddFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken);
    Task RemoveFavoriteLocationAsync(Guid locationId, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadMediaAsync(Guid id, List<IFormFile> imageUploads, string? thumbnailFileName, CancellationToken cancellationToken);
    Task<LocationMediaResponse> AddLocationWithMediaAsync(LocationCreateWithMediaFileModel locationCreateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task UpdateLocationAsync(Guid id, LocationUpdateWithMediaFileModel locationUpdateModel, string? thumbnailSelected, CancellationToken cancellationToken);
    Task<bool> DeleteMediaAsync(Guid id, List<string> deletedImages, CancellationToken cancellationToken);
    Task<LocationMediaResponse> UploadVideoAsync(
        Guid id,
        List<IFormFile> videoUploads,
        CancellationToken cancellationToken);

    Task<List<LocationDataModel>> GetAllLocationAdminAsync();
    Task<LocationDataDetailModel?> GetLocationByIdWithVideosAsync(Guid id, CancellationToken cancellationToken);

    Task<LocationDataModel> CreateBasicLocationAsync(LocationCreateModel model, CancellationToken cancellationToken);
    Task<bool> AddCuisineDataAsync(Guid locationId, CuisineCreateModel? model, CancellationToken cancellationToken);
    Task<bool> AddCraftVillageDataAsync(Guid locationId, CraftVillageCreateModel? model, CancellationToken cancellationToken);
    Task<bool> AddHistoricalLocationDataAsync(Guid locationId, HistoricalLocationCreateModel? model, CancellationToken cancellationToken);
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

    // public async Task<LocationDataModel> AddLocationAsync(LocationCreateModel locationCreateModel, CancellationToken cancellationToken)
    // {
    //     using var transaction = await _unitOfWork.BeginTransactionAsync();
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var currentTime = _timeService.SystemTimeNow;

    //         var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), locationCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
    //         if (!checkRole)
    //         {
    //             throw CustomExceptionFactory.CreateForbiddenError();
    //         }

    //         var newLocation = _mapper.Map<Location>(locationCreateModel);
    //         newLocation.CreatedBy = currentUserId;
    //         newLocation.LastUpdatedBy = currentUserId;
    //         newLocation.CreatedTime = currentTime;
    //         newLocation.LastUpdatedTime = currentTime;

    //         await _unitOfWork.LocationRepository.AddAsync(newLocation);
    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         var result = _unitOfWork.LocationRepository.ActiveEntities
    //             .FirstOrDefault(l => l.Id == newLocation.Id);

    //         return _mapper.Map<LocationDataModel>(result);
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
    //     finally
    //     {
    //         //  _unitOfWork.Dispose();
    //     }
    // }

    public async Task<LocationDataModel> AddLocationAsync(LocationCreateModel locationCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), locationCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            // Danh sách type hợp lệ
            // var typeSet = locationCreateModel.Types.ToHashSet();

            // // Danh sách lỗi không đồng bộ
            // var errors = new List<string>();

            // // Kiểm tra từng loại dữ liệu có đồng bộ với type

            // if (locationCreateModel.Cuisine != null && !typeSet.Contains(LocationType.Cuisine))
            //     errors.Add("Cuisine data is provided but 'Cuisine' type is missing.");

            // if (locationCreateModel.CraftVillage != null && !typeSet.Contains(LocationType.CraftVillage))
            //     errors.Add("CraftVillage data is provided but 'CraftVillage' type is missing.");

            // if (locationCreateModel.HistoricalLocation != null && !typeSet.Contains(LocationType.HistoricalSite))
            //     errors.Add("HistoricalLocation data is provided but 'HistoricalSite' type is missing.");

            // // nếu type có nhưng không có dữ liệu
            // if (typeSet.Contains(LocationType.Cuisine) && locationCreateModel.Cuisine == null)
            //     errors.Add("Type 'Cuisine' is specified but Cuisine data is missing.");

            // if (typeSet.Contains(LocationType.CraftVillage) && locationCreateModel.CraftVillage == null)
            //     errors.Add("Type 'CraftVillage' is specified but CraftVillage data is missing.");

            // if (typeSet.Contains(LocationType.HistoricalSite) && locationCreateModel.HistoricalLocation == null)
            //     errors.Add("Type 'HistoricalSite' is specified but HistoricalLocation data is missing.");

            // if (errors.Count > 0)
            //     throw new InvalidOperationException(string.Join(" | ", errors));

            var newLocation = _mapper.Map<Location>(locationCreateModel);
            // newLocation.CreatedBy = currentUserId;
            // newLocation.LastUpdatedBy = currentUserId;
            // newLocation.CreatedTime = currentTime;
            // newLocation.LastUpdatedTime = currentTime;

            // newLocation.LocationTypes = typeSet.Select(type => new LocationTypeMapping
            // {
            //     Type = type,
            //     LocationId = newLocation.Id
            // }).ToList();

            // // Gán dữ liệu phụ nếu có

            // if (locationCreateModel.CraftVillage != null)
            // {
            //     newLocation.CraftVillage = new CraftVillage
            //     {
            //         PhoneNumber = locationCreateModel.CraftVillage.PhoneNumber,
            //         Email = locationCreateModel.CraftVillage.Email,
            //         Website = locationCreateModel.CraftVillage.Website,
            //         WorkshopsAvailable = locationCreateModel.CraftVillage.WorkshopsAvailable,
            //         LocationId = newLocation.Id
            //     };
            // }

            // if (locationCreateModel.Cuisine != null)
            // {
            //     newLocation.Cuisine = new Cuisine
            //     {
            //         PhoneNumber = locationCreateModel.Cuisine.PhoneNumber,
            //         Email = locationCreateModel.Cuisine.Email,
            //         Website = locationCreateModel.Cuisine.Website,
            //         CuisineType = locationCreateModel.Cuisine.CuisineType,
            //         LocationId = newLocation.Id
            //     };
            // }

            // if (locationCreateModel.HistoricalLocation != null)
            // {
            //     newLocation.HistoricalLocation = new HistoricalLocation
            //     {
            //         HeritageRank = locationCreateModel.HistoricalLocation.HeritageRank,
            //         EstablishedDate = locationCreateModel.HistoricalLocation.EstablishedDate,
            //         LocationId = newLocation.Id,
            //         TypeHistoricalLocationId = locationCreateModel.HistoricalLocation.TypeHistoricalLocationId,
            //     };
            // }

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
            // Validate user role
            var currentUserId = _userContextService.GetCurrentUserId();
            // var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(
            //     Guid.Parse(currentUserId),
            //     model.DistrictId ?? Guid.Empty,
            //     cancellationToken);

            // if (!checkRole)
            // {
            //     throw CustomExceptionFactory.CreateForbiddenError();
            // }

            // Map and set common fields
            var currentTime = _timeService.SystemTimeNow;
            var newLocation = _mapper.Map<Location>(model);
            newLocation.CreatedBy = currentUserId;
            newLocation.LastUpdatedBy = currentUserId;
            newLocation.CreatedTime = currentTime;
            newLocation.LastUpdatedTime = currentTime;

            // Add LocationTypeMapping
            // var typeSet = model.Types.ToHashSet();
            // newLocation.LocationTypes = typeSet.Select(type => new LocationTypeMapping
            // {
            //     Type = type,
            //     LocationId = newLocation.Id
            // }).ToList();

            await _unitOfWork.LocationRepository.AddAsync(newLocation);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            var response = _mapper.Map<LocationDataModel>(newLocation);
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
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<bool> AddCuisineDataAsync(Guid locationId, CuisineCreateModel? model, CancellationToken cancellationToken)
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

            // Add LocationTypeMapping
            var typeMapping = new LocationTypeMapping
            {
                LocationId = location.Id,
                Type = LocationType.Cuisine
            };
            await _unitOfWork.LocationTypeMappingRepository.AddAsync(typeMapping);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return true;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<bool> AddCraftVillageDataAsync(Guid locationId, CraftVillageCreateModel? model, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (model == null)
            {
                throw new InvalidOperationException("CraftVillage data is missing for type 'CraftVillage'.");
            }

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("location");

            // Add CraftVillage data
            var craftVillage = new CraftVillage
            {
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                WorkshopsAvailable = model.WorkshopsAvailable,
                LocationId = location.Id
            };
            await _unitOfWork.CraftVillageRepository.AddAsync(craftVillage);

            // Add LocationTypeMapping
            var typeMapping = new LocationTypeMapping
            {
                LocationId = location.Id,
                Type = LocationType.CraftVillage
            };
            await _unitOfWork.LocationTypeMappingRepository.AddAsync(typeMapping);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return true;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task<bool> AddHistoricalLocationDataAsync(Guid locationId, HistoricalLocationCreateModel? model, CancellationToken cancellationToken)
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

            // Add HistoricalLocation data
            var historicalLocation = new HistoricalLocation
            {
                HeritageRank = model.HeritageRank,
                EstablishedDate = model.EstablishedDate,
                LocationId = locationId,
                TypeHistoricalLocationId = model.TypeHistoricalLocationId
            };
            await _unitOfWork.HistoricalLocationRepository.AddAsync(historicalLocation);

            // Add LocationTypeMapping
            var typeMapping = new LocationTypeMapping
            {
                LocationId = locationId,
                Type = LocationType.HistoricalSite
            };
            await _unitOfWork.LocationTypeMappingRepository.AddAsync(typeMapping);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            return true;
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
            // _unitOfWork.Dispose();
        }
    }

    public async Task DeleteLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);

            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), existingLocation.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var isInUsing = await _unitOfWork.ExperienceRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null ||
                await _unitOfWork.EventRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null ||
                await _unitOfWork.NewsRepository.ActiveEntities.FirstOrDefaultAsync(e => e.LocationId == id, cancellationToken) != null;

            // xóa các trường recomment nếu location bị xóa
            await _unitOfWork.LocationCraftVillageSuggestionRepository.ActiveEntities
                .Where(s => s.LocationId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);
            //    .ToListAsync(cancellationToken);
            //existingCraftVillageSuggestions.ForEach(s => s.IsDeleted = true);

            await _unitOfWork.LocationCuisineSuggestionRepository.ActiveEntities
                .Where(s => s.LocationId == id)
                .ForEachAsync(s => s.IsDeleted = true, cancellationToken);
            //    .ToListAsync(cancellationToken);
            //existingCuisineSuggestions.ForEach(s => s.IsDeleted = true);

            if (isInUsing)
            {
                throw CustomExceptionFactory.CreateBadRequestError(ResponseMessages.BE_USED);
            }

            existingLocation.LastUpdatedBy = currentUserId;
            existingLocation.DeletedBy = currentUserId;
            existingLocation.DeletedTime = currentTime;
            existingLocation.LastUpdatedTime = currentTime;
            existingLocation.IsDeleted = true;

            _unitOfWork.LocationRepository.Update(existingLocation);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
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

    public async Task<LocationDataDetailModel?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetWithIncludeAsync(id, include => include
                .Include(l => l.LocationTypes)
            );
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
                locationDataModel.Cuisine.CuisineId = cuisine.Id;
            }

            if (craftVillage != null)
            {
                locationDataModel.CraftVillage = _mapper.Map<CraftVillageDataModel>(craftVillage);
                locationDataModel.CraftVillage.CraftVillageId = craftVillage.Id;
            }

            if (historicalLocation != null)
            {
                locationDataModel.HistoricalLocation = _mapper.Map<HistoricalLocationDataModel>(historicalLocation);
                locationDataModel.HistoricalLocation.HistoricalLocationId = historicalLocation.Id;
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

    public async Task UpdateLocationAsync(Guid id, LocationUpdateModel locationUpdateModel, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(
                Guid.Parse(currentUserId),
                locationUpdateModel.DistrictId ?? Guid.Empty,
                cancellationToken);

            if (!checkRole)
                throw CustomExceptionFactory.CreateForbiddenError();

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
            existingLocation.LocationTypes.Clear();
            foreach (var type in typeSet)
            {
                existingLocation.LocationTypes.Add(new LocationTypeMapping
                {
                    LocationId = existingLocation.Id,
                    Type = type
                });
            }

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

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), locationCreateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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

            var checkRole = await _unitOfWork.RoleRepository.CheckUserRoleForDistrict(Guid.Parse(currentUserId), locationUpdateModel.DistrictId ?? Guid.Empty, cancellationToken);
            if (!checkRole)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    public async Task<LocationCraftVillageSuggestionDataResponse> GetRecommendedCraftVillagesAsync(Guid locationId, CancellationToken cancellationToken)
    {
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            // Lấy danh sách craft village được gợi ý
            var craftVillageSuggestions = await _unitOfWork.LocationCraftVillageSuggestionRepository
                .ActiveEntities
                .Where(cv => cv.LocationId == locationId && !cv.IsDeleted && cv.CraftVillage != null && cv.CraftVillage.Location != null)
                .Select(cv => new
                {
                    cv.CraftVillageId,
                    Name = cv.CraftVillage.Location.Name ?? string.Empty,
                    Description = cv.CraftVillage.Location.Description ?? string.Empty,
                    Address = cv.CraftVillage.Location.Address ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            var craftVillageLocationIds = craftVillageSuggestions.Select(x => x.CraftVillageId).ToList();

            // Lấy media từ bảng LocationMedia (liên quan đến các CraftVillage LocationId)
            var craftVillageMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(m => m.LocationId != null && craftVillageLocationIds.Contains(m.LocationId))
                .Select(m => new
                {
                    LocationId = m.LocationId,
                    Media = new MediaResponse
                    {
                        MediaUrl = m.MediaUrl,
                        FileName = m.FileName ?? string.Empty,
                        FileType = m.FileType,
                        SizeInBytes = m.SizeInBytes,
                        CreatedTime = m.CreatedTime
                    }
                })
                .ToListAsync(cancellationToken);

            // Nhóm media theo LocationId (là CraftVillageId)
            var mediaLookup = craftVillageMedias.ToLookup(m => m.LocationId, m => m.Media);

            // Map kết quả
            var craftVillages = craftVillageSuggestions.Select(data => new CraftVillageResponse
            {
                Id = data.CraftVillageId,
                Name = data.Name,
                Description = data.Description,
                Address = data.Address,
                Medias = mediaLookup[data.CraftVillageId].ToList()
            }).ToList();

            return new LocationCraftVillageSuggestionDataResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
                RecommendedCraftVillages = craftVillages
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
    }

    public async Task<bool> AddRecommendedCraftVillagesAsync(Guid locationId, List<Guid> craftVillageIds, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            // lấy craftVillage suggestion  
            var existingCraftVillages = _unitOfWork.LocationCraftVillageSuggestionRepository
                .Entities
                .Where(lh => lh.LocationId == locationId)
                .ToList();

            var newRecommendations = new List<LocationCraftVillageSuggestion>();

            foreach (var craftVillageId in craftVillageIds)
            {
                var existingCraftVillage = existingCraftVillages.FirstOrDefault(lh => lh.CraftVillageId == craftVillageId);
                if (existingCraftVillage != null)
                {
                    if (existingCraftVillage.IsDeleted)
                    {
                        existingCraftVillage.IsDeleted = false;
                        existingCraftVillage.LastUpdatedBy = currentUserId;
                        existingCraftVillage.LastUpdatedTime = currentTime;
                        _unitOfWork.LocationCraftVillageSuggestionRepository.Update(existingCraftVillage);
                    }
                }
                else
                {
                    newRecommendations.Add(new LocationCraftVillageSuggestion
                    {
                        Id = Guid.NewGuid(),
                        LocationId = locationId,
                        CraftVillageId = craftVillageId,
                        CreatedBy = currentUserId,
                        CreatedTime = currentTime,
                        IsDeleted = false
                    });
                }
            }

            if (newRecommendations.Count != 0)
            {
                await _unitOfWork.LocationCraftVillageSuggestionRepository.AddRangeAsync(newRecommendations, cancellationToken);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> UpdateRecommendedCraftVillagesAsync(Guid locationId, List<Guid> craftVillageIds, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var existingLinks = _unitOfWork.LocationCraftVillageSuggestionRepository.Entities
                .Where(lh => lh.LocationId == locationId)
                .ToList();

            var craftVillagesToUpdate = new HashSet<Guid>(craftVillageIds);
            var updatedRecords = new List<LocationCraftVillageSuggestion>();

            foreach (var link in existingLinks)
            {
                if (craftVillagesToUpdate.Contains(link.CraftVillageId))
                {
                    if (link.IsDeleted)
                    {
                        link.IsDeleted = false;
                        link.LastUpdatedBy = currentUserId;
                        link.LastUpdatedTime = currentTime;
                        updatedRecords.Add(link);
                    }
                    craftVillagesToUpdate.Remove(link.CraftVillageId);
                }
                else
                {
                    link.IsDeleted = true;
                    link.DeletedBy = currentUserId;
                    link.DeletedTime = currentTime;
                    updatedRecords.Add(link);
                }
            }

            if (craftVillagesToUpdate.Count != 0)
            {
                var newRecommendations = craftVillagesToUpdate.Select(craftVillageId => new LocationCraftVillageSuggestion
                {
                    Id = Guid.NewGuid(),
                    LocationId = locationId,
                    CraftVillageId = craftVillageId,
                    CreatedBy = currentUserId,
                    CreatedTime = currentTime,
                    IsDeleted = false
                }).ToList();

                await _unitOfWork.LocationCraftVillageSuggestionRepository.AddRangeAsync(newRecommendations, cancellationToken);
            }

            if (updatedRecords.Any())
            {
                _unitOfWork.LocationCraftVillageSuggestionRepository.UpdateRange(updatedRecords);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<LocationCuisineSuggestionDataResponse> GetRecommendedCuisinesAsync(Guid locationId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var cuisinesData = await _unitOfWork.LocationCuisineSuggestionRepository.Entities
                .Where(h => h.LocationId == locationId && !h.IsDeleted)
                .Include(h => h.Location)
                .Select(h => new
                {
                    h.CuisineId,
                    h.Cuisine.Location.Name,
                    h.Cuisine.Location.Description,
                    h.Cuisine.Location.Address
                })
                .ToListAsync(cancellationToken);

            var cuisineIds = cuisinesData.Select(r => r.CuisineId).ToList();

            // Lấy media từ bảng LocationMedia (liên quan đến các CraftVillage LocationId)
            var cuisineMedias = await _unitOfWork.LocationMediaRepository
                .ActiveEntities
                .Where(m => m.LocationId != null && cuisineIds.Contains(m.LocationId))
                .Select(m => new
                {
                    LocationId = m.LocationId,
                    Media = new MediaResponse
                    {
                        MediaUrl = m.MediaUrl,
                        FileName = m.FileName ?? string.Empty,
                        FileType = m.FileType,
                        SizeInBytes = m.SizeInBytes,
                        CreatedTime = m.CreatedTime
                    }
                })
                .ToListAsync(cancellationToken);

            var mediaLookup = cuisineMedias.ToLookup(m => m.LocationId, m => m.Media);

            var cuisines = cuisinesData.Select(data => new CuisineResponse
            {
                Id = data.CuisineId,
                Name = data.Name,
                Description = data.Description ?? string.Empty,
                Address = data.Address ?? string.Empty,
                Medias = mediaLookup[data.CuisineId].ToList()
            }).ToList();

            await transaction.CommitAsync(cancellationToken);

            return new LocationCuisineSuggestionDataResponse
            {
                LocationId = existingLocation.Id,
                LocationName = existingLocation.Name,
                RecommendedCuisines = cuisines
            };
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> AddRecommendedCuisinesAsync(Guid locationId, List<Guid> cuisineIds, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var existingCuisines = _unitOfWork.LocationCuisineSuggestionRepository.Entities
                .Where(lh => lh.LocationId == locationId)
                .ToList();

            var newRecommendations = new List<LocationCuisineSuggestion>();

            foreach (var cuisineId in cuisineIds)
            {
                var existingCuisine = existingCuisines.FirstOrDefault(lh => lh.CuisineId == cuisineId);
                if (existingCuisine != null)
                {
                    if (existingCuisine.IsDeleted)
                    {
                        existingCuisine.IsDeleted = false;
                        existingCuisine.LastUpdatedBy = currentUserId;
                        existingCuisine.LastUpdatedTime = currentTime;
                        _unitOfWork.LocationCuisineSuggestionRepository.Update(existingCuisine);
                    }
                }
                else
                {
                    newRecommendations.Add(new LocationCuisineSuggestion
                    {
                        Id = Guid.NewGuid(),
                        LocationId = locationId,
                        CuisineId = cuisineId,
                        CreatedBy = currentUserId,
                        CreatedTime = currentTime,
                        IsDeleted = false
                    });
                }
            }

            if (newRecommendations.Any())
            {
                await _unitOfWork.LocationCuisineSuggestionRepository.AddRangeAsync(newRecommendations, cancellationToken);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> UpdateRecommendedCuisinesAsync(Guid locationId, List<Guid> cuisineIds, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null || existingLocation.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            var existingLinks = _unitOfWork.LocationCuisineSuggestionRepository.Entities
                .Where(lh => lh.LocationId == locationId)
                .ToList();

            var cuisinesToUpdate = new HashSet<Guid>(cuisineIds);
            var updatedRecords = new List<LocationCuisineSuggestion>();

            foreach (var link in existingLinks)
            {
                if (cuisinesToUpdate.Contains(link.CuisineId))
                {
                    if (link.IsDeleted)
                    {
                        link.IsDeleted = false;
                        link.LastUpdatedBy = currentUserId;
                        link.LastUpdatedTime = currentTime;
                        updatedRecords.Add(link);
                    }
                    cuisinesToUpdate.Remove(link.CuisineId);
                }
                else
                {
                    link.IsDeleted = true;
                    link.DeletedBy = currentUserId;
                    link.DeletedTime = currentTime;
                    updatedRecords.Add(link);
                }
            }

            if (cuisinesToUpdate.Count != 0)
            {
                var newRecommendations = cuisinesToUpdate.Select(cuisineId => new LocationCuisineSuggestion
                {
                    Id = Guid.NewGuid(),
                    LocationId = locationId,
                    CuisineId = cuisineId,
                    CreatedBy = currentUserId,
                    CreatedTime = currentTime,
                    IsDeleted = false
                }).ToList();

                await _unitOfWork.LocationCuisineSuggestionRepository.AddRangeAsync(newRecommendations, cancellationToken);
            }

            if (updatedRecords.Any())
            {
                _unitOfWork.LocationCuisineSuggestionRepository.UpdateRange(updatedRecords);
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(string? title, Guid? typeId, Guid? districtId, HeritageRank? heritageRank, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(title, typeId, districtId, heritageRank, pageNumber, pageSize, cancellationToken);

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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<List<LocationDataModel>> GetAllLocationAdminAsync()
    {
        try
        {
            // 1. Lấy userId nếu có
            var currentUserId = _userContextService.TryGetCurrentUserId();

            // 2. Nếu không có userId → là khách → trả toàn bộ
            if (string.IsNullOrEmpty(currentUserId))
            {
                var allLocations = await _unitOfWork.LocationRepository.GetAllAsync();
                var allLocationDataModels = _mapper.Map<List<LocationDataModel>>(allLocations);
                await EnrichLocationDataModelsAsync(allLocationDataModels, new CancellationToken());
                return allLocationDataModels;
            }

            // 3. Nếu có userId → tìm user
            var userId = Guid.Parse(currentUserId);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, CancellationToken.None);

            // 4. Nếu không tìm được user (trường hợp hiếm) → fallback: trả toàn bộ
            if (user == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("user");
            }

            // 5. Lấy role
            var roles = await _unitOfWork.UserRepository.GetRolesByUserIdAsync(userId);
            var roleNames = roles.Select(r => r.Name).ToList();
            var roleIds = roles.Select(r => r.Id).ToList();

            List<Location> locations;

            // 6. Admin toàn quyền
            if (roleNames.Equals(AppRole.ADMIN))
            {
                locations = (await _unitOfWork.LocationRepository.GetAllAsync()).ToList();
            }
            // 7. Admin huyện (dựa vào RoleDistrict)
            else
            {
                // Lấy các DistrictId mà user được phân quyền quản lý
                var allowedDistrictIds = await _unitOfWork.RoleDistrictRepository.ActiveEntities
                    .Where(rd => roleIds.Contains(rd.RoleId))
                    .Select(rd => rd.DistrictId)
                    .Distinct()
                    .ToListAsync();

                if (allowedDistrictIds.Any())
                {
                    // Là admin huyện → chỉ được lấy các Location trong danh sách huyện đó
                    locations = await _unitOfWork.LocationRepository.ActiveEntities
                        .Where(l => l.DistrictId.HasValue && allowedDistrictIds.Contains(l.DistrictId.Value))
                        .ToListAsync();
                }
                else
                {
                    // Không có quyền theo huyện nào → xem là người dùng thường
                    locations = (await _unitOfWork.LocationRepository.GetAllAsync()).ToList();
                }
            }

            var locationDataModels = _mapper.Map<List<LocationDataModel>>(locations);
            await EnrichLocationDataModelsAsync(locationDataModels, new CancellationToken());
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

    public async Task<LocationMediaResponse> UploadMediaAsync(
        Guid id,
        List<IFormFile>? imageUploads,
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

            if (imageUploads == null || imageUploads.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var allMedia = _unitOfWork.LocationMediaRepository.Entities
                .Where(dm => dm.LocationId == existingLocation.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin location
            if ((imageUploads == null || imageUploads.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
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
            if (imageUploads == null || imageUploads.Count == 0)
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
            // var imageUrls = await _cloudinaryService.UploadImagesAsync(imageUploads);
            var imageUrls = await _mediaService.UploadMultipleImagesAsync(imageUploads);
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw CustomExceptionFactory.CreateInternalServerError();
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
                    .FirstOrDefaultAsync(m => m.LocationId == id && m.MediaUrl == imageUpload && !m.IsDeleted, cancellationToken);

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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
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

            location.Categories = await _unitOfWork.LocationRepository.GetAllCategoriesAsync(location.Id);

            // location.HeritageRankName = _enumService.GetEnumDisplayName(location.HeritageRank);
        }
    }

    //public async Task UploadImageLocationAsync(Guid id, IFormFile image, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var currentUserId = _userContextService.GetCurrentUserId();
    //        var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
    //        if (existingLocation == null || existingLocation.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("location");
    //        }

    //        string imageUrl = string.Empty;

    //        if (image != null && image.Length > 0)
    //        {
    //            imageUrl = await _cloudinaryService.UploadImageAsync(image);
    //        }
    //        else
    //        {
    //            imageUrl = existingLocation.ImageUrl ?? "";
    //        }

    //        existingLocation.ImageUrl = imageUrl;
    //        existingLocation.LastUpdatedBy = currentUserId;
    //        existingLocation.LastUpdatedTime = _timeService.SystemTimeNow;

    //        _unitOfWork.BeginTransaction();
    //        _unitOfWork.LocationRepository.Update(existingLocation);
    //        _unitOfWork.CommitTransaction();
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

    //// upload nhiều hình ảnh cho location (chưa có interface)
    //public async Task UploadImagesLocationAsync(Guid id, List<IFormFile> images, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var currentUserId = _userContextService.GetCurrentUserId();
    //        var existingLocation = await _unitOfWork.LocationRepository.GetByIdAsync(id, cancellationToken);
    //        if (existingLocation == null || existingLocation.IsDeleted)
    //        {
    //            throw CustomExceptionFactory.CreateNotFoundError("location");
    //        }

    //        var imageUrls = new List<string>();

    //        if (images != null && images.Count > 0)
    //        {
    //            foreach (var image in images)
    //            {
    //                if (image.Length > 0)
    //                {
    //                    var imageUrl = await _cloudinaryService.UploadImageAsync(image);
    //                    imageUrls.Add(imageUrl);
    //                }
    //            }
    //        }
    //        // luu sang bang khac
    //        existingLocation.ImageUrl = string.Join(";", imageUrls);
    //        existingLocation.LastUpdatedBy = currentUserId;
    //        existingLocation.LastUpdatedTime = _timeService.SystemTimeNow;

    //        _unitOfWork.BeginTransaction();
    //        _unitOfWork.LocationRepository.Update(existingLocation);
    //        _unitOfWork.CommitTransaction();
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

    //public async Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(pageNumber, pageSize, name, cancellationToken);

    //        var locationDataModels = _mapper.Map<List<LocationDataModel>>(pagedResult.Items);

    //        return new PagedResult<LocationDataModel>
    //        {
    //            Items = locationDataModels,
    //            TotalCount = pagedResult.TotalCount,
    //            PageNumber = pageNumber,
    //            PageSize = pageSize
    //        };
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

    //public async Task<PagedResult<LocationDataModel>> GetPagedLocationsWithSearchAsync(int pageNumber, int pageSize, string name, Guid typeId, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var pagedResult = await _unitOfWork.LocationRepository.GetPageWithSearchAsync(pageNumber, pageSize, name, cancellationToken);

    //        var locationDataModels = _mapper.Map<List<LocationDataModel>>(pagedResult.Items);

    //        return new PagedResult<LocationDataModel>
    //        {
    //            Items = locationDataModels,
    //            TotalCount = pagedResult.TotalCount,
    //            PageNumber = pageNumber,
    //            PageSize = pageSize
    //        };
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

    //public async Task<List<Location>> GetFavoriteLocationsAsync(CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

    //        var favoriteLocations = await _unitOfWork.FavoriteLocationRepository.ActiveEntities
    //            .Where(fl => fl.UserId == currentUserId && !fl.IsDeleted)
    //            .Select(fl => fl.Location)
    //            .ToListAsync(cancellationToken);

    //        return _mapper.Map<List<LocationDataModel>>(favoriteLocations);

    //        //return favoriteLocations;
    //    }
    //    catch (Exception)
    //    {
    //        throw CustomExceptionFactory.CreateInternalServerError();
    //    }
    //}

    #endregion
}
