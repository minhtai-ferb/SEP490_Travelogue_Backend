// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using Travelogue.Repository.Bases;
// using Travelogue.Repository.Bases.Exceptions;
// using Travelogue.Repository.Const;
// using Travelogue.Repository.Data;
// using Travelogue.Repository.Entities;
// using Travelogue.Repository.Entities.Enums;
// using Travelogue.Service.BusinessModels.TourGuideModels;
// using Travelogue.Service.BusinessModels.TourModels;
// using Travelogue.Service.Commons.Interfaces;

// namespace Travelogue.Service.Services;

// public interface ITourService1
// {
//     /// <summary>
//     /// Lấy chi tiết kế hoạch chuyến đi theo ID.
//     /// </summary>
//     /// <param name="id">ID của kế hoạch chuyến đi.</param>
//     Task<TourDetailResponse?> GetTourByIdAsync(Guid id, CancellationToken cancellationToken);

//     /// <summary>
//     /// Lấy toàn bộ danh sách kế hoạch chuyến đi.
//     /// </summary>
//     Task<List<TourDataModel>> GetAllToursAsync(CancellationToken cancellationToken);

//     /// <summary>
//     /// Thêm mới một kế hoạch chuyến đi.
//     /// </summary>
//     /// <param name="tourCreateModel">Dữ liệu đầu vào để tạo kế hoạch.</param>
//     Task<TourDataModel> AddTourAsync(TourCreateModel tourCreateModel, CancellationToken cancellationToken);

//     /// <summary>
//     /// Cập nhật kế hoạch chuyến đi theo ID.
//     /// </summary>
//     /// <param name="id">ID của kế hoạch cần cập nhật.</param>
//     /// <param name="tourUpdateModel">Dữ liệu cập nhật.</param>
//     // Task<TourDataModel?> UpdateTourAsync(Guid id, TourUpdateModel tourUpdateModel, CancellationToken cancellationToken);

//     /// <summary>
//     /// Xóa kế hoạch chuyến đi theo ID.
//     /// </summary>
//     /// <param name="id">ID của kế hoạch cần xóa.</param>
//     Task DeleteTourAsync(Guid id, CancellationToken cancellationToken);

//     /// <summary>
//     /// Lấy danh sách kế hoạch chuyến đi có phân trang và tìm kiếm theo tiêu đề.
//     /// </summary>
//     /// <param name="title">Tiêu đề kế hoạch (tùy chọn, để tìm kiếm).</param>
//     /// <param name="pageNumber">Trang hiện tại.</param>
//     /// <param name="pageSize">Số lượng phần tử mỗi trang.</param>
//     Task<PagedResult<TourDataModel>> GetPagedTourWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken);
// }

// public class TourService1 : ITourService1
// {
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IMapper _mapper;
//     private readonly IUserContextService _userContextService;
//     private readonly ITimeService _timeService;

//     public TourService1(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
//     {
//         _unitOfWork = unitOfWork;
//         _mapper = mapper;
//         _userContextService = userContextService;
//         _timeService = timeService;
//     }

//     public async Task DeleteTourAsync(Guid id, CancellationToken cancellationToken)
//     {
//         try
//         {
//             Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

//             var checkRole = _userContextService.HasRole(AppRole.ADMIN, AppRole.MODERATOR);

//             var tour = _unitOfWork.TourRepository.ActiveEntities
//                 .FirstOrDefault(tp => tp.Id == id);
//             if (tour == null || tour.IsDeleted)
//             {
//                 throw CustomExceptionFactory.CreateNotFoundError("Tour");
//             }
//             tour.IsDeleted = true;
//             _unitOfWork.TourRepository.Update(tour);
//             await _unitOfWork.SaveAsync();

//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             ////  _unitOfWork.Dispose();
//         }
//     }

//     public Task<List<TourDataModel>> GetAllToursAsync(CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }

//     public async Task<PagedResult<TourDataModel>> GetPagedTourWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var pagedResult = await _unitOfWork.TourRepository.GetPageWithSearchAsync(name, pageNumber, pageSize, cancellationToken);

//             var tourDataModels = _mapper.Map<List<TourDataModel>>(pagedResult.Items);

