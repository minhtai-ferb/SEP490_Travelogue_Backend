using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.TripPlanModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITripPlanService
{
    Task<TripPlanDetailResponse?> GetTripPlanByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TripPlanDataModel>> GetAllTripPlansAsync(CancellationToken cancellationToken);
    Task<TripPlanDataModel> AddTripPlanAsync(TripPlanCreateModel tripPlanCreateModel, CancellationToken cancellationToken);
    Task<TripPlanDataModel?> UpdateTripPlanAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken);
    Task DeleteTripPlanAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TripPlanDataModel>> GetPagedTripPlanWithSearchAsync(string? title, string? categoryName, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TripPlanService : ITripPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TripPlanService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public Task DeleteTripPlanAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<TripPlanDataModel>> GetAllTripPlansAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<TripPlanDataModel>> GetPagedTripPlanWithSearchAsync(string? title, string? categoryName, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<TripPlanDetailResponse?> GetTripPlanByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (tripPlan == null || tripPlan.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("news");
            }

            var activities = await GetAllActivities(id);

            var result = new TripPlanDetailResponse
            {
                Id = tripPlan.Id,
                Name = tripPlan.Name,
                Description = tripPlan.Description,
                StartDate = tripPlan.StartDate,
                EndDate = tripPlan.EndDate,
                TotalDays = (tripPlan.EndDate - tripPlan.StartDate).Days + 1,
                Days = BuildDaySchedule(tripPlan.StartDate, tripPlan.EndDate, activities)
            };

            return result;

            // if (tripPlan == null)
            // {
            //     return Task.FromResult<TripPlanDataModel?>(null);
            // }

            // return tripPlan.ContinueWith(t => _mapper.Map<TripPlanDataModel>(t.Result), cancellationToken);
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
            _unitOfWork.Dispose();
        }
    }

    private async Task<List<TripActivity>> GetAllActivities(Guid tripPlanId)
    {
        var activities = new List<TripActivity>();

        var tripPlan = await _unitOfWork.TripPlanRepository
            .ActiveEntities
            .Include(x => x.TripPlanVersions)
                .ThenInclude(x => x.TripPlanCraftVillages)
            .Include(x => x.TripPlanVersions)
                .ThenInclude(x => x.TripPlanCuisines)
            .Include(x => x.TripPlanVersions)
                .ThenInclude(x => x.TripPlanLocations)
            .FirstOrDefaultAsync(x => x.Id == tripPlanId);

        if (tripPlan == null || tripPlan.TripPlanVersionId == null)
            throw CustomExceptionFactory.CreateNotFoundError("Trip plan hoặc version");

        var version = tripPlan.TripPlanVersions.FirstOrDefault(v => v.Id == tripPlan.TripPlanVersionId);
        if (version == null)
            throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");

        // Locations
        foreach (var tpl in version.TripPlanLocations ?? new List<TripPlanLocation>())
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(tpl.LocationId, cancellationToken: CancellationToken.None);
            if (location == null) continue;

            activities.Add(new TripActivity
            {
                Id = tpl.Id,
                Type = TripActivityTypeEnum.Location.ToString(),
                Name = location.Name,
                Description = location.Description,
                Address = location.Address,
                StartTime = tpl.StartTime,
                EndTime = tpl.EndTime,
                StartTimeFormatted = tpl.StartTime?.ToString("HH:mm"),
                EndTimeFormatted = tpl.EndTime?.ToString("HH:mm"),
                Duration = $"{(tpl.EndTime - tpl.StartTime)?.TotalMinutes} phút",
                Notes = tpl.Notes,
                ImageUrl = string.Empty
            });
        }

        // Cuisines
        foreach (var tpc in version.TripPlanCuisines ?? new List<TripPlanCuisine>())
        {
            var cuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(tpc.CuisineId, cancellationToken: CancellationToken.None);
            if (cuisine == null) continue;

            activities.Add(new TripActivity
            {
                Id = tpc.Id,
                Type = TripActivityTypeEnum.Cuisine.ToString(),
                Name = cuisine.Name,
                Description = cuisine.Description,
                Address = cuisine.Address,
                StartTime = tpc.StartTime,
                EndTime = tpc.EndTime,
                StartTimeFormatted = tpc.StartTime?.ToString("HH:mm"),
                EndTimeFormatted = tpc.EndTime?.ToString("HH:mm"),
                Duration = $"{(tpc.EndTime - tpc.StartTime)?.TotalMinutes} phút",
                Notes = tpc.Notes,
                ImageUrl = string.Empty
            });
        }

        // CraftVillages
        foreach (var tpv in version.TripPlanCraftVillages ?? new List<TripPlanCraftVillage>())
        {
            var craftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(tpv.CraftVillageId, cancellationToken: CancellationToken.None);
            if (craftVillage == null) continue;

            activities.Add(new TripActivity
            {
                Id = tpv.Id,
                Type = TripActivityTypeEnum.CraftVillage.ToString(),
                Name = craftVillage.Name,
                Description = craftVillage.Description,
                Address = craftVillage.Address,
                StartTime = tpv.StartTime,
                EndTime = tpv.EndTime,
                StartTimeFormatted = tpv.StartTime?.ToString("HH:mm"),
                EndTimeFormatted = tpv.EndTime?.ToString("HH:mm"),
                Duration = $"{(tpv.EndTime - tpv.StartTime)?.TotalMinutes} phút",
                Notes = tpv.Notes,
                ImageUrl = string.Empty
            });
        }

        return activities.OrderBy(x => x.StartTime).ToList();
    }


    private async Task<string> GetLocationImageUrl(Guid locationId)
    {
        // Note: Nếu có isThumbnail thì lấy ảnh thumbnail, nếu không thì lấy ảnh đầu tiên
        var locationMedia = await _unitOfWork.LocationMediaRepository.ActiveEntities
            .FirstOrDefaultAsync(l => l.LocationId == locationId);

        return locationMedia.MediaUrl ?? string.Empty;
    }

    private List<TripDayDetail> BuildDaySchedule(DateTime startDate, DateTime endDate, List<TripActivity> activities)
    {
        var days = new List<TripDayDetail>();
        var currentDate = startDate.Date;
        var dayNumber = 1;

        while (currentDate <= endDate.Date)
        {
            var dayActivities = activities
                .Where(a => a.StartTime?.Date == currentDate ||
                           (a.StartTime == null && a.EndTime?.Date == currentDate))
                .ToList();

            dayActivities = SortActivitiesByTime(dayActivities);

            days.Add(new TripDayDetail
            {
                DayNumber = dayNumber,
                Date = currentDate,
                DateFormatted = currentDate.ToString("dd/MM/yyyy (dddd)", new CultureInfo("vi-VN")),
                Activities = dayActivities
            });

            currentDate = currentDate.AddDays(1);
            dayNumber++;
        }

        return days;
    }

    private List<TripActivity> SortActivitiesByTime(List<TripActivity> activities)
    {
        return activities.OrderBy(a =>
        {
            if (a.Order.HasValue)
                return a.Order.Value;

            if (a.StartTime.HasValue)
                return a.StartTime.Value.Hour * 60 + a.StartTime.Value.Minute;

            return int.MaxValue;
        })
        .ThenBy(a => a.StartTime ?? DateTime.MaxValue)
        .ToList();
    }

    private string? CalculateDuration(DateTime? startTime, DateTime? endTime)
    {
        if (!startTime.HasValue || !endTime.HasValue)
            return null;

        var duration = endTime.Value - startTime.Value;

        if (duration.TotalHours >= 1)
        {
            var hours = (int)duration.TotalHours;
            var minutes = duration.Minutes;
            return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours}h";
        }
        else
        {
            return $"{duration.Minutes}m";
        }
    }

    public async Task<TripPlanDataModel?> UpdateTripPlanAsync(
    Guid id,
    TripPlanUpdateModel tripPlanUpdateModel,
    CancellationToken cancellationToken)
    {
        var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
        if (!validationResult)
        {
            throw CustomExceptionFactory.CreateBadRequest("Lỗi validation lịch trình:");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .Include(tp => tp.TripPlanVersions)
                    .ThenInclude(v => v.TripPlanCuisines)
                .Include(tp => tp.TripPlanVersions)
                    .ThenInclude(v => v.TripPlanLocations)
                .Include(tp => tp.TripPlanVersions)
                    .ThenInclude(v => v.TripPlanCraftVillages)
                .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken);

            if (tripPlan == null)
                throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var newVersion = new TripPlanVersion
            {
                TripPlanId = tripPlan.Id,
                VersionDate = _timeService.SystemTimeNow,
                VersionDescription = "Cập nhật lịch trình",
                VersionNumber = tripPlan.TripPlanVersions.Count + 1,
                Status = "Draft",
            };

            await _unitOfWork.TripPlanVersionRepository.AddAsync(newVersion);
            await _unitOfWork.SaveAsync();

            // Update vào version con
            await UpdateTripPlanLocationsAsync(newVersion, tripPlanUpdateModel.Locations);
            await UpdateTripPlanCuisinesAsync(newVersion, tripPlanUpdateModel.Cuisines);
            await UpdateTripPlanCraftVillagesAsync(newVersion, tripPlanUpdateModel.CraftVillages);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return _mapper.Map<TripPlanDataModel>(tripPlan);
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

    private async Task UpdateTripPlanCuisinesAsync(
        TripPlanVersion tripPlanVersion,
        List<TripPlanCuisineModel> cuisineModels)
    {
        var existingIds = cuisineModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id.Value)
            .ToList();

        // Loại bỏ các món không còn tồn tại trong input
        var cuisinesToRemove = tripPlanVersion.TripPlanCuisines
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        _unitOfWork.TripPlanCuisineRepository.RemoveRange(cuisinesToRemove);

        foreach (var cuisineModel in cuisineModels)
        {
            if (cuisineModel.Id.HasValue)
            {
                var existingCuisine = tripPlanVersion.TripPlanCuisines
                    .FirstOrDefault(c => c.Id == cuisineModel.Id.Value);

                if (existingCuisine != null)
                {
                    existingCuisine.CuisineId = cuisineModel.CuisineId;
                    existingCuisine.StartTime = cuisineModel.StartTime ?? DateTime.Now;
                    existingCuisine.EndTime = cuisineModel.EndTime ?? DateTime.Now;
                    existingCuisine.Notes = cuisineModel.Notes;
                }
            }
            else
            {
                var newCuisine = new TripPlanCuisine
                {
                    TripPlanVersionId = tripPlanVersion.Id,
                    CuisineId = cuisineModel.CuisineId,
                    StartTime = cuisineModel.StartTime ?? DateTime.Now,
                    EndTime = cuisineModel.EndTime ?? DateTime.Now,
                    Notes = cuisineModel.Notes
                };

                await _unitOfWork.TripPlanCuisineRepository.AddAsync(newCuisine);
            }
        }
    }

    private async Task UpdateTripPlanLocationsAsync(
        TripPlanVersion tripPlanVersion,
        List<TripPlanLocationModel> locationModels)
    {
        var existingIds = locationModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id.Value)
            .ToList();

        // Loại bỏ các món không còn tồn tại trong input
        var locationsToRemove = tripPlanVersion.TripPlanLocations
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        _unitOfWork.TripPlanLocationRepository.RemoveRange(locationsToRemove);

        foreach (var locationModel in locationModels)
        {
            if (locationModel.Id.HasValue)
            {
                var existingLocation = tripPlanVersion.TripPlanLocations
                    .FirstOrDefault(c => c.Id == locationModel.Id.Value);

                if (existingLocation != null)
                {
                    existingLocation.LocationId = locationModel.LocationId;
                    existingLocation.StartTime = locationModel.StartTime ?? DateTime.Now;
                    existingLocation.EndTime = locationModel.EndTime ?? DateTime.Now;
                    existingLocation.Notes = locationModel.Notes;
                }
            }
            else
            {
                var newLocation = new TripPlanLocation
                {
                    TripPlanVersionId = tripPlanVersion.Id,
                    LocationId = locationModel.LocationId,
                    StartTime = locationModel.StartTime ?? DateTime.Now,
                    EndTime = locationModel.EndTime ?? DateTime.Now,
                    Notes = locationModel.Notes
                };

                await _unitOfWork.TripPlanLocationRepository.AddAsync(newLocation);
            }
        }
    }

    private async Task UpdateTripPlanCraftVillagesAsync(
        TripPlanVersion tripPlanVersion,
        List<TripPlanCraftVillageModel> craftVillageModels)
    {
        var existingIds = craftVillageModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id.Value)
            .ToList();

        // Loại bỏ các món không còn tồn tại trong input
        var craftVillagesToRemove = tripPlanVersion.TripPlanCraftVillages
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        _unitOfWork.TripPlanCraftVillageRepository.RemoveRange(craftVillagesToRemove);

        foreach (var craftVillageModel in craftVillageModels)
        {
            if (craftVillageModel.Id.HasValue)
            {
                var existingCraftVillage = tripPlanVersion.TripPlanCraftVillages
                    .FirstOrDefault(c => c.Id == craftVillageModel.Id.Value);

                if (existingCraftVillage != null)
                {
                    existingCraftVillage.CraftVillageId = craftVillageModel.CraftVillageId;
                    existingCraftVillage.StartTime = craftVillageModel.StartTime ?? DateTime.Now;
                    existingCraftVillage.EndTime = craftVillageModel.EndTime ?? DateTime.Now;
                    existingCraftVillage.Notes = craftVillageModel.Notes;
                }
            }
            else
            {
                var newCraftVillage = new TripPlanCraftVillage
                {
                    TripPlanVersionId = tripPlanVersion.Id,
                    CraftVillageId = craftVillageModel.CraftVillageId,
                    StartTime = craftVillageModel.StartTime ?? DateTime.Now,
                    EndTime = craftVillageModel.EndTime ?? DateTime.Now,
                    Notes = craftVillageModel.Notes
                };

                await _unitOfWork.TripPlanCraftVillageRepository.AddAsync(newCraftVillage);
            }
        }
    }

    // public async Task<TripPlanDataModel?> UpdateTripPlanNoDateTimeAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken)
    // {
    //     var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
    //     if (!validationResult)
    //     {
    //         throw CustomExceptionFactory.CreateBadRequest($"Lỗi validation lịch trình:");
    //     }

    //     using var transaction = await _unitOfWork.BeginTransactionAsync();

    //     try
    //     {
    //         var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
    //             .Include(tp => tp.TripPlanLocations)
    //             .Include(tp => tp.TripPlanCuisines)
    //             .Include(tp => tp.TripPlanCraftVillages)
    //             .FirstOrDefaultAsync(tp => tp.Id == id);

    //         if (tripPlan == null)
    //             throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

    //         tripPlan.Name = tripPlanUpdateModel.Name;
    //         tripPlan.Description = tripPlanUpdateModel.Description;
    //         tripPlan.StartDate = tripPlanUpdateModel.StartDate;
    //         tripPlan.EndDate = tripPlanUpdateModel.EndDate;

    //         // Update Locations
    //         await UpdateTripPlanLocationsAsync(tripPlan, tripPlanUpdateModel.Locations);

    //         // Update Cuisines
    //         await UpdateTripPlanCuisinesAsync(tripPlan, tripPlanUpdateModel.Cuisines);

    //         // Update Activities
    //         await UpdateTripPlanCraftVillagesAsync(tripPlan, tripPlanUpdateModel.CraftVillages);

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync();

    //         return _mapper.Map<TripPlanDataModel>(tripPlan);
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    //     finally
    //     {
    //         _unitOfWork.Dispose();
    //     }
    // }

    public async Task<TripPlanDataModel> AddTripPlanAsync(TripPlanCreateModel tripPlanCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newTripPlan = _mapper.Map<TripPlan>(tripPlanCreateModel);

            newTripPlan.UserId = Guid.Parse(currentUserId);
            newTripPlan.CreatedBy = currentUserId;
            newTripPlan.LastUpdatedBy = currentUserId;
            newTripPlan.CreatedTime = currentTime;
            newTripPlan.LastUpdatedTime = currentTime;

            await _unitOfWork.TripPlanRepository.AddAsync(newTripPlan);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == newTripPlan.Id);

            return _mapper.Map<TripPlanDataModel>(result);
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

    #region Helper Methods
    // private async Task UpdateTripPlanLocationsAsync(TripPlan tripPlan, List<TripPlanLocationModel> locationModels)
    // {
    //     // Xóa các location không còn trong danh sách mới
    //     var existingIds = locationModels.Where(l => l.Id.HasValue).Select(l => l.Id.Value).ToList();
    //     var tripPlanVersion = tripPlan.TripPlanVersions
    //         .Where(tpv => tpv.TripPlanId == tripPlan.Id)
    //         .FirstOrDefault();

    //     var locationsToRemove = tripPlanVersion.TripPlanLocations
    //         .Where(tpl => !existingIds.Contains(tpl.Id))
    //         .ToList();

    //     _unitOfWork.TripPlanLocationRepository.RemoveRange(locationsToRemove);

    //     // Update hoặc thêm mới
    //     foreach (var locationModel in locationModels)
    //     {
    //         if (locationModel.Id.HasValue)
    //         {
    //             // Update existing
    //             var existingVersion = tripPlan.TripPlanVersions
    //                 .Where(tpv => tpv.TripPlanId == tripPlan.Id)
    //                 .FirstOrDefault();

    //             var existingLocation = existingVersion.TripPlanLocations
    //                 .FirstOrDefault(tpl => tpl.Id == locationModel.Id.Value);

    //             if (existingLocation != null)
    //             {
    //                 existingLocation.LocationId = locationModel.LocationId;
    //                 existingLocation.StartTime = locationModel.StartTime ?? DateTime.Now;
    //                 existingLocation.EndTime = locationModel.EndTime ?? DateTime.Now;
    //                 existingLocation.Notes = locationModel.Notes;
    //             }
    //         }
    //         else
    //         {
    //             // Add new
    //             var newLocation = new TripPlanLocation
    //             {
    //                 TripPlanId = tripPlan.Id,
    //                 LocationId = locationModel.LocationId,
    //                 StartTime = locationModel.StartTime ?? DateTime.Now,
    //                 EndTime = locationModel.EndTime ?? DateTime.Now,
    //                 Notes = locationModel.Notes
    //             };

    //             await _unitOfWork.TripPlanLocationRepository.AddAsync(newLocation);
    //         }
    //     }
    // }

    // private async Task UpdateTripPlanCuisinesAsync(TripPlan tripPlan, List<TripPlanCuisineModel> cuisineModels)
    // {
    //     // Logic tương tự như UpdateTripPlanLocationsAsync
    //     // var existingIds = cuisineModels.Where(c => c.Id.HasValue).Select(c => c.Id.Value).ToList();
    //     // var cuisinesToRemove = tripPlan.TripPlanCuisines
    //     //     .Where(tpc => !existingIds.Contains(tpc.Id))
    //     //     .ToList();

    //     var currentVersion = tripPlan.TripPlanVersions;
    //     if (currentVersion == null)
    //         throw new Exception("Không tìm thấy phiên bản hiện tại của TripPlan");

    //     var existingIds = cuisineModels
    //         .Where(c => c.Id.HasValue)
    //         .Select(c => c.Id.Value)
    //         .ToList();

    //     var cuisinesToRemove = currentVersion.Cuisines
    //         .Where(c => !existingIds.Contains(c.Id))
    //         .ToList();

    //     _unitOfWork.TripPlanCuisineRepository.RemoveRange(cuisinesToRemove);

    //     foreach (var cuisineModel in cuisineModels)
    //     {
    //         if (cuisineModel.Id.HasValue)
    //         {
    //             var existingCuisine = tripPlan.TripPlanCuisines
    //                 .FirstOrDefault(tpc => tpc.Id == cuisineModel.Id.Value);

    //             if (existingCuisine != null)
    //             {
    //                 existingCuisine.CuisineId = cuisineModel.CuisineId;
    //                 existingCuisine.StartTime = cuisineModel.StartTime ?? DateTime.Now;
    //                 existingCuisine.EndTime = cuisineModel.EndTime ?? DateTime.Now;
    //                 existingCuisine.Notes = cuisineModel.Notes;
    //             }
    //         }
    //         else
    //         {
    //             var newCuisine = new TripPlanCuisine
    //             {
    //                 TripPlanId = tripPlan.Id,
    //                 CuisineId = cuisineModel.CuisineId,
    //                 StartTime = cuisineModel.StartTime ?? DateTime.Now,
    //                 EndTime = cuisineModel.EndTime ?? DateTime.Now,
    //                 Notes = cuisineModel.Notes
    //             };

    //             await _unitOfWork.TripPlanCuisineRepository.AddAsync(newCuisine);
    //         }
    //     }
    // }

    // private async Task UpdateTripPlanCraftVillagesAsync(TripPlan tripPlan, List<TripPlanCraftVillageModel> CraftVillageModels)
    // {
    //     // Logic tương tự như UpdateTripPlanLocationsAsync
    //     var existingIds = CraftVillageModels.Where(c => c.Id.HasValue).Select(c => c.Id.Value).ToList();
    //     var CraftVillagesToRemove = tripPlan.TripPlanCraftVillages
    //         .Where(tpc => !existingIds.Contains(tpc.Id))
    //         .ToList();

    //     _unitOfWork.TripPlanCraftVillageRepository.RemoveRange(CraftVillagesToRemove);

    //     foreach (var CraftVillageModel in CraftVillageModels)
    //     {
    //         if (CraftVillageModel.Id.HasValue)
    //         {
    //             var existingCraftVillage = tripPlan.TripPlanCraftVillages
    //                 .FirstOrDefault(tpc => tpc.Id == CraftVillageModel.Id.Value);

    //             if (existingCraftVillage != null)
    //             {
    //                 existingCraftVillage.CraftVillageId = CraftVillageModel.CraftVillageId;
    //                 existingCraftVillage.StartTime = CraftVillageModel.StartTime ?? DateTime.Now;
    //                 existingCraftVillage.EndTime = CraftVillageModel.EndTime ?? DateTime.Now;
    //                 existingCraftVillage.Notes = CraftVillageModel.Notes;
    //             }
    //         }
    //         else
    //         {
    //             var newCraftVillage = new TripPlanCraftVillage
    //             {
    //                 TripPlanId = tripPlan.Id,
    //                 CraftVillageId = CraftVillageModel.CraftVillageId,
    //                 StartTime = CraftVillageModel.StartTime ?? DateTime.Now,
    //                 EndTime = CraftVillageModel.EndTime ?? DateTime.Now,
    //                 Notes = CraftVillageModel.Notes
    //             };

    //             await _unitOfWork.TripPlanCraftVillageRepository.AddAsync(newCraftVillage);
    //         }
    //     }
    // }

    public class TripPlanItemSchedule
    {
        public string ItemType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid? ItemId { get; set; }
    }

    private List<TripPlanItemSchedule> CollectAllSchedules(TripPlanUpdateModel tripPlanUpdateModel)
    {
        var schedules = new List<TripPlanItemSchedule>();

        foreach (var location in tripPlanUpdateModel.Locations ?? new List<TripPlanLocationModel>())
        {
            schedules.Add(new TripPlanItemSchedule
            {
                ItemType = "Location",
                StartTime = location.StartTime.Value,
                EndTime = location.EndTime.Value,
                ItemId = location.Id
            });
        }

        foreach (var cuisine in tripPlanUpdateModel.Cuisines ?? new List<TripPlanCuisineModel>())
        {
            schedules.Add(new TripPlanItemSchedule
            {
                ItemType = "Cuisine",
                StartTime = cuisine.StartTime.Value,
                EndTime = cuisine.EndTime.Value,
                ItemId = cuisine.Id
            });
        }

        foreach (var craftVillage in tripPlanUpdateModel.CraftVillages ?? new List<TripPlanCraftVillageModel>())
        {
            schedules.Add(new TripPlanItemSchedule
            {
                ItemType = "CraftVillage",
                StartTime = craftVillage.StartTime.Value,
                EndTime = craftVillage.EndTime.Value,
                ItemId = craftVillage.Id
            });
        }

        return schedules.OrderBy(s => s.StartTime).ToList();
    }

    private bool ValidateDateRanges(DateTime tripStartDate, DateTime tripEndDate, List<TripPlanItemSchedule> schedules)
    {
        var result = true;

        foreach (var schedule in schedules)
        {
            if (schedule.StartTime.Date < tripStartDate.Date)
            {
                result = false; // Thời gian bắt đầu của schedule nằm trước ngày bắt đầu của trip plan
                break;
            }

            if (schedule.EndTime.Date > tripEndDate.Date)
            {
                result = false; // Thời gian kết thúc của schedule nằm sau ngày kết thúc của trip plan
                break;
            }

            // if (schedule.StartTime < tripStartDate || schedule.EndTime > tripEndDate)
            // {
            //     result = false; // Thời gian của schedule nằm ngoài khoảng thời gian của trip plan
            //     break;
            // }

            if (schedule.StartTime >= schedule.EndTime)
            {
                result = false; // Thời gian bắt đầu không được lớn hơn thời gian kết thúc
                break;
            }
        }

        return result;
    }

    private bool ValidateTimeConflicts(List<TripPlanItemSchedule> schedules)
    {
        if (schedules.Count == 0) return true;

        var result = true;

        var sortedSchedules = schedules.OrderBy(s => s.StartTime).ToList();

        for (int i = 0; i < sortedSchedules.Count - 1; i++)
        {
            var current = sortedSchedules[i];
            var next = sortedSchedules[i + 1];

            var bufferMinutes = 15;
            var bufferTime = TimeSpan.FromMinutes(bufferMinutes);

            if (current.EndTime.Add(-bufferTime) > next.StartTime)
            {
                result = false; // Có sự trùng lặp thời gian
                break;
            }
        }

        return result;
    }

    private bool ValidateLogicalTimeSequences(List<TripPlanItemSchedule> schedules)
    {
        if (schedules.Count == 0) return true;

        var result = true;

        var dailySchedules = schedules
            .GroupBy(s => s.StartTime.Date)
            .ToList();

        foreach (var dailySchedule in dailySchedules)
        {
            var daySchedules = dailySchedule.OrderBy(s => s.StartTime).ToList();

            foreach (var schedule in daySchedules)
            {
                if (schedule.StartTime.Hour < 6)
                {
                    result = false; // Không hợp lý nếu bắt đầu quá sớm
                }

                if (schedule.EndTime.Hour > 23)
                {
                    result = false; // Không hợp lý nếu kết thúc quá muộn
                }
            }

            if (!result) break;
        }

        return result;
    }

    public bool ValidateTripPlanSchedule(TripPlanUpdateModel tripPlanUpdateModel)
    {
        var result = true;

        if (tripPlanUpdateModel.StartDate >= tripPlanUpdateModel.EndDate)
        {
            result = false; // Ngày bắt đầu phải trước ngày kết thúc
        }

        var allSchedules = CollectAllSchedules(tripPlanUpdateModel);

        var dateRangeValidation = ValidateDateRanges(tripPlanUpdateModel.StartDate, tripPlanUpdateModel.EndDate, allSchedules);
        if (!dateRangeValidation)
        {
            result = false; // Có schedule nằm ngoài khoảng thời gian của trip plan
            throw CustomExceptionFactory.CreateBadRequest("Lịch trình nằm ngoài khoảng thời gian của kế hoạch chuyến đi");
        }

        var timeConflictValidation = ValidateTimeConflicts(allSchedules);
        if (!timeConflictValidation)
        {
            result = false; // Có sự trùng lặp thời gian trong các schedule
            throw CustomExceptionFactory.CreateBadRequest("Lịch trình có sự trùng lặp thời gian");
        }

        var logicalValidation = ValidateLogicalTimeSequences(allSchedules);
        if (!logicalValidation)
        {
            result = false; // Có sự không hợp lý trong trình tự thời gian
            throw CustomExceptionFactory.CreateBadRequest("Lịch trình có trình tự thời gian không hợp lý");
        }

        return result;
    }
    #endregion

    // private async Task<bool> IsTripPlanScheduleValid(TripPlanUpdateModel tripPlanUpdateModel)
    // {
    //     var schedules = await CollectAllSchedules(tripPlanUpdateModel);
    //     return ValidateTripPlanSchedule(schedules);
    // }
}
