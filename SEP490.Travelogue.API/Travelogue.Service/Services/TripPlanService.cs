using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
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

    private async Task<List<TripActivity>> GetAllActivities(Guid id)
    {
        var activities = new List<TripActivity>();

        var tripPlanLocation = await _unitOfWork.TripPlanLocationRepository.ActiveEntities
            .Where(tpl => tpl.TripPlanId == id)
            .ToListAsync();

        foreach (var tripLocation in tripPlanLocation)
        {
            var craftVillage = await _unitOfWork.LocationRepository.GetByIdAsync(tripLocation.LocationId, new CancellationToken());
            if (craftVillage != null)
            {
                activities.Add(new TripActivity
                {
                    Id = tripLocation.Id,
                    Type = "Location",
                    Name = craftVillage.Name,
                    Description = craftVillage.Description,
                    Address = craftVillage.Address,
                    StartTime = tripLocation.StartTime,
                    EndTime = tripLocation.EndTime,
                    StartTimeFormatted = tripLocation.StartTime?.ToString("HH:mm"),
                    EndTimeFormatted = tripLocation.EndTime?.ToString("HH:mm"),
                    Duration = $"{(tripLocation.EndTime - tripLocation.StartTime)?.TotalMinutes} phút",
                    Notes = tripLocation.Notes,
                    // Order = location.Order,
                    ImageUrl = string.Empty
                    //await GetLocationImageUrl(location.LocationId),
                    // Rating = location.Location.Rating,
                    // ContactInfo = location.Location.ContactInfo
                });
            }
        }

        var tripPlanCuisine = await _unitOfWork.TripPlanCuisineRepository.ActiveEntities
            .Where(tpl => tpl.TripPlanId == id)
            .ToListAsync();

        foreach (var tripCuisine in tripPlanCuisine)
        {
            var craftVillage = await _unitOfWork.CuisineRepository.GetByIdAsync(tripCuisine.CuisineId, new CancellationToken());
            if (craftVillage != null)
            {
                activities.Add(new TripActivity
                {
                    Id = tripCuisine.Id,
                    Type = "Cuisine",
                    Name = craftVillage.Name,
                    Description = craftVillage.Description,
                    Address = craftVillage.Address,
                    StartTime = tripCuisine.StartTime,
                    EndTime = tripCuisine.EndTime,
                    StartTimeFormatted = tripCuisine.StartTime?.ToString("HH:mm"),
                    EndTimeFormatted = tripCuisine.EndTime?.ToString("HH:mm"),
                    Duration = $"{(tripCuisine.EndTime - tripCuisine.StartTime)?.TotalMinutes} phút",
                    Notes = tripCuisine.Notes,
                    // Order = location.Order,
                    ImageUrl = string.Empty
                    //await GetCuisineImageUrl(location.CuisineId),
                    // Rating = location.Cuisine.Rating,
                    // ContactInfo = location.Cuisine.ContactInfo
                });
            }
        }

        var tripPlanCraftVillage = await _unitOfWork.TripPlanCraftVillageRepository.ActiveEntities
            .Where(tpl => tpl.TripPlanId == id)
            .ToListAsync();

        foreach (var tripCraftVillage in tripPlanCraftVillage)
        {
            var craftVillage = await _unitOfWork.CraftVillageRepository.GetByIdAsync(tripCraftVillage.CraftVillageId, new CancellationToken());
            if (craftVillage != null)
            {
                activities.Add(new TripActivity
                {
                    Id = tripCraftVillage.Id,
                    Type = "CraftVillage",
                    Name = craftVillage.Name,
                    Description = craftVillage.Description,
                    Address = craftVillage.Address,
                    StartTime = tripCraftVillage.StartTime,
                    EndTime = tripCraftVillage.EndTime,
                    StartTimeFormatted = tripCraftVillage.StartTime?.ToString("HH:mm"),
                    EndTimeFormatted = tripCraftVillage.EndTime?.ToString("HH:mm"),
                    Duration = $"{(tripCraftVillage.EndTime - tripCraftVillage.StartTime)?.TotalMinutes} phút",
                    Notes = tripCraftVillage.Notes,
                    // Order = location.Order,
                    ImageUrl = string.Empty
                    //await GetCraftVillageImageUrl(location.CraftVillageId),
                    // Rating = location.CraftVillage.Rating,
                    // ContactInfo = location.CraftVillage.ContactInfo
                });
            }
        }

        return activities.OrderBy(a => a.StartTime).ToList();
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

    public async Task<TripPlanDataModel?> UpdateTripPlanAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken)
    {
        var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
        if (!validationResult)
        {
            throw CustomExceptionFactory.CreateBadRequest($"Lỗi validation lịch trình:");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .Include(tp => tp.TripPlanLocations)
                .Include(tp => tp.TripPlanCuisines)
                .Include(tp => tp.TripPlanCraftVillages)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (tripPlan == null)
                throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            tripPlan.Name = tripPlanUpdateModel.Name;
            tripPlan.Description = tripPlanUpdateModel.Description;
            tripPlan.StartDate = tripPlanUpdateModel.StartDate;
            tripPlan.EndDate = tripPlanUpdateModel.EndDate;

            // Update Locations
            await UpdateTripPlanLocationsAsync(tripPlan, tripPlanUpdateModel.Locations);

            // Update Cuisines
            await UpdateTripPlanCuisinesAsync(tripPlan, tripPlanUpdateModel.Cuisines);

            // Update Activities
            await UpdateTripPlanCraftVillagesAsync(tripPlan, tripPlanUpdateModel.CraftVillages);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

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

    public async Task<TripPlanDataModel?> UpdateTripPlanNoDateTimeAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken)
    {
        var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
        if (!validationResult)
        {
            throw CustomExceptionFactory.CreateBadRequest($"Lỗi validation lịch trình:");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .Include(tp => tp.TripPlanLocations)
                .Include(tp => tp.TripPlanCuisines)
                .Include(tp => tp.TripPlanCraftVillages)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (tripPlan == null)
                throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            tripPlan.Name = tripPlanUpdateModel.Name;
            tripPlan.Description = tripPlanUpdateModel.Description;
            tripPlan.StartDate = tripPlanUpdateModel.StartDate;
            tripPlan.EndDate = tripPlanUpdateModel.EndDate;

            // Update Locations
            await UpdateTripPlanLocationsAsync(tripPlan, tripPlanUpdateModel.Locations);

            // Update Cuisines
            await UpdateTripPlanCuisinesAsync(tripPlan, tripPlanUpdateModel.Cuisines);

            // Update Activities
            await UpdateTripPlanCraftVillagesAsync(tripPlan, tripPlanUpdateModel.CraftVillages);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

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
    private async Task UpdateTripPlanLocationsAsync(TripPlan tripPlan, List<TripPlanLocationModel> locationModels)
    {
        // Xóa các location không còn trong danh sách mới
        var existingIds = locationModels.Where(l => l.Id.HasValue).Select(l => l.Id.Value).ToList();
        var locationsToRemove = tripPlan.TripPlanLocations
            .Where(tpl => !existingIds.Contains(tpl.Id))
            .ToList();

        _unitOfWork.TripPlanLocationRepository.RemoveRange(locationsToRemove);

        // Update hoặc thêm mới
        foreach (var locationModel in locationModels)
        {
            if (locationModel.Id.HasValue)
            {
                // Update existing
                var existingLocation = tripPlan.TripPlanLocations
                    .FirstOrDefault(tpl => tpl.Id == locationModel.Id.Value);

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
                // Add new
                var newLocation = new TripPlanLocation
                {
                    TripPlanId = tripPlan.Id,
                    LocationId = locationModel.LocationId,
                    StartTime = locationModel.StartTime ?? DateTime.Now,
                    EndTime = locationModel.EndTime ?? DateTime.Now,
                    Notes = locationModel.Notes
                };

                await _unitOfWork.TripPlanLocationRepository.AddAsync(newLocation);
            }
        }
    }

    private async Task UpdateTripPlanCuisinesAsync(TripPlan tripPlan, List<TripPlanCuisineModel> cuisineModels)
    {
        // Logic tương tự như UpdateTripPlanLocationsAsync
        var existingIds = cuisineModels.Where(c => c.Id.HasValue).Select(c => c.Id.Value).ToList();
        var cuisinesToRemove = tripPlan.TripPlanCuisines
            .Where(tpc => !existingIds.Contains(tpc.Id))
            .ToList();

        _unitOfWork.TripPlanCuisineRepository.RemoveRange(cuisinesToRemove);

        foreach (var cuisineModel in cuisineModels)
        {
            if (cuisineModel.Id.HasValue)
            {
                var existingCuisine = tripPlan.TripPlanCuisines
                    .FirstOrDefault(tpc => tpc.Id == cuisineModel.Id.Value);

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
                    TripPlanId = tripPlan.Id,
                    CuisineId = cuisineModel.CuisineId,
                    StartTime = cuisineModel.StartTime ?? DateTime.Now,
                    EndTime = cuisineModel.EndTime ?? DateTime.Now,
                    Notes = cuisineModel.Notes
                };

                await _unitOfWork.TripPlanCuisineRepository.AddAsync(newCuisine);
            }
        }
    }

    private async Task UpdateTripPlanCraftVillagesAsync(TripPlan tripPlan, List<TripPlanCraftVillageModel> CraftVillageModels)
    {
        // Logic tương tự như UpdateTripPlanLocationsAsync
        var existingIds = CraftVillageModels.Where(c => c.Id.HasValue).Select(c => c.Id.Value).ToList();
        var CraftVillagesToRemove = tripPlan.TripPlanCraftVillages
            .Where(tpc => !existingIds.Contains(tpc.Id))
            .ToList();

        _unitOfWork.TripPlanCraftVillageRepository.RemoveRange(CraftVillagesToRemove);

        foreach (var CraftVillageModel in CraftVillageModels)
        {
            if (CraftVillageModel.Id.HasValue)
            {
                var existingCraftVillage = tripPlan.TripPlanCraftVillages
                    .FirstOrDefault(tpc => tpc.Id == CraftVillageModel.Id.Value);

                if (existingCraftVillage != null)
                {
                    existingCraftVillage.CraftVillageId = CraftVillageModel.CraftVillageId;
                    existingCraftVillage.StartTime = CraftVillageModel.StartTime ?? DateTime.Now;
                    existingCraftVillage.EndTime = CraftVillageModel.EndTime ?? DateTime.Now;
                    existingCraftVillage.Notes = CraftVillageModel.Notes;
                }
            }
            else
            {
                var newCraftVillage = new TripPlanCraftVillage
                {
                    TripPlanId = tripPlan.Id,
                    CraftVillageId = CraftVillageModel.CraftVillageId,
                    StartTime = CraftVillageModel.StartTime ?? DateTime.Now,
                    EndTime = CraftVillageModel.EndTime ?? DateTime.Now,
                    Notes = CraftVillageModel.Notes
                };

                await _unitOfWork.TripPlanCraftVillageRepository.AddAsync(newCraftVillage);
            }
        }
    }

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