//             // foreach (var tour in tourDataModels)
//             // {
//             //     tour.OwnerName = await _unitOfWork.UserRepository.GetUserNameByIdAsync(tour.UserId) ?? string.Empty;
//             // }

//             var result = new PagedResult<TourDataModel>
//             {
//                 Items = tourDataModels,
//                 TotalCount = pagedResult.TotalCount,
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             };

//             return result;
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             ////  _unitOfWork.Dispose();
//         }
//     }

//     // public async Task<TourDetailResponse?> GetTourByIdAsync(Guid tourId, Guid scheduleId, CancellationToken cancellationToken)
//     // {
//     //     try
//     //     {
//     //         // Fetch the tour associated with the tourVersionId
//     //         var tour = await _unitOfWork.TourRepository.ActiveEntities
//     //             .FirstOrDefaultAsync(t => t.Id == tourId, cancellationToken);

//     //         if (tour == null)
//     //         {
//     //             throw CustomExceptionFactory.CreateNotFoundError("Tour with specified version");
//     //         }

//     //         // Fetch the specific TourSchedule by scheduleId
//     //         var schedule = await _unitOfWork.TourScheduleRepository
//     //             .ActiveEntities
//     //             .FirstOrDefaultAsync(ts => ts.Id == scheduleId && ts.TourPlanVersionId == tourId, cancellationToken);

//     //         if (schedule == null)
//     //         {
//     //             throw CustomExceptionFactory.CreateNotFoundError("Tour Schedule");
//     //         }

//     //         var activities = await GetAllActivities(tour.Id);

//     //         var result = new TourDetailResponse
//     //         {
//     //             Id = tour.Id,
//     //             Name = tour.Name,
//     //             Description = tour.Description,
//     //             Content = tour.Content,
//     //             TotalDays = tour.TotalDays,
//     //             TotalDaysText = $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
//     //             AdultPrice = schedule.AdultPrice,
//     //             ChildrenPrice = schedule.ChildrenPrice,
//     //             FinalPrice = schedule.AdultPrice, 
//     //             IsDiscount = false, // TODO: Implement discount logic
//     //             TourTypeId = tour.TourTypeId,
//     //             TourTypeText = tour.TourType?.Name ?? string.Empty,
//     //             TourGuide = tour.TourGuideMappings
//     //                 .Select(tgm => new TourGuideDataModel
//     //                 {
//     //                     Id = tgm.GuideId,
//     //                     UserName = tgm.TourGuide?.User.FullName ?? string.Empty,
//     //                     AvatarUrl = tgm.TourGuide?.User.AvatarUrl ?? string.Empty
//     //                 })
//     //                 .FirstOrDefault(),
//     //             Days = BuildDaySchedule(tour.TotalDays, activities)
//     //         };

//     //         return result;
//     //     }
//     //     catch (CustomException)
//     //     {
//     //         throw;
//     //     }
//     //     catch (Exception ex)
//     //     {
//     //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//     //     }
//     //     finally
//     //     {
//     //         //// _unitOfWork.Dispose();
//     //     }
//     // }

//     public async Task<TourDetailResponse?> GetTourByIdAsync(Guid id, Guid scheduleId, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var tour = await _unitOfWork.TourRepository.ActiveEntities
//                 .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//             if (tour == null || tour.IsDeleted)
//             {
//                 throw CustomExceptionFactory.CreateNotFoundError("Tour");
//             }

//             // Fetch the specific TourSchedule by scheduleId
//             var schedule = await _unitOfWork.TourScheduleRepository.ActiveEntities
//                 .FirstOrDefaultAsync(ts => ts.Id == scheduleId, cancellationToken);

//             if (schedule == null)
//             {
//                 throw CustomExceptionFactory.CreateNotFoundError("Tour Schedule");
//             }

//             var activities = await GetAllActivities(id);

