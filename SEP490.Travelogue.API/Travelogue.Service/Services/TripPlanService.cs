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
    /// <summary>
    /// Lấy chi tiết kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch chuyến đi.</param>
    Task<TripPlanDetailResponse?> GetTripPlanByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy toàn bộ danh sách kế hoạch chuyến đi.
    /// </summary>
    Task<List<TripPlanResponseDto>> GetAllTripPlansAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Thêm mới một kế hoạch chuyến đi.
    /// </summary>
    /// <param name="tripPlanCreateModel">Dữ liệu đầu vào để tạo kế hoạch.</param>
    Task<TripPlanResponseDto> AddTripPlanAsync(TripPlanCreateModel tripPlanCreateModel, CancellationToken cancellationToken);

    Task<TripPlanResponseDto> UpdateTripPlanAsync(Guid tripPlanId, TripPlanUpdateDto dto);

    /// <summary>
    /// Cập nhật kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch cần cập nhật.</param>
    /// <param name="tripPlanUpdateModel">Dữ liệu cập nhật.</param>
    Task<TripPlanResponseDto?> UpdateTripPlanAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken);

    Task<List<TripPlanLocationResponseDto>> UpdateTripPlanLocationsAsync(Guid tripPlanId, List<UpdateTripPlanLocationDto> dtos);

    /// <summary>
    /// Xóa kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch cần xóa.</param>
    Task DeleteTripPlanAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy danh sách kế hoạch chuyến đi có phân trang và tìm kiếm theo tiêu đề.
    /// </summary>
    /// <param name="title">Tiêu đề kế hoạch (tùy chọn, để tìm kiếm).</param>
    /// <param name="pageNumber">Trang hiện tại.</param>
    /// <param name="pageSize">Số lượng phần tử mỗi trang.</param>
    Task<PagedResult<TripPlanResponseDto>> GetPagedTripPlanWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> UpdateTripPlanImageUrlAsync(Guid tripPlanId, string imageUrl, CancellationToken cancellationToken);
}

