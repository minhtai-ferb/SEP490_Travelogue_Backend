using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ExchangeSessionModels;
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
    Task<List<TripPlanDataModel>> GetAllTripPlansAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Thêm mới một kế hoạch chuyến đi.
    /// </summary>
    /// <param name="tripPlanCreateModel">Dữ liệu đầu vào để tạo kế hoạch.</param>
    Task<TripPlanDataModel> AddTripPlanAsync(TripPlanCreateModel tripPlanCreateModel, CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch cần cập nhật.</param>
    /// <param name="tripPlanUpdateModel">Dữ liệu cập nhật.</param>
    Task<TripPlanDataModel?> UpdateTripPlanAsync(Guid id, TripPlanUpdateModel tripPlanUpdateModel, CancellationToken cancellationToken);

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
    Task<PagedResult<TripPlanDataModel>> GetPagedTripPlanWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Tạo một phiên bản mới từ TripPlan hiện tại (copy) khi người dùng gửi yêu cầu đặt lịch hướng dẫn viên.
    /// Hành động này thường xảy ra khi hướng dẫn viên muốn đề xuất phiên bản khác với lịch trình hiện tại.
    /// </summary>
    /// <param name="tripPlanId">ID của kế hoạch chuyến đi gốc.</param>
    /// <param name="guideNote">Ghi chú từ hướng dẫn viên.</param>
    Task<object> CreateVersionFromTripPlanAsync(Guid tripPlanId, string guideNote);

    Task<ExchangeSessionDataModel> CreateExchangeSessionAsync(Guid tripPlanId, CreateExchangeSessionRequest request, CancellationToken cancellationToken);

    Task<ExchangeSessionDataModel> CreateExchangeSessionWithNewVersionAsync(
            Guid tripPlanId, CreateExchangeSessionRequest request, CancellationToken cancellationToken);
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

    public async Task<ExchangeSessionDataModel> CreateExchangeSessionAsync(Guid tripPlanId, CreateExchangeSessionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIdAsync(request.TourGuideId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour guide");

            var tripPlan = await _unitOfWork.TripPlanRepository.GetByIdAsync(tripPlanId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var tripPlanVersion = await _unitOfWork.TripPlanVersionRepository.GetByIdAsync(request.TripPlanVersionId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");


            var newSession = new TripPlanExchangeSession
            {
                // TourGuideId = tourGuide.Id,
                TourGuideId = request.TourGuideId,
                // TripPlanId = tripPlan.Id,
                TripPlanId = tripPlanId,
                CreatedByUserId = Guid.Parse(currentUserId),
                FinalStatus = ExchangeSessionStatus.Pending,
                CreatedBy = currentUserId,
                CreatedAt = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime
            };

            newSession.Exchanges.Add(new TripPlanExchange
            {
                UserId = Guid.Parse(currentUserId),
                TripPlanId = tripPlan.Id,
                TripPlanVersionId = tripPlanVersion.Id,
                TourGuideId = tourGuide.Id,
                SessionId = newSession.Id,
                Status = ExchangeSessionStatus.Pending,
                RequestedAt = currentTime,
                CreatedBy = currentUserId,
                CreatedTime = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime
            });

            var result = _mapper.Map<ExchangeSessionDataModel>(newSession);

            await _unitOfWork.TripPlanExchangeSessionRepository.AddAsync(newSession);
            await _unitOfWork.SaveAsync();

            return result;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-----------------Error creating exchange session: {ex.Message}");
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<ExchangeSessionDataModel> CreateExchangeSessionWithNewVersionAsync(
        Guid tripPlanId, CreateExchangeSessionRequest request, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            // Lấy TripPlan và TourGuide
            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIdAsync(request.TourGuideId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour guide");
            var tripPlan = await _unitOfWork.TripPlanRepository.GetWithIncludeAsync(
                tripPlanId,
                include => include.Include(tp => tp.TripPlanVersions)
                                  .ThenInclude(v => v.TripPlanLocations)
            ) ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var latestVersion = tripPlan.TripPlanVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault()
                ?? throw CustomExceptionFactory.CreateNotFoundError("No versions found for this trip plan");

            // Tạo TripPlanVersion mới
            var newVersion = new TripPlanVersion
            {
                TripPlanId = tripPlanId,
                Notes = request.GuideNote,
                CreatedTime = currentTime,
                VersionDate = currentTime,
                VersionNumber = latestVersion.VersionNumber + 1,
                Status = "Draft",
                TripPlanLocations = (latestVersion.TripPlanLocations ?? new List<TripPlanLocation>())
                    .Select(loc => new TripPlanLocation
                    {
                        LocationId = loc.LocationId,
                        StartTime = loc.StartTime,
                        EndTime = loc.EndTime,
                        Notes = loc.Notes,
                        Order = loc.Order
                    }).ToList()
            };

            await _unitOfWork.TripPlanVersionRepository.AddAsync(newVersion);

            // Tạo Exchange Session
            var newSession = new TripPlanExchangeSession
            {
                TourGuideId = tourGuide.Id,
                TripPlanId = tripPlan.Id,
                CreatedByUserId = Guid.Parse(currentUserId),
                FinalStatus = ExchangeSessionStatus.Pending,
                CreatedBy = currentUserId,
                CreatedAt = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime,
                Exchanges = new List<TripPlanExchange>
            {
                new TripPlanExchange
                {
                    UserId = Guid.Parse(currentUserId),
                    TripPlanId = tripPlan.Id,
                    TripPlanVersionId = newVersion.Id,
                    TourGuideId = tourGuide.Id,
                    SessionId = Guid.Empty,
                    Status = ExchangeSessionStatus.Pending,
                    RequestedAt = currentTime,
                    CreatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedBy = currentUserId,
                    LastUpdatedTime = currentTime
                }
            }
            };

            await _unitOfWork.TripPlanExchangeSessionRepository.AddAsync(newSession);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            var result = _mapper.Map<ExchangeSessionDataModel>(newSession);
            result.TripPlanVersionId = newVersion.Id;

            return _mapper.Map<ExchangeSessionDataModel>(newSession);
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


    public async Task DeleteTripPlanAsync(Guid id, CancellationToken cancellationToken)
    {
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

    public Task<List<TripPlanDataModel>> GetAllTripPlansAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<TripPlanDataModel>> GetPagedTripPlanWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var pagedResult = await _unitOfWork.TripPlanRepository.GetPageWithSearchAsync(title, pageNumber, pageSize, cancellationToken);

            var newsDataModels = _mapper.Map<List<TripPlanDataModel>>(pagedResult.Items);

            foreach (var tripPlan in newsDataModels)
            {
                tripPlan.OwnerName = await _unitOfWork.UserRepository.GetUserNameByIdAsync(tripPlan.UserId) ?? string.Empty;
            }

            var result = new PagedResult<TripPlanDataModel>
            {
                Items = newsDataModels,
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
            .Include(x => x.TripPlanVersions)
                .ThenInclude(x => x.TripPlanLocations)
            .FirstOrDefaultAsync(x => x.Id == tripPlanId);

        if (tripPlan == null || tripPlan.UserTripPlanVersionId == null)
            throw CustomExceptionFactory.CreateNotFoundError("Trip plan hoặc version");

        var version = tripPlan.TripPlanVersions.FirstOrDefault(v => v.Id == tripPlan.UserTripPlanVersionId)
            ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");

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
    //     catch (Exception)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError();
    //     }
    //     finally
    //     {
    //         ////  _unitOfWork.Dispose();
    //     }
    // }

    public async Task<TripPlanDataModel?> UpdateTripPlanAsync(
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
                .Include(tp => tp.TripPlanVersions)
                    .ThenInclude(v => v.TripPlanLocations)
                .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var currentUserId = _userContextService.GetCurrentUserId();
            var userRoles = await _userContextService.GetCurrentUserRolesAsync();
            var isTourGuide = userRoles.Contains(AppRole.TOUR_GUIDE);

            bool shouldCreateNewVersion = isTourGuide || !tripPlan.TripPlanVersions.Any();

            TripPlanVersion? newVersion = null;

            if (shouldCreateNewVersion)
            {
                newVersion = new TripPlanVersion
                {
                    TripPlanId = tripPlan.Id,
                    VersionDate = _timeService.SystemTimeNow,
                    Description = "Cập nhật lịch trình",
                    VersionNumber = tripPlan.TripPlanVersions.Count + 1,
                    Status = "Draft",
                    IsFromTourGuide = isTourGuide
                };

                await _unitOfWork.TripPlanVersionRepository.AddAsync(newVersion);
                await _unitOfWork.SaveAsync();

                // Update vào version con
                if (tripPlanUpdateModel.Locations == null)
                {
                    tripPlanUpdateModel.Locations = new List<TripPlanLocationModel>();
                }

                await UpdateTripPlanLocationsAsync(newVersion, tripPlanUpdateModel.Locations);

                // Gán lại version mới cho user nếu là user tạo
                if (!isTourGuide)
                {
                    tripPlan.UserTripPlanVersionId = newVersion.Id;
                }
            }

            _mapper.Map(tripPlanUpdateModel, tripPlan);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _mapper.Map<TripPlanDataModel>(tripPlan);

            result.TripPlanVersionId = newVersion != null ? newVersion.Id : (tripPlan.UserTripPlanVersionId ?? Guid.Empty);
            result.IsFromTourGuide = isTourGuide;

            return result;
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
        TripPlanVersion tripPlanVersion,
        List<TripPlanLocationModel> locationModels)
    {
        var existingIds = locationModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id.Value)
            .ToList();

        if (existingIds.Count != 0)
        {
            // Loại bỏ các món không còn tồn tại trong input
            var locationsToRemove = tripPlanVersion.TripPlanLocations
                .Where(c => !existingIds.Contains(c.Id))
                .ToList();

            _unitOfWork.TripPlanLocationRepository.RemoveRange(locationsToRemove);
        }

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
            // _unitOfWork.Dispose();
        }
    }

    #region Helper Methods

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

    public async Task<object> CreateVersionFromTripPlanAsync(Guid tripPlanId, string guideNote)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // var currentUserId = _userContextService.GetCurrentUserId();
            var currentUserId = "example-user-id";
            var currentTime = _timeService.SystemTimeNow;

            // Lấy phiên bản TripPlan hiện tại từ tripPlanId
            var tripPlan = await _unitOfWork.TripPlanRepository
                .GetWithIncludeAsync(tripPlanId, include => include.Include(tp => tp.TripPlanVersions)
                    .ThenInclude(v => v.TripPlanLocations)) ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan not found");
            var version = tripPlan.TripPlanVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault() ?? throw CustomExceptionFactory.CreateNotFoundError("No versions found for this trip plan");

            // Tạo phiên bản mới từ TripPlan hiện tại
            var newVersion = new TripPlanVersion
            {
                TripPlanId = version.TripPlanId,
                Notes = guideNote,
                CreatedTime = currentTime,
                VersionDate = currentTime,
                VersionNumber = version.VersionNumber + 1,
                Status = "Draft"
            };

            // Sao chép các địa điểm, món ăn, làng nghề từ phiên bản cũ sang phiên bản mới
            newVersion.TripPlanLocations = (version.TripPlanLocations ?? new List<TripPlanLocation>())
                .Select(loc => new TripPlanLocation
                {
                    LocationId = loc.LocationId,
                    StartTime = loc.StartTime,
                    EndTime = loc.EndTime,
                    Notes = loc.Notes,
                    Order = loc.Order
                }).ToList();

            await _unitOfWork.TripPlanVersionRepository.AddAsync(newVersion);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new { VersionId = newVersion.Id, VersionNumber = newVersion.VersionNumber };
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