//             var result = new TourDetailResponse
//             {
//                 Id = tour.Id,
//                 Name = tour.Name,
//                 Description = tour.Description,
//                 Content = tour.Content,
//                 TotalDays = tour.TotalDays,
//                 TotalDaysText = $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
//                 AdultPrice = schedule.AdultPrice,
//                 ChildrenPrice = schedule.ChildrenPrice,
//                 FinalPrice = schedule.AdultPrice, // Adjust if discount logic applies
//                 IsDiscount = false, // TODO: Implement discount logic
//                 TourTypeId = tour.TourTypeId,
//                 TourTypeText = tour.TourType?.Name ?? string.Empty,
//                 TourGuide = tour.TourGuideMappings
//                     .Select(tgm => new TourGuideDataModel
//                     {
//                         Id = tgm.GuideId,
//                         UserName = tgm.TourGuide?.User.FullName ?? string.Empty,
//                         AvatarUrl = tgm.TourGuide?.User.AvatarUrl ?? string.Empty
//                     })
//                     .FirstOrDefault(),
//                 Days = BuildDaySchedule(tour.TotalDays, activities)
//             };

//             return result;
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             ////  _unitOfWork.Dispose();
//         }
//     }

//     public async Task<TourDetailResponse?> GetTourByIdAsync(Guid id, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var tour = await _unitOfWork.TourRepository.ActiveEntities
//                 .FirstOrDefaultAsync(tp => tp.Id == id);

//             if (tour == null || tour.IsDeleted)
//             {
//                 throw CustomExceptionFactory.CreateNotFoundError("Tour");
//             }

//             var activities = await GetAllActivities(id);

//             var result = new TourDetailResponse
//             {
//                 Id = tour.Id,
//                 Name = tour.Name,
//                 Description = tour.Description,
//                 Content = tour.Content,
//                 TotalDays = tour.TotalDays,
//                 TotalDaysText = $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
//                 // AdultPrice = tour.CurrentVersion?.AdultPrice ?? 0,
//                 // ChildrenPrice = tour.CurrentVersion?.ChildrenPrice ?? 0,
//                 // FinalPrice = tour.CurrentVersion?.AdultPrice ?? 0,
//                 IsDiscount = false, // TODO: Implement discount logic
//                 TourTypeId = tour.TourTypeId,
//                 TourTypeText = tour.TourType?.Name ?? string.Empty,
//                 TourGuide = tour.TourGuideMappings
//                     .Select(tgm => new TourGuideDataModel
//                     {
//                         Id = tgm.GuideId,
//                         UserName = tgm.TourGuide?.User.FullName ?? string.Empty,
//                         AvatarUrl = tgm.TourGuide?.User.AvatarUrl ?? string.Empty
//                     })
//                     .FirstOrDefault(),
//                 Days = BuildDaySchedule(tour.TotalDays, activities)
//             };

//             return result;
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             ////  _unitOfWork.Dispose();
//         }
//     }

//     // public async Task<TourDetailResponse?> GetTourByVersionIdAsync(Guid tourId, CancellationToken cancellationToken)
//     // {
//     //     try
//     //     {
//     //         var tour = await _unitOfWork.TourRepository.ActiveEntities
//     //             .Include(v => v.TourType)
//     //             .Include(v => v.TourPlanLocations)
//     //             .FirstOrDefaultAsync(v => v.Id == tourId);

//     //         if (tour == null || tour.IsDeleted)
//     //         {
//     //             throw CustomExceptionFactory.CreateNotFoundError("Tour");
//     //         }

//     //         var activities = await GetAllActivitiesByVersionId(tourId);

//     //         var minPriceOfTour = await _unitOfWork.TourScheduleRepository.ActiveEntities
//     //             .Where(ts => ts.TourPlanVersionId == tourId)
//     //             .MinAsync(ts => ts.AdultPrice);

//     //         var result = new TourDetailResponse
//     //         {
//     //             Id = tour.Id,
//     //             Name = tour.Name,
//     //             Description = tour.Description,
//     //             Content = tour.Content,
//     //             TotalDays = tour.TotalDays,
//     //             TotalDaysText = $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
//     //             AdultPrice = tour?.TourSchedules.AdultPrice ?? 0,
//     //             ChildrenPrice = tour.CurrentVersion?.ChildrenPrice ?? 0,
//     //             FinalPrice = minPriceOfTour,
//     //             IsDiscount = false, // TODO: Implement discount logic
//     //             TourTypeId = tour.TourTypeId,
//     //             TourTypeText = tour.TourType?.Name ?? string.Empty,
//     //             // CurrentVersionId = tour.CurrentVersionId ?? Guid.Empty,
//     //             TourGuide = tour.TourGuideMappings
//     //                 .Select(tgm => new TourGuideDataModel
//     //                 {
//     //                     Id = tgm.GuideId,
//     //                     UserName = tgm.TourGuide?.User.FullName ?? string.Empty,
//     //                     AvatarUrl = tgm.TourGuide?.User.AvatarUrl ?? string.Empty
//     //                 })
//     //                 .FirstOrDefault(),
//     //             Days = BuildDaySchedule(tour.TotalDays, activities)
//     //         };