public class TripPlanService : ITripPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IEnumService _enumService;

    public TripPlanService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _enumService = enumService;
    }
    public async Task DeleteTripPlanAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());
            var tripPlan = _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == id && tp.UserId == userId);
            if (tripPlan == null || tripPlan.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("trip plan");
            }
            tripPlan.IsDeleted = true;
            _unitOfWork.TripPlanRepository.Update(tripPlan);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
        }
        catch (CustomException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public Task<List<TripPlanResponseDto>> GetAllTripPlansAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<TripPlanResponseDto>> GetPagedTripPlanWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            Guid currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var pagedResult = await _unitOfWork.TripPlanRepository.GetPageWithSearchAsync(title, pageNumber, pageSize, cancellationToken);

            var tripPlanResponseModel = _mapper.Map<List<TripPlanResponseDto>>(pagedResult.Items);

            tripPlanResponseModel = tripPlanResponseModel.Where(x => x.UserId == currentUserId).ToList();

            foreach (var tripPlan in tripPlanResponseModel)
            {
                tripPlan.OwnerName = await _unitOfWork.UserRepository.GetUserNameByIdAsync(tripPlan.UserId) ?? string.Empty;
                tripPlan.Status = tripPlan.Status;
                tripPlan.StatusText = _enumService.GetEnumDisplayName<TripPlanStatus>(tripPlan.Status);
            }

            var result = new PagedResult<TripPlanResponseDto>
            {
                Items = tripPlanResponseModel,
                TotalCount = pagedResult.TotalCount,
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
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<TripPlanResponseDto> UpdateTripPlanAsync(Guid tripPlanId, TripPlanUpdateDto dto)
    {
        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository
                .ActiveEntities
                .Include(t => t.TripPlanLocations)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tripPlanId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TripPlan");

            if (tripPlan.Bookings.Any(b => b.Status == BookingStatus.Confirmed) || tripPlan.Status == TripPlanStatus.Booked)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật Trip plan đã được book");

            // Validate date range
            if (dto.StartDate > dto.EndDate)
                throw CustomExceptionFactory.CreateBadRequestError("StartDate must be before EndDate.");

            // Check date constraints if changed
            bool datesChanged = tripPlan.StartDate != dto.StartDate || tripPlan.EndDate != dto.EndDate;
            if (datesChanged)
            {
                var tripPlanStartDate = dto.StartDate.Date;
                var tripPlanEndDate = dto.EndDate.Date.AddDays(1).AddTicks(-1);

                foreach (var location in tripPlan.TripPlanLocations.Where(l => !l.IsDeleted))
                {
                    if (location.StartTime < tripPlanStartDate || location.StartTime > tripPlanEndDate)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Start time {location.StartTime} for location {location.LocationId} is outside the new TripPlan date range."
                        );
                    if (location.EndTime < tripPlanStartDate || location.EndTime > tripPlanEndDate)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"End time {location.EndTime} for location {location.LocationId} is outside the new TripPlan date range."
                        );
                }
            }

            // Map new values from dto
            tripPlan.Name = dto.Name;
            tripPlan.Description = dto.Description;
            tripPlan.StartDate = dto.StartDate;
            tripPlan.EndDate = dto.EndDate;
            tripPlan.ImageUrl = dto.ImageUrl;
            tripPlan.LastUpdatedTime = DateTimeOffset.UtcNow;

            tripPlan.Status = TripPlanStatus.Sketch;

            await _unitOfWork.SaveAsync();

            var ownerName = await _unitOfWork.UserRepository
                .GetUserNameByIdAsync(tripPlan.UserId) ?? string.Empty;

            return new TripPlanResponseDto
            {
                Id = tripPlan.Id,
                Name = tripPlan.Name,
                Description = tripPlan.Description,
                StartDate = tripPlan.StartDate,
                EndDate = tripPlan.EndDate,
                ImageUrl = tripPlan.ImageUrl,
                Status = tripPlan.Status,
                StatusText = _enumService.GetEnumDisplayName<TripPlanStatus>(tripPlan.Status),
                UserId = tripPlan.UserId,
                OwnerName = ownerName,
                CreatedTime = tripPlan.CreatedTime,
                LastUpdatedTime = tripPlan.LastUpdatedTime
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


    public async Task<TripPlanDetailResponse?> GetTripPlanByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (tripPlan == null || tripPlan.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("trip plan");
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
                Status = tripPlan.Status,
                StatusText = _enumService.GetEnumDisplayName<TripPlanStatus>(tripPlan.Status),
                Days = BuildDaySchedule(tripPlan.StartDate, tripPlan.EndDate, activities)
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
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    private async Task<List<TripActivity>> GetAllActivities(Guid tripPlanId)
    {
        var activities = new List<TripActivity>();

        var tripPlan = await _unitOfWork.TripPlanRepository
            .ActiveEntities
            .Include(x => x.TripPlanLocations.Where(tpl => !tpl.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == tripPlanId);

        if (tripPlan == null)
            throw CustomExceptionFactory.CreateNotFoundError("Trip plan hoặc version");

        // Locations
        foreach (var tpl in tripPlan.TripPlanLocations ?? new List<TripPlanLocation>())
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(tpl.LocationId, cancellationToken: CancellationToken.None);
            if (location == null) continue;

            var imageUrl = await GetLocationImageUrl(tpl.LocationId);
            activities.Add(new TripActivity
            {
                TripPlanLocationId = tpl.Id,
                LocationId = tpl.LocationId,
                Type = _enumService.GetEnumDisplayName<LocationType>(location.LocationType),
                Name = location.Name,
                Description = location.Description,
                Address = location.Address,
                StartTime = tpl.StartTime,
                EndTime = tpl.EndTime,
                StartTimeFormatted = tpl.StartTime?.ToString("HH:mm"),
                EndTimeFormatted = tpl.EndTime?.ToString("HH:mm"),
                Duration = $"{(tpl.EndTime - tpl.StartTime)?.TotalMinutes} phút",
                Notes = tpl.Notes,
                Order = tpl.Order,
                ImageUrl = imageUrl ?? string.Empty,
            });
        }

        // {
        //     var cuisine = await _unitOfWork.CuisineRepository.GetByIdAsync(tpc.CuisineId, cancellationToken: CancellationToken.None);
        //     if (cuisine == null) continue;

        //     activities.Add(new TripActivity
        //     {
        //         Id = tpc.Id,
        //         Type = TripActivityTypeEnum.Cuisine.ToString(),
        //         Name = cuisine.Name,
        //         Description = cuisine.Description,
        //         Address = cuisine.Address,
        //         StartTime = tpc.StartTime,
        //         EndTime = tpc.EndTime,
        //         StartTimeFormatted = tpc.StartTime?.ToString("HH:mm"),
        //         EndTimeFormatted = tpc.EndTime?.ToString("HH:mm"),
        //         Duration = $"{(tpc.EndTime - tpc.StartTime)?.TotalMinutes} phút",
        //         Notes = tpc.Notes,
        //         ImageUrl = string.Empty
        //     });

        return activities.OrderBy(x => x.StartTime).ToList();
    }

    private async Task<string> GetLocationImageUrl(Guid locationId)
    {
        try
        {
            // Note: Nếu có isThumbnail thì lấy ảnh thumbnail, nếu không thì lấy ảnh đầu tiên
            var locationMedia = await _unitOfWork.LocationMediaRepository.ActiveEntities
                .FirstOrDefaultAsync(l => l.LocationId == locationId);

            return locationMedia?.MediaUrl ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError($"Lỗi khi lấy ảnh địa điểm: {ex.Message}");
        }
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

    // public async Task<TripPlanDataModel?> UpdateTripPlanAsync(
    //     Guid id,
    //     TripPlanUpdateModel tripPlanUpdateModel,
    //     CancellationToken cancellationToken)
    // {
    //     var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
    //     if (!validationResult)
    //     {
    //         throw CustomExceptionFactory.CreateBadRequestError("Lỗi validation lịch trình:");
    //     }

    //     using var transaction = await _unitOfWork.BeginTransactionAsync();

    //     try
    //     {
    //         var tripPlan = await _unitOfWork.TripPlanRepository
    //             .ActiveEntities
    //             .Include(tp => tp.TripPlanVersions)
    //                 .ThenInclude(v => v.TripPlanLocations)
    //             .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");
    //         var newVersion = new TripPlanVersion
    //         {
    //             TripPlanId = tripPlan.Id,
    //             VersionDate = _timeService.SystemTimeNow,
    //             Description = "Cập nhật lịch trình",
    //             VersionNumber = tripPlan.TripPlanVersions.Count + 1,
    //             Status = "Draft",
    //         };

    //         await _unitOfWork.TripPlanVersionRepository.AddAsync(newVersion);
    //         await _unitOfWork.SaveAsync();

    //         // Update vào version con
    //         if (tripPlanUpdateModel.Locations == null)
    //         {
    //             tripPlanUpdateModel.Locations = new List<TripPlanLocationModel>();
    //         }
    //         await UpdateTripPlanLocationsAsync(newVersion, tripPlanUpdateModel.Locations);
    //         // await UpdateTripPlanCuisinesAsync(newVersion, tripPlanUpdateModel.Cuisines);
    //         // await UpdateTripPlanCraftVillagesAsync(newVersion, tripPlanUpdateModel.CraftVillages);

    //         _mapper.Map(tripPlanUpdateModel, tripPlan);
    //         tripPlan.UserTripPlanVersionId = newVersion.Id;

    //         await _unitOfWork.SaveAsync();
    //         await transaction.CommitAsync(cancellationToken);

    //         return _mapper.Map<TripPlanDataModel>(tripPlan);
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    //     finally
    //     {
    //         ////  _unitOfWork.Dispose();
    //     }
    // }

    public async Task<TripPlanResponseDto?> UpdateTripPlanAsync(
        Guid id,
        TripPlanUpdateModel tripPlanUpdateModel,
        CancellationToken cancellationToken)
    {
        var validationResult = ValidateTripPlanSchedule(tripPlanUpdateModel);
        if (!validationResult)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Lỗi validation lịch trình:");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository
                .ActiveEntities
                .Include(v => v.TripPlanLocations)
                .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var currentUserId = _userContextService.GetCurrentUserId();
            var userRoles = await _userContextService.GetCurrentUserRolesAsync();

            // Update vào version con
            if (tripPlanUpdateModel.Locations == null)
            {
                tripPlanUpdateModel.Locations = new List<TripPlanLocationModel>();
            }

            await UpdateTripPlanLocationsAsync(tripPlan, tripPlanUpdateModel.Locations);


            _mapper.Map(tripPlanUpdateModel, tripPlan);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _mapper.Map<TripPlanResponseDto>(tripPlan);

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

    public async Task<List<TripPlanLocationResponseDto>> UpdateTripPlanLocationsAsync(Guid tripPlanId, List<UpdateTripPlanLocationDto> dtos)
    {
        try
        {
            var tripPlan = await _unitOfWork.TripPlanRepository
                .ActiveEntities
                .Include(t => t.TripPlanLocations)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tripPlanId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TripPlan");

            foreach (var dto in dtos)
            {
                if (dto.TripPlanLocationId.HasValue && !tripPlan.TripPlanLocations.Any(l => l.Id == dto.TripPlanLocationId && !l.IsDeleted))
                    throw CustomExceptionFactory.CreateBadRequestError($"Location with ID {dto.TripPlanLocationId} not found or already deleted.");
                if (dto.LocationId == Guid.Empty)
                    throw CustomExceptionFactory.CreateBadRequestError($"Location ID is required for Order {dto.Order}.");
                if (dto.Order < 1)
                    throw CustomExceptionFactory.CreateBadRequestError($"Order must be positive for location {dto.LocationId}.");
                if (dto.StartTime >= dto.EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"End time must be after start time for location {dto.LocationId}.");
            }

            if (tripPlan.Bookings.Any(b => b.Status == BookingStatus.Confirmed) || tripPlan.Status == TripPlanStatus.Booked)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật Trip plan đã được book");

            // Validate LocationId
            var locationIds = dtos.Select(d => d.LocationId).Distinct().ToList();
            var validLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Where(l => locationIds.Contains(l.Id))
                .Select(l => l.Id)
                .ToListAsync();
            var invalidLocations = locationIds.Except(validLocations).ToList();
            if (invalidLocations.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"Invalid location IDs: {string.Join(", ", invalidLocations)}");

            var existingLocations = tripPlan.TripPlanLocations.Where(l => !l.IsDeleted).ToList();
            var providedLocationIds = dtos.Where(d => d.TripPlanLocationId.HasValue).Select(d => d.TripPlanLocationId.Value).ToList();
            var toDelete = existingLocations.Where(l => !providedLocationIds.Contains(l.Id)).ToList();
            var toAdd = dtos.Where(d => !d.TripPlanLocationId.HasValue)
                .Select(d => new TripPlanLocation
                {
                    TripPlanId = tripPlanId,
                    LocationId = d.LocationId,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Notes = d.Notes,
                    Order = d.Order,
                    TravelTimeFromPrev = d.TravelTimeFromPrev,
                    DistanceFromPrev = d.DistanceFromPrev,
                    EstimatedStartTime = d.EstimatedStartTime,
                    EstimatedEndTime = d.EstimatedEndTime,
                    IsActive = true,
                    IsDeleted = false
                }).ToList();
            var toUpdate = dtos.Where(d => d.TripPlanLocationId.HasValue).ToList();

            var updateIds = toUpdate.Select(u => u.TripPlanLocationId.Value).ToHashSet();

            // Validate time overlaps
            var allLocations = existingLocations
                 .Where(l => !toDelete.Contains(l) && !updateIds.Contains(l.Id))
                .Select(l => new { l.Id, l.StartTime, l.EndTime, l.Order })
                .Concat(toAdd.Select(l => new { Id = Guid.Empty, l.StartTime, l.EndTime, l.Order }))
                .Concat(toUpdate.Select(u => new { Id = u.TripPlanLocationId.Value, u.StartTime, u.EndTime, u.Order }))
                .GroupBy(l => l.Order)
                .ToList();

            foreach (var group in allLocations)
            {
                var locationsInOrder = group.OrderBy(l => l.StartTime).ToList();
                for (int i = 1; i < locationsInOrder.Count; i++)
                {
                    if (locationsInOrder[i].StartTime < locationsInOrder[i - 1].EndTime)
                        throw CustomExceptionFactory.CreateBadRequestError($"Time overlap detected in Order {group.Key}.");
                }
            }

            // Validate date range
            var tripPlanStartDate = tripPlan.StartDate.Date;
            var tripPlanEndDate = tripPlan.EndDate.Date.AddDays(1).AddTicks(-1);
            foreach (var location in allLocations.SelectMany(g => g))
            {
                if (location.StartTime < tripPlanStartDate || location.StartTime > tripPlanEndDate)
                    throw CustomExceptionFactory.CreateBadRequestError($"Start time {location.StartTime} for location is outside the TripPlan date range ({tripPlanStartDate} to {tripPlanEndDate}).");
                if (location.EndTime < tripPlanStartDate || location.EndTime > tripPlanEndDate)
                    throw CustomExceptionFactory.CreateBadRequestError($"End time {location.EndTime} for location is outside the TripPlan date range ({tripPlanStartDate} to {tripPlanEndDate}).");
            }

            var changes = new List<string>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (var location in toDelete)
                    {
                        location.IsDeleted = true;
                        location.LastUpdatedTime = DateTimeOffset.UtcNow;
                        changes.Add($"Deleted location: {location.LocationId}");
                    }
                    if (toAdd?.Any() == true)
                        await _unitOfWork.TripPlanLocationRepository.AddRangeAsync(toAdd);
                    foreach (var location in toAdd)
                        changes.Add($"Added location: {location.LocationId}");

                    foreach (var dto in toUpdate)
                    {
                        var location = existingLocations.First(l => l.Id == dto.TripPlanLocationId.Value);
                        location.LocationId = dto.LocationId;
                        location.Order = dto.Order;
                        location.StartTime = dto.StartTime;
                        location.EndTime = dto.EndTime;
                        location.Notes = dto.Notes;
                        location.TravelTimeFromPrev = dto.TravelTimeFromPrev;
                        location.DistanceFromPrev = dto.DistanceFromPrev;
                        location.EstimatedStartTime = dto.EstimatedStartTime;
                        location.EstimatedEndTime = dto.EstimatedEndTime;
                        location.LastUpdatedTime = DateTimeOffset.UtcNow;
                        changes.Add($"Updated location: {dto.LocationId}");
                    }

                    var remainingLocationsCount = tripPlan.TripPlanLocations.Count(l => !l.IsDeleted)
                           + (toAdd?.Count ?? 0)
                           - toDelete.Count;

                    if (remainingLocationsCount <= 0)
                    {
                        tripPlan.Status = TripPlanStatus.Draft;
                    }
                    else
                    {
                        tripPlan.Status = TripPlanStatus.Sketch;
                    }

                    await _unitOfWork.SaveAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var result = new List<TripPlanLocationResponseDto>();
            foreach (var location in existingLocations.Where(l => !l.IsDeleted))
            {
                var imageUrl = await GetLocationImageUrl(location.LocationId);
                result.Add(new TripPlanLocationResponseDto
                {
                    TripPlanLocationId = location.Id,
                    LocationId = location.LocationId,
                    Order = location.Order,
                    StartTime = location.StartTime,
                    EndTime = location.EndTime,
                    Notes = location.Notes,
                    TravelTimeFromPrev = location.TravelTimeFromPrev,
                    DistanceFromPrev = location.DistanceFromPrev,
                    EstimatedStartTime = location.EstimatedStartTime,
                    EstimatedEndTime = location.EstimatedEndTime,
                    ImageUrl = imageUrl
                });
            }

            foreach (var location in toAdd)
            {
                var imageUrl = await GetLocationImageUrl(location.LocationId);
                result.Add(new TripPlanLocationResponseDto
                {
                    TripPlanLocationId = location.Id,
                    LocationId = location.LocationId,
                    Order = location.Order,
                    StartTime = location.StartTime,
                    EndTime = location.EndTime,
                    Notes = location.Notes,
                    TravelTimeFromPrev = location.TravelTimeFromPrev,
                    DistanceFromPrev = location.DistanceFromPrev,
                    EstimatedStartTime = location.EstimatedStartTime,
                    EstimatedEndTime = location.EstimatedEndTime,
                    ImageUrl = imageUrl
                });
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

    // private async Task UpdateTripPlanCuisinesAsync(
    //     TripPlanVersion tripPlanVersion,
    //     List<TripPlanCuisineModel> cuisineModels)
    // {
    //     var existingIds = cuisineModels
    //         .Where(c => c.Id.HasValue)
    //         .Select(c => c.Id.Value)
    //         .ToList();

    //     if (existingIds.Count != 0)
    //     {
    //         // Loại bỏ các món không còn tồn tại trong input
    //         var cuisinesToRemove = tripPlanVersion.TripPlanCuisines
    //             .Where(c => !existingIds.Contains(c.Id))
    //             .ToList();

    //         _unitOfWork.TripPlanCuisineRepository.RemoveRange(cuisinesToRemove);
    //     }

    //     foreach (var cuisineModel in cuisineModels)
    //     {
    //         if (cuisineModel.Id.HasValue)
    //         {
    //             var existingCuisine = tripPlanVersion.TripPlanCuisines
    //                 .FirstOrDefault(c => c.Id == cuisineModel.Id.Value);

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
    //                 TripPlanVersionId = tripPlanVersion.Id,
    //                 CuisineId = cuisineModel.CuisineId,
    //                 StartTime = cuisineModel.StartTime ?? DateTime.Now,
    //                 EndTime = cuisineModel.EndTime ?? DateTime.Now,
    //                 Notes = cuisineModel.Notes
    //             };

    //             await _unitOfWork.TripPlanCuisineRepository.AddAsync(newCuisine);
    //         }
    //     }
    // }

    private async Task UpdateTripPlanLocationsAsync(
        TripPlan tripPlan,
        List<TripPlanLocationModel> locationModels)
    {
        var existingIds = locationModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id!.Value)
            .ToList();

        if (existingIds.Count != 0)
        {
            // Loại bỏ các món không còn tồn tại trong input
            var locationsToRemove = (tripPlan.TripPlanLocations ?? new List<TripPlanLocation>())
                .Where(c => !existingIds.Contains(c.Id))
                .ToList();

            _unitOfWork.TripPlanLocationRepository.RemoveRange(locationsToRemove);
        }

        foreach (var locationModel in locationModels)
        {
            if (locationModel.Id.HasValue)
            {
                var existingLocation = (tripPlan.TripPlanLocations ?? new List<TripPlanLocation>())
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

    public async Task<TripPlanResponseDto> AddTripPlanAsync(TripPlanCreateModel tripPlanCreateModel, CancellationToken cancellationToken)
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

            return _mapper.Map<TripPlanResponseDto>(result);
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

    #region Helper Methods

    public class TripPlanItemSchedule
    {
        public string? ItemType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid? ItemId { get; set; }
    }

    private List<TripPlanItemSchedule> CollectAllSchedules(TripPlanUpdateModel tripPlanUpdateModel)
    {
        var schedules = new List<TripPlanItemSchedule>();

        foreach (var location in tripPlanUpdateModel.Locations ?? new List<TripPlanLocationModel>())
        {
            if (location.StartTime.HasValue && location.EndTime.HasValue)
            {
                schedules.Add(new TripPlanItemSchedule
                {
                    ItemType = "Location",
                    StartTime = location.StartTime.Value,
                    EndTime = location.EndTime.Value,
                    ItemId = location.Id
                });
            }
            // Optionally, handle else case (e.g., log or throw if required)
        }

        // foreach (var cuisine in tripPlanUpdateModel.Cuisines ?? new List<TripPlanCuisineModel>())
        // {
        //     schedules.Add(new TripPlanItemSchedule
        //     {
        //         ItemType = "Cuisine",
        //         StartTime = cuisine.StartTime.Value,
        //         EndTime = cuisine.EndTime.Value,
        //         ItemId = cuisine.Id
        //     });
        // }

        // foreach (var craftVillage in tripPlanUpdateModel.CraftVillages ?? new List<TripPlanCraftVillageModel>())
        // {
        //     schedules.Add(new TripPlanItemSchedule
        //     {
        //         ItemType = "CraftVillage",
        //         StartTime = craftVillage.StartTime.Value,
        //         EndTime = craftVillage.EndTime.Value,
        //         ItemId = craftVillage.Id
        //     });
        // }

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
            throw CustomExceptionFactory.CreateBadRequestError("Lịch trình nằm ngoài khoảng thời gian của kế hoạch chuyến đi");
        }

        var timeConflictValidation = ValidateTimeConflicts(allSchedules);
        if (!timeConflictValidation)
        {
            result = false; // Có sự trùng lặp thời gian trong các schedule
            throw CustomExceptionFactory.CreateBadRequestError("Lịch trình có sự trùng lặp thời gian");
        }

        var logicalValidation = ValidateLogicalTimeSequences(allSchedules);
        if (!logicalValidation)
        {
            result = false; // Có sự không hợp lý trong trình tự thời gian
            throw CustomExceptionFactory.CreateBadRequestError("Lịch trình có trình tự thời gian không hợp lý");
        }

        return result;
    }

    public async Task<bool> UpdateTripPlanImageUrlAsync(Guid tripPlanId, string imageUrl, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Image URL cannot be empty.");
            }

            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == tripPlanId, cancellationToken);

            if (tripPlan == null)
            {
                return false;
            }

            tripPlan.ImageUrl = imageUrl;
            tripPlan.LastUpdatedTime = DateTime.UtcNow;

            try
            {
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
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

    // private async Task UpdateTripPlanCraftVillagesAsync(
    //     TripPlanVersion tripPlanVersion,
    //     List<TripPlanCraftVillageModel> craftVillageModels)
    // {
    //     var existingIds = craftVillageModels
    //         .Where(c => c.Id.HasValue)
    //         .Select(c => c.Id.Value)
    //         .ToList();

    //     if (existingIds.Count != 0)
    //     {
    //         // Loại bỏ các món không còn tồn tại trong input
    //         var craftVillagesToRemove = tripPlanVersion.TripPlanCraftVillages
    //             .Where(c => !existingIds.Contains(c.Id))
    //             .ToList();

    //         _unitOfWork.TripPlanCraftVillageRepository.RemoveRange(craftVillagesToRemove);
    //     }

    //     foreach (var craftVillageModel in craftVillageModels)
    //     {
    //         if (craftVillageModel.Id.HasValue)
    //         {
    //             var existingCraftVillage = tripPlanVersion.TripPlanCraftVillages
    //                 .FirstOrDefault(c => c.Id == craftVillageModel.Id.Value);

    //             if (existingCraftVillage != null)
    //             {
    //                 existingCraftVillage.CraftVillageId = craftVillageModel.CraftVillageId;
    //                 existingCraftVillage.StartTime = craftVillageModel.StartTime ?? DateTime.Now;
    //                 existingCraftVillage.EndTime = craftVillageModel.EndTime ?? DateTime.Now;
    //                 existingCraftVillage.Notes = craftVillageModel.Notes;
    //             }
    //         }
    //         else
    //         {
    //             var newCraftVillage = new TripPlanCraftVillage
    //             {
    //                 TripPlanVersionId = tripPlanVersion.Id,
    //                 CraftVillageId = craftVillageModel.CraftVillageId,
    //                 StartTime = craftVillageModel.StartTime ?? DateTime.Now,
    //                 EndTime = craftVillageModel.EndTime ?? DateTime.Now,
    //                 Notes = craftVillageModel.Notes
    //             };

    //             await _unitOfWork.TripPlanCraftVillageRepository.AddAsync(newCraftVillage);
    //         }
    //     }
    // }

    #endregion

}

public class TripPlanVersionDto
{
    public Guid Id { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public List<TripPlanLocationDto>? TripPlanLocations { get; set; }
}

public class TripPlanLocationDto
{
    public Guid LocationId { get; set; }
    public string? Notes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Order { get; set; }
}
