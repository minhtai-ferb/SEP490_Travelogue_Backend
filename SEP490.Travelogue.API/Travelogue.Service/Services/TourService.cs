using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourService
{
    /// <summary>
    /// Lấy chi tiết kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch chuyến đi.</param>
    Task<TourDetailResponse?> GetTourByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy toàn bộ danh sách kế hoạch chuyến đi.
    /// </summary>
    Task<List<TourDataModel>> GetAllToursAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Thêm mới một kế hoạch chuyến đi.
    /// </summary>
    /// <param name="tourCreateModel">Dữ liệu đầu vào để tạo kế hoạch.</param>
    Task<TourDataModel> AddTourAsync(TourCreateModel tourCreateModel, CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch cần cập nhật.</param>
    /// <param name="tourUpdateModel">Dữ liệu cập nhật.</param>
    Task<TourDataModel?> UpdateTourAsync(Guid id, TourUpdateModel tourUpdateModel, CancellationToken cancellationToken);

    /// <summary>
    /// Xóa kế hoạch chuyến đi theo ID.
    /// </summary>
    /// <param name="id">ID của kế hoạch cần xóa.</param>
    Task DeleteTourAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy danh sách kế hoạch chuyến đi có phân trang và tìm kiếm theo tiêu đề.
    /// </summary>
    /// <param name="title">Tiêu đề kế hoạch (tùy chọn, để tìm kiếm).</param>
    /// <param name="pageNumber">Trang hiện tại.</param>
    /// <param name="pageSize">Số lượng phần tử mỗi trang.</param>
    Task<PagedResult<TourDataModel>> GetPagedTourWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TourService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task DeleteTourAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var checkRole = _userContextService.HasRole(AppRole.ADMIN, AppRole.MODERATOR);

            var tour = _unitOfWork.TourRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == id);
            if (tour == null || tour.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }
            tour.IsDeleted = true;
            _unitOfWork.TourRepository.Update(tour);
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

    public Task<List<TourDataModel>> GetAllToursAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<TourDataModel>> GetPagedTourWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var pagedResult = await _unitOfWork.TourRepository.GetPageWithSearchAsync(title, pageNumber, pageSize, cancellationToken);

            var newsDataModels = _mapper.Map<List<TourDataModel>>(pagedResult.Items);

            // foreach (var tour in newsDataModels)
            // {
            //     tour.OwnerName = await _unitOfWork.UserRepository.GetUserNameByIdAsync(tour.UserId) ?? string.Empty;
            // }

            var result = new PagedResult<TourDataModel>
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

    public async Task<TourDetailResponse?> GetTourByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (tour == null || tour.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour");
            }

            var activities = await GetAllActivities(id);

            var result = new TourDetailResponse
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                TotalDays = tour.TotalDays,
                Days = BuildDaySchedule(tour.TotalDays, activities)
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

    public async Task<TourDataModel?> UpdateTourAsync(
        Guid id,
        TourUpdateModel tourUpdateModel,
        CancellationToken cancellationToken)
    {
        var validationResult = ValidateTourSchedule(tourUpdateModel);
        if (!validationResult)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Lỗi validation lịch trình:");
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                    .Include(v => v.TourPlanVersions)
                        .ThenInclude(v => v.TourPlanLocations)
                    .Include(v => v.TourPlanVersions)
                        .ThenInclude(v => v.Orders)
                .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour plan");

            var currentUserId = _userContextService.GetCurrentUserId();
            var userRoles = await _userContextService.GetCurrentUserRolesAsync();

            var latestVersion = tour.TourPlanVersions
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefault();

            bool shouldCreateNewVersion = latestVersion == null || latestVersion.Orders.Any();
            TourPlanVersion? newVersion = null;

            if (shouldCreateNewVersion)
            {
                newVersion = new TourPlanVersion
                {
                    Price = tourUpdateModel.Price,
                    TourId = tour.Id,
                    VersionDate = _timeService.SystemTimeNow,
                    Description = "Cập nhật lịch trình",
                    VersionNumber = latestVersion?.VersionNumber + 1 ?? 1,
                    // Status = "Draft",
                    // IsFromTourGuide = isTourGuide
                };

                await _unitOfWork.TourPlanVersionRepository.AddAsync(newVersion);
                await _unitOfWork.SaveAsync();

                // Update vào version con
                if (tourUpdateModel.Locations == null)
                {
                    tourUpdateModel.Locations = new List<TourPlanLocationModel>();
                }

                await UpdateTourPlanLocationsAsync(newVersion, tourUpdateModel.Locations);
            }
            else
            {
                newVersion = latestVersion;
                newVersion!.VersionDate = _timeService.SystemTimeNow;
                newVersion.Description = "Cập nhật lịch trình";
            }


            _mapper.Map(tourUpdateModel, tour);

            var tourVersion = tour.TourPlanVersions
                .FirstOrDefault(v => v.Id == newVersion.Id)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour version");

            tourVersion.TourPlanLocations = _mapper.Map<List<TourPlanLocation>>(tourUpdateModel.Locations);

            tour.CurrentVersionId = newVersion.Id;

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _mapper.Map<TourDataModel>(tour);

            result.CurrentVersionId = newVersion != null ? newVersion.Id : (tour.CurrentVersionId ?? Guid.Empty);
            // result.IsFromTourGuide = isTourGuide;

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

    public async Task<TourDataModel> AddTourAsync(TourCreateModel tourCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newTour = _mapper.Map<Tour>(tourCreateModel);

            newTour.CreatedBy = currentUserId;
            newTour.LastUpdatedBy = currentUserId;
            newTour.CreatedTime = currentTime;
            newTour.LastUpdatedTime = currentTime;

            await _unitOfWork.TourRepository.AddAsync(newTour);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.TourRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == newTour.Id);

            return _mapper.Map<TourDataModel>(result);
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

    public class TourItemScheduleDto
    {
        public string ItemType { get; set; }
        public int DayOrder { get; set; } = 1;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Guid? ItemId { get; set; }
    }

    private List<TourItemScheduleDto> CollectAllSchedules(TourUpdateModel tourUpdateModel)
    {
        var schedules = new List<TourItemScheduleDto>();

        foreach (var location in tourUpdateModel.Locations ?? new List<TourPlanLocationModel>())
        {
            if (location.StartTime.HasValue && location.EndTime.HasValue)
            {
                schedules.Add(new TourItemScheduleDto
                {
                    ItemType = "Location",
                    DayOrder = location.DayOrder,
                    StartTime = location.StartTime.Value,
                    EndTime = location.EndTime.Value,
                    ItemId = location.Id
                });
            }
            // Optionally, handle else case (e.g., log or throw if required)
        }

        return schedules.OrderBy(s => s.DayOrder).ToList();
    }

    private bool ValidateDateRanges(int totalDays, List<TourItemScheduleDto> schedules)
    {
        var result = true;

        foreach (var schedule in schedules)
        {
            // if (schedule.StartTime.Date < tripStartDate.Date)
            // {
            //     result = false; // Thời gian bắt đầu của schedule nằm trước ngày bắt đầu của Tour
            //     break;
            // }

            // if (schedule.EndTime.Date > tripEndDate.Date)
            // {
            //     result = false; // Thời gian kết thúc của schedule nằm sau ngày kết thúc của Tour
            //     break;
            // }

            if (schedule.DayOrder < 1 || schedule.DayOrder > totalDays)
            {
                result = false; // Ngày trong schedule không hợp lệ
                break;
            }

            // if (schedule.StartTime < tripStartDate || schedule.EndTime > tripEndDate)
            // {
            //     result = false; // Thời gian của schedule nằm ngoài khoảng thời gian của Tour
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

    private bool ValidateTimeConflicts(List<TourItemScheduleDto> schedules)
    {
        if (schedules.Count == 0) return true;

        var result = true;

        var sortedSchedules = schedules.OrderBy(s => s.DayOrder).ToList();

        for (int i = 0; i < sortedSchedules.Count - 1; i++)
        {
            var current = sortedSchedules[i];
            var next = sortedSchedules[i + 1];

            var bufferMinutes = 15;
            var bufferTime = TimeSpan.FromMinutes(bufferMinutes);

            if (current.EndTime.Add(-bufferTime) > next.StartTime
                && current.DayOrder == next.DayOrder)
            {
                result = false; // Có sự trùng lặp thời gian
                break;
            }
        }

        return result;
    }

    private bool ValidateLogicalTimeSequences(List<TourItemScheduleDto> schedules)
    {
        if (schedules.Count == 0) return true;

        var result = true;

        var dailySchedules = schedules
            .GroupBy(s => s.DayOrder)
            .ToList();

        foreach (var dailySchedule in dailySchedules)
        {
            var daySchedules = dailySchedule.OrderBy(s => s.StartTime).ToList();

            foreach (var schedule in daySchedules)
            {
                if (schedule.StartTime.Hours < 6)
                {
                    result = false; // Không hợp lý nếu bắt đầu quá sớm
                }

                if (schedule.EndTime.Hours > 23)
                {
                    result = false; // Không hợp lý nếu kết thúc quá muộn
                }
            }

            if (!result) break;
        }

        return result;
    }

    private bool ValidateTourSchedule(TourUpdateModel tourUpdateModel)
    {
        var result = true;

        // if (tourUpdateModel.StartDate >= tourUpdateModel.EndDate)
        // {
        //     result = false; // Ngày bắt đầu phải trước ngày kết thúc
        // }

        var allSchedules = CollectAllSchedules(tourUpdateModel);

        var dateRangeValidation = ValidateDateRanges(tourUpdateModel.TotalDays, allSchedules);
        if (!dateRangeValidation)
        {
            result = false; // Có schedule nằm ngoài khoảng thời gian của Tour
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

    private async Task<List<TourActivity>> GetAllActivities(Guid tourId)
    {
        try
        {

            var activities = new List<TourActivity>();

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(x => x.TourPlanVersions)
                    .ThenInclude(x => x.TourPlanLocations)
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (tour == null || tour.CurrentVersionId == null)
                throw CustomExceptionFactory.CreateNotFoundError("Tour hoặc version");

            var version = tour.TourPlanVersions.FirstOrDefault(v => v.Id == tour.CurrentVersionId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour version");

            // Locations
            foreach (var tpl in version.TourPlanLocations ?? new List<TourPlanLocation>())
            {
                var location = await _unitOfWork.LocationRepository.GetByIdAsync(tpl.LocationId, cancellationToken: CancellationToken.None);
                if (location == null) continue;

                activities.Add(new TourActivity
                {
                    Id = tpl.Id,
                    Type = TripActivityTypeEnum.Location.ToString(),
                    DayOrder = tpl.DayOrder,
                    Name = location.Name,
                    Description = location.Description,
                    Address = location.Address ?? string.Empty,
                    StartTime = tpl.StartTime,
                    EndTime = tpl.EndTime,
                    StartTimeFormatted = tpl.StartTime.ToString(@"hh\:mm"),
                    EndTimeFormatted = tpl.EndTime.ToString(@"hh\:mm"),
                    Duration = $"{(tpl.EndTime - tpl.StartTime).TotalMinutes} phút",
                    // Notes = tpl.Notes,
                    ImageUrl = string.Empty
                });
            }

            return activities.OrderBy(x => x.StartTime).ToList();
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

    private async Task<string> GetLocationImageUrl(Guid locationId)
    {
        // Note: Nếu có isThumbnail thì lấy ảnh thumbnail, nếu không thì lấy ảnh đầu tiên
        var locationMedia = await _unitOfWork.LocationMediaRepository.ActiveEntities
            .FirstOrDefaultAsync(l => l.LocationId == locationId);

        return locationMedia.MediaUrl ?? string.Empty;
    }

    private List<TourDayDetail> BuildDaySchedule(int totalDays, List<TourActivity> activities)
    {
        var days = new List<TourDayDetail>();
        // var currentDate = startDate.Date;
        var dayNumber = 1;

        while (dayNumber <= totalDays)
        {
            var dayActivities = activities
                .Where(a => a.DayOrder == dayNumber)
                .ToList();

            dayActivities = SortActivitiesByTime(dayActivities);

            days.Add(new TourDayDetail
            {
                DayNumber = dayNumber,
                Activities = dayActivities
            });
            dayNumber++;
        }

        return days;
    }

    private List<TourActivity> SortActivitiesByTime(List<TourActivity> activities)
    {
        return activities.OrderBy(a =>
        {
            // if (a.Order.HasValue)
            //     return a.Order.Value;

            if (a.StartTime.HasValue)
                return a.StartTime.Value.Hours * 60 + a.StartTime.Value.Minutes;

            return int.MaxValue;
        })
        .ThenBy(a => a.StartTime ?? TimeSpan.MaxValue)
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

    private async Task UpdateTourPlanLocationsAsync(
        TourPlanVersion tourVersion,
        List<TourPlanLocationModel> locationModels)
    {
        var existingIds = locationModels
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id.Value)
            .ToList();

        if (existingIds.Count != 0)
        {
            // Loại bỏ các location không còn tồn tại trong input
            var locationsToRemove = tourVersion.TourPlanLocations
                .Where(c => !existingIds.Contains(c.Id))
                .ToList();

            _unitOfWork.TourPlanLocationRepository.RemoveRange(locationsToRemove);
        }

        foreach (var locationModel in locationModels)
        {
            if (locationModel.Id.HasValue)
            {
                var existingLocation = tourVersion.TourPlanLocations
                    .FirstOrDefault(c => c.Id == locationModel.Id.Value);

                if (existingLocation != null)
                {
                    existingLocation.LocationId = locationModel.LocationId;
                    existingLocation.StartTime = locationModel.StartTime ?? TimeSpan.Zero;
                    existingLocation.EndTime = locationModel.EndTime ?? TimeSpan.Zero;
                    existingLocation.DayOrder = locationModel.DayOrder;

                    // existingLocation.Notes = locationModel.Notes;
                }
            }
            else
            {
                var newLocation = new TourPlanLocation
                {
                    TourPlanVersionId = tourVersion.Id,
                    LocationId = locationModel.LocationId,
                    StartTime = locationModel.StartTime ?? TimeSpan.Zero,
                    EndTime = locationModel.EndTime ?? TimeSpan.Zero,
                    DayOrder = locationModel.DayOrder,
                    // Notes = locationModel.Notes
                };

                await _unitOfWork.TourPlanLocationRepository.AddAsync(newLocation);
            }
        }
    }
    #endregion

}

public class TourVersionDto
{
    public Guid Id { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public List<TourPlanLocationDto>? TourPlanLocations { get; set; }
}

public class TourPlanLocationDto
{
    public Guid LocationId { get; set; }
    public string? Notes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Order { get; set; }
}