//     //         return result;
//     //     }
//     //     catch (CustomException)
//     //     {
//     //         throw;
//     //     }
//     //     catch (Exception ex)
//     //     {
//     //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//     //     }
//     //     finally
//     //     {
//     //         ////  _unitOfWork.Dispose();
//     //     }
//     // }

//     // public async Task<TourDataModel?> UpdateTourAsync(
//     //     Guid id,
//     //     TourUpdateModel tourUpdateModel,
//     //     CancellationToken cancellationToken)
//     // {
//     //     var validationResult = ValidateTourSchedule(tourUpdateModel);
//     //     if (!validationResult)
//     //         throw CustomExceptionFactory.CreateBadRequestError("Lỗi validation lịch trình");

//     //     using var transaction = await _unitOfWork.BeginTransactionAsync();

//     //     try
//     //     {
//     //         var tour = await _unitOfWork.TourRepository
//     //             .ActiveEntities
//     //             .Include(t => t.Bookings)
//     //             .Include(t => t.TourPlanLocations)
//     //             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
//     //             ?? throw CustomExceptionFactory.CreateNotFoundError("Tour không tồn tại");

//     //         if (shouldCreateNewVersion)
//     //         {
//     //             newVersion = new TourPlanVersion
//     //             {
//     //                 TourId = tour.Id,
//     //                 Description = "Cập nhật lịch trình",
//     //                 VersionDate = _timeService.SystemTimeNow,
//     //                 VersionNumber = (latestVersion?.VersionNumber ?? 0) + 1
//     //             };

//     //             if (tourUpdateModel.Locations != null)
//     //             {
//     //                 newVersion.TourPlanLocations = _mapper.Map<List<TourPlanLocation>>(tourUpdateModel.Locations);
//     //             }

//     //             await _unitOfWork.TourPlanVersionRepository.AddAsync(newVersion);
//     //             tour.CurrentVersionId = newVersion.Id;
//     //         }
//     //         else
//     //         {
//     //             newVersion = latestVersion!;
//     //             _mapper.Map(tourUpdateModel, tour);

//     //             if (tourUpdateModel.Locations != null)
//     //             {
//     //                 newVersion.TourPlanLocations.Clear();
//     //                 newVersion.TourPlanLocations = _mapper.Map<List<TourPlanLocation>>(tourUpdateModel.Locations);
//     //             }

//     //             newVersion.VersionDate = _timeService.SystemTimeNow;
//     //         }

//     //         if (tourUpdateModel.TourSchedules != null)
//     //         {
//     //             // Xóa lịch cũ nếu đang update bản cũ
//     //             newVersion.TourSchedules.Clear();

//     //             foreach (var scheduleModel in tourUpdateModel.TourSchedules)
//     //             {
//     //                 var schedule = new TourSchedule
//     //                 {
//     //                     DepartureDate = scheduleModel.DepartureDate,
//     //                     MaxParticipant = scheduleModel.MaxParticipants,
//     //                     AdultPrice = scheduleModel.AdultPrice,
//     //                     ChildrenPrice = scheduleModel.ChildrenPrice,
//     //                     TotalDays = tourUpdateModel.TotalDays,
//     //                     TourPlanVersion = newVersion
//     //                 };

//     //                 newVersion.TourSchedules.Add(schedule);
//     //             }
//     //         }

//     //         await _unitOfWork.SaveAsync();
//     //         await transaction.CommitAsync(cancellationToken);

//     //         var result = _mapper.Map<TourDataModel>(tour);
//     //         result.CurrentVersionId = tour.CurrentVersionId ?? Guid.Empty;
//     //         return result;
//     //     }
//     //     catch (CustomException) { throw; }
//     //     catch (Exception ex)
//     //     {
//     //         await transaction.RollbackAsync(cancellationToken);
//     //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//     //     }
//     // }

//     public async Task<TourDataModel> AddTourAsync(TourCreateModel tourCreateModel, CancellationToken cancellationToken)
//     {
//         using var transaction = await _unitOfWork.BeginTransactionAsync();
//         try
//         {
//             var currentUserId = _userContextService.GetCurrentUserId();
//             var currentTime = _timeService.SystemTimeNow;

//             var newTour = _mapper.Map<Tour>(tourCreateModel);

//             newTour.CreatedBy = currentUserId;
//             newTour.LastUpdatedBy = currentUserId;
//             newTour.CreatedTime = currentTime;
//             newTour.LastUpdatedTime = currentTime;

//             await _unitOfWork.TourRepository.AddAsync(newTour);
//             await _unitOfWork.SaveAsync();
//             await transaction.CommitAsync(cancellationToken);

//             var result = _unitOfWork.TourRepository.ActiveEntities
//                 .FirstOrDefault(tp => tp.Id == newTour.Id);

//             return _mapper.Map<TourDataModel>(result);
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             // _unitOfWork.Dispose();
//         }
//     }

//     #region Helper Methods

//     public class TourItemScheduleDto
//     {
//         public string? ItemType { get; set; }
//         public int DayOrder { get; set; } = 1;
//         public TimeSpan StartTime { get; set; }
//         public TimeSpan EndTime { get; set; }
//         public Guid? ItemId { get; set; }
//     }

//     private List<TourItemScheduleDto> CollectAllSchedules(TourUpdateModel tourUpdateModel)
//     {
//         var schedules = new List<TourItemScheduleDto>();

//         foreach (var location in tourUpdateModel.Locations ?? new List<TourPlanLocationModel>())
//         {
//             if (location.StartTime.HasValue && location.EndTime.HasValue)
//             {
//                 schedules.Add(new TourItemScheduleDto
//                 {
//                     ItemType = "Location",
//                     DayOrder = location.DayOrder,
//                     StartTime = location.StartTime.Value,
//                     EndTime = location.EndTime.Value,
//                     ItemId = location.Id
//                 });
//             }
//             // Optionally, handle else case (e.g., log or throw if required)
//         }

//         return schedules.OrderBy(s => s.DayOrder).ToList();
//     }

//     private bool ValidateDateRanges(int totalDays, List<TourItemScheduleDto> schedules)
//     {
//         var result = true;

//         foreach (var schedule in schedules)
//         {
//             // if (schedule.StartTime.Date < tripStartDate.Date)
//             // {
//             //     result = false; // Thời gian bắt đầu của schedule nằm trước ngày bắt đầu của Tour
//             //     break;
//             // }

//             // if (schedule.EndTime.Date > tripEndDate.Date)
//             // {
//             //     result = false; // Thời gian kết thúc của schedule nằm sau ngày kết thúc của Tour
//             //     break;
//             // }

//             if (schedule.DayOrder < 1 || schedule.DayOrder > totalDays)
//             {
//                 result = false; // Ngày trong schedule không hợp lệ
//                 break;
//             }

//             // if (schedule.StartTime < tripStartDate || schedule.EndTime > tripEndDate)
//             // {
//             //     result = false; // Thời gian của schedule nằm ngoài khoảng thời gian của Tour
//             //     break;
//             // }

//             if (schedule.StartTime >= schedule.EndTime)
//             {
//                 result = false; // Thời gian bắt đầu không được lớn hơn thời gian kết thúc
//                 break;
//             }
//         }

//         return result;
//     }

//     private bool ValidateTimeConflicts(List<TourItemScheduleDto> schedules)
//     {
//         if (schedules.Count == 0) return true;

//         var result = true;

//         var sortedSchedules = schedules.OrderBy(s => s.DayOrder).ToList();

//         for (int i = 0; i < sortedSchedules.Count - 1; i++)
//         {
//             var current = sortedSchedules[i];
//             var next = sortedSchedules[i + 1];

//             var bufferMinutes = 15;
//             var bufferTime = TimeSpan.FromMinutes(bufferMinutes);

//             if (current.EndTime.Add(-bufferTime) > next.StartTime
//                 && current.DayOrder == next.DayOrder)
//             {
//                 result = false; // Có sự trùng lặp thời gian
//                 break;
//             }
//         }

//         return result;
//     }

//     private bool ValidateLogicalTimeSequences(List<TourItemScheduleDto> schedules)
//     {
//         if (schedules.Count == 0) return true;

//         var result = true;

//         var dailySchedules = schedules
//             .GroupBy(s => s.DayOrder)
//             .ToList();

//         foreach (var dailySchedule in dailySchedules)
//         {
//             var daySchedules = dailySchedule.OrderBy(s => s.StartTime).ToList();

//             foreach (var schedule in daySchedules)
//             {
//                 if (schedule.StartTime.Hours < 6)
//                 {
//                     result = false; // Không hợp lý nếu bắt đầu quá sớm
//                 }

//                 if (schedule.EndTime.Hours > 23)
//                 {
//                     result = false; // Không hợp lý nếu kết thúc quá muộn
//                 }
//             }

//             if (!result) break;
//         }

//         return result;
//     }

//     private bool ValidateTourSchedule(TourUpdateModel tourUpdateModel)
//     {
//         var result = true;

//         // if (tourUpdateModel.StartDate >= tourUpdateModel.EndDate)
//         // {
//         //     result = false; // Ngày bắt đầu phải trước ngày kết thúc
//         // }

//         var allSchedules = CollectAllSchedules(tourUpdateModel);

//         var dateRangeValidation = ValidateDateRanges(tourUpdateModel.TotalDays, allSchedules);
//         if (!dateRangeValidation)
//         {
//             result = false; // Có schedule nằm ngoài khoảng thời gian của Tour
//             throw CustomExceptionFactory.CreateBadRequestError("Lịch trình nằm ngoài khoảng thời gian của kế hoạch chuyến đi");
//         }

//         var timeConflictValidation = ValidateTimeConflicts(allSchedules);
//         if (!timeConflictValidation)
//         {
//             result = false; // Có sự trùng lặp thời gian trong các schedule
//             throw CustomExceptionFactory.CreateBadRequestError("Lịch trình có sự trùng lặp thời gian");
//         }

//         var logicalValidation = ValidateLogicalTimeSequences(allSchedules);
//         if (!logicalValidation)
//         {
//             result = false; // Có sự không hợp lý trong trình tự thời gian
//             throw CustomExceptionFactory.CreateBadRequestError("Lịch trình có trình tự thời gian không hợp lý");
//         }

//         return result;
//     }

//     private async Task<List<TourActivity>> GetAllActivities(Guid tourId)
//     {
//         try
//         {

//             var activities = new List<TourActivity>();

//             var tour = await _unitOfWork.TourRepository
//                 .ActiveEntities
//                 .Include(x => x.TourPlanLocations)
//                 .FirstOrDefaultAsync(x => x.Id == tourId);

//             if (tour == null)
//                 throw CustomExceptionFactory.CreateNotFoundError("Tour hoặc version");

//             // Locations
//             foreach (var tpl in tour.TourPlanLocations ?? new List<TourPlanLocation>())
//             {
//                 var location = await _unitOfWork.LocationRepository.GetByIdAsync(tpl.LocationId, cancellationToken: CancellationToken.None);
//                 if (location == null) continue;

//                 activities.Add(new TourActivity
//                 {
//                     Id = tpl.Id,
//                     Type = TripActivityTypeEnum.Location.ToString(),
//                     DayOrder = tpl.DayOrder,
//                     Name = location.Name,
//                     Description = location.Description,
//                     Address = location.Address ?? string.Empty,
//                     StartTime = tpl.StartTime,
//                     EndTime = tpl.EndTime,
//                     StartTimeFormatted = tpl.StartTime.ToString(@"hh\:mm"),
//                     EndTimeFormatted = tpl.EndTime.ToString(@"hh\:mm"),
//                     Duration = $"{(tpl.EndTime - tpl.StartTime).TotalMinutes} phút",
//                     Notes = tpl.Notes,
//                     ImageUrl = await GetLocationImageUrl(tpl.Id) ?? string.Empty
//                 });
//             }

//             return activities.OrderBy(x => x.StartTime).ToList();
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             // _unitOfWork.Dispose();
//         }
//     }

//     private async Task<List<TourActivity>> GetAllActivitiesByVersionId(Guid tourId)
//     {
//         try
//         {

//             var activities = new List<TourActivity>();

//             var tour = await _unitOfWork.TourRepository
//                 .ActiveEntities
//                 .Include(x => x.TourPlanLocations)
//                 .FirstOrDefaultAsync(x => x.Id == tourId)
//                 ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

//             // Locations
//             foreach (var tpl in tour.TourPlanLocations ?? new List<TourPlanLocation>())
//             {
//                 var location = await _unitOfWork.LocationRepository.GetByIdAsync(tpl.LocationId, cancellationToken: CancellationToken.None);
//                 if (location == null) continue;

//                 activities.Add(new TourActivity
//                 {
//                     Id = tpl.Id,
//                     Type = TripActivityTypeEnum.Location.ToString(),
//                     DayOrder = tpl.DayOrder,
//                     Name = location.Name,
//                     Description = location.Description,
//                     Address = location.Address ?? string.Empty,
//                     StartTime = tpl.StartTime,
//                     EndTime = tpl.EndTime,
//                     StartTimeFormatted = tpl.StartTime.ToString(@"hh\:mm"),
//                     EndTimeFormatted = tpl.EndTime.ToString(@"hh\:mm"),
//                     Duration = $"{(tpl.EndTime - tpl.StartTime).TotalMinutes} phút",
//                     Notes = tpl.Notes,
//                     ImageUrl = await GetLocationImageUrl(tpl.Id) ?? string.Empty
//                 });
//             }

//             return activities.OrderBy(x => x.StartTime).ToList();
//         }
//         catch (CustomException)
//         {
//             throw;
//         }
//         catch (Exception ex)
//         {
//             throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
//         }
//         finally
//         {
//             // _unitOfWork.Dispose();
//         }
//     }

//     private async Task<string> GetLocationImageUrl(Guid locationId)
//     {
//         // Note: Nếu có isThumbnail thì lấy ảnh thumbnail, nếu không thì lấy ảnh đầu tiên
//         var locationMedia = await _unitOfWork.LocationMediaRepository.ActiveEntities
//             .FirstOrDefaultAsync(l => l.LocationId == locationId);

//         return locationMedia != null ? locationMedia.MediaUrl ?? string.Empty : string.Empty;
//     }

//     private List<TourDayDetail> BuildDaySchedule(int totalDays, List<TourActivity> activities)
//     {
//         var days = new List<TourDayDetail>();
//         // var currentDate = startDate.Date;
//         var dayNumber = 1;

//         while (dayNumber <= totalDays)
//         {
//             var dayActivities = activities
//                 .Where(a => a.DayOrder == dayNumber)
//                 .ToList();

//             dayActivities = SortActivitiesByTime(dayActivities);

//             days.Add(new TourDayDetail
//             {
//                 DayNumber = dayNumber,
//                 Activities = dayActivities
//             });
//             dayNumber++;
//         }

//         return days;
//     }

//     private List<TourActivity> SortActivitiesByTime(List<TourActivity> activities)
//     {
//         return activities.OrderBy(a =>
//         {
//             // if (a.Order.HasValue)
//             //     return a.Order.Value;

//             if (a.StartTime.HasValue)
//                 return a.StartTime.Value.Hours * 60 + a.StartTime.Value.Minutes;

//             return int.MaxValue;
//         })
//         .ThenBy(a => a.StartTime ?? TimeSpan.MaxValue)
//         .ToList();
//     }

//     private string? CalculateDuration(DateTime? startTime, DateTime? endTime)
//     {
//         if (!startTime.HasValue || !endTime.HasValue)
//             return null;

//         var duration = endTime.Value - startTime.Value;

//         if (duration.TotalHours >= 1)
//         {
//             var hours = (int)duration.TotalHours;
//             var minutes = duration.Minutes;
//             return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours}h";
//         }
//         else
//         {
//             return $"{duration.Minutes}m";
//         }
//     }

//     #endregion

// }

// public class TourVersionDto
// {
//     public Guid Id { get; set; }
//     public string? Notes { get; set; }
//     public DateTimeOffset CreatedTime { get; set; }
//     public List<TourPlanLocationDto>? TourPlanLocations { get; set; }
// }

// public class TourPlanLocationDto
// {
//     public Guid LocationId { get; set; }
//     public string? Notes { get; set; }
//     public DateTime StartTime { get; set; }
//     public DateTime EndTime { get; set; }
//     public int Order { get; set; }
// }
