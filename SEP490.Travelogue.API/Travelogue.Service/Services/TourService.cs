using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourService
{
    Task<List<TourResponseDto>> GetAllToursAsync(TourFilterModel filter);
    Task<TourResponseDto> CreateTourAsync(CreateTourDto dto);
    Task<TourResponseDto> UpdateTourAsync(Guid tourId, UpdateTourDto dto);
    Task<bool> DeleteTourAsync(Guid id, CancellationToken cancellationToken);
    Task<TourResponseDto> ConfirmTourAsync(Guid tourId, ConfirmTourDto dto);
    Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId);
    Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId, Guid? scheduleId = null);
    Task<List<TourPlanLocationResponseDto>> UpdateLocationsAsync(Guid tourId, List<UpdateTourPlanLocationDto> dtos);
    Task<List<TourPlanLocationResponseDto>> GetLocationsAsync(Guid tourId);
    Task<List<TourScheduleResponseDto>> CreateSchedulesAsync(Guid tourId, List<CreateTourScheduleDto> dtos);
    Task<TourScheduleResponseDto> UpdateScheduleAsync(Guid tourId, Guid scheduleId, CreateTourScheduleDto dto);
    Task DeleteScheduleAsync(Guid tourId, Guid scheduleId);

    Task AddTourGuideToScheduleAsync(Guid tourScheduleId, Guid guideId);
    Task RemoveTourGuideAsync(Guid tourScheduleId, Guid guideId);

    Task<List<TourMedia>> AddTourMediasAsync(Guid tourId, List<TourMediaCreateDto> createDtos);
    Task<bool> DeleteTourMediaAsync(Guid tourMediaId);

    Task<PagedResult<TourResponseDto>> GetPagedToursByGuideEmailAsync(string email, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IEnumService _enumService;
    private readonly IUserContextService _userContextService;
    private readonly IMediaService _mediaService;
    private readonly ITimeService _timeService;

    public TourService(IUnitOfWork unitOfWork, IEmailService emailService, IEnumService enumService, IUserContextService userContextService, IMediaService mediaService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _enumService = enumService;
        _userContextService = userContextService;
        _mediaService = mediaService;
        _timeService = timeService;
    }

    public async Task<TourResponseDto> CreateTourAsync(CreateTourDto dto)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var isValidRole = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            if (!isValidRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên tour là bắt buộc.");
            if (dto.TotalDays <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Số ngày phải lớn hơn 0.");

            var tour = new Tour
            {
                Name = dto.Name,
                Description = dto.Description,
                TransportType = dto.TransportType,
                StayInfo = dto.StayInfo,
                PickupAddress = dto.PickupAddress,
                Content = dto.Content,
                TotalDays = dto.TotalDays,
                TourType = dto.TourType,
                Status = TourStatus.Draft,
                CreatedBy = currentUserId,
                CreatedTime = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime
            };

            await _unitOfWork.TourRepository.AddAsync(tour);
            await _unitOfWork.SaveAsync();

            List<TourMedia> tourMedias = new();
            if (dto.MediaDtos.Any())
            {
                tourMedias = dto.MediaDtos.Select(media => new TourMedia
                {
                    Id = Guid.NewGuid(),
                    TourId = tour.Id,
                    MediaUrl = media.MediaUrl,
                    IsThumbnail = media.IsThumbnail,
                    CreatedBy = currentUserId,
                    LastUpdatedBy = currentUserId,
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                }).ToList();

                await _unitOfWork.TourMediaRepository.AddRangeAsync(tourMedias);
                await _unitOfWork.SaveAsync();
            }

            var activeSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
            var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
            var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
            var finalPrice = adultPrice;

            // var tourGuide = GetTourGuidesInfo(tour);

            var groupedLocations = tour.TourPlanLocations
                .Where(l => !l.IsDeleted)
                .GroupBy(l => l.DayOrder)
                .OrderBy(g => g.Key)
                .ToList();

            // var dayDetails = BuildDayDetails(tour);

            var tourResponse = new TourResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TransportType = tour.TransportType,
                StayInfo = tour.StayInfo,
                PickupAddress = tour.PickupAddress,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourType = tour.TourType,
                TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                Status = tour.Status,
                // TourGuide = tourGuide,
                // DayDetails = dayDetails,
            };

            return tourResponse;
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

    public async Task<TourResponseDto> UpdateTourAsync(Guid tourId, UpdateTourDto dto)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var isValidRole = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            if (!isValidRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên tour là bắt buộc.");
            if (dto.TotalDays <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Số ngày phải lớn hơn 0.");

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Include(t => t.TourSchedules)
                .Include(t => t.TourPlanLocations)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã bị hủy.");

            if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã có người đặt.");

            // Validate TotalDays
            // var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            // if (dto.TotalDays < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({dto.TotalDays}) không được nhỏ hơn ngày lớn nhất đã lên kế hoạch ({maxDayOrder}).");

            // Track changes for notification
            var changes = GetTourChanges(tour, dto);

            // Update tour properties
            tour.Name = dto.Name;
            tour.Description = dto.Description;
            tour.Content = dto.Content;
            tour.TransportType = dto.TransportType;
            tour.StayInfo = dto.StayInfo;
            tour.PickupAddress = dto.PickupAddress;
            tour.TourType = dto.TourType;
            tour.TotalDays = dto.TotalDays;
            tour.LastUpdatedTime = DateTimeOffset.UtcNow;
            tour.Status = TourStatus.Draft;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.SaveAsync();

                    // if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed) && changes.Any())
                    // {
                    //     var changeSummary = string.Join("\n", changes);
                    //     foreach (var booking in tour.Bookings)
                    //     {
                    //         await _emailService.SendEmailAsync(
                    //             new[] { booking.User.Email },
                    //             $"Cập nhật thông tin Tour {tour.Name}",
                    //             $"Tour {tour.Name} đã có các thay đổi sau:\n{changeSummary}\nVui lòng kiểm tra chi tiết."
                    //         );
                    //     }
                    // }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            await UpdateTourMediasAsync(tourId, dto.MediaDtos, new CancellationToken());

            var activeSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
            var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
            var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
            var finalPrice = adultPrice;

            // var tourGuide = GetTourGuidesInfo(tour);

            var groupedLocations = tour.TourPlanLocations
                .Where(l => !l.IsDeleted)
                .GroupBy(l => l.DayOrder)
                .OrderBy(g => g.Key)
                .ToList();

            // var dayDetails = BuildDayDetails(tour);

            var tourResponse = new TourResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TransportType = tour.TransportType,
                StayInfo = tour.StayInfo,
                PickupAddress = tour.PickupAddress,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourType = tour.TourType,
                TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                Status = tour.Status,
                // TourGuide = tourGuide,
                // DayDetails = dayDetails,
            };

            return tourResponse;
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

    public async Task<TourResponseDto> ConfirmTourAsync(Guid tourId, ConfirmTourDto dto)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var isValidRole = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            if (!isValidRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.TourPlanLocations == null || !tour.TourPlanLocations.Any(l => !l.IsDeleted))
                throw CustomExceptionFactory.CreateBadRequestError("Tour chưa có điểm đến (locations).");

            if (tour.TourSchedules == null || !tour.TourSchedules.Any(s => !s.IsDeleted))
                throw CustomExceptionFactory.CreateBadRequestError("Tour chưa có lịch trình (schedules).");

            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;

            if (tour.TotalDays < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({tour.TotalDays}) không được nhỏ hơn ngày lớn nhất đã lên kế hoạch ({maxDayOrder}).");

            if (tour.Status == TourStatus.Confirmed)
                throw CustomExceptionFactory.CreateBadRequestError("Tour đã được xác nhận.");

            tour.Status = TourStatus.Confirmed;
            tour.LastUpdatedTime = DateTimeOffset.UtcNow;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.SaveAsync();

                    // if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    // {
                    //     foreach (var booking in tour.Bookings)
                    //     {
                    //         await _emailService.SendEmailAsync(
                    //             new[] { booking.User.Email },
                    //             $"Tour {tour.Name} Đã Được Xác Nhận",
                    //             $"Tour {tour.Name} đã được xác nhận và sẵn sàng cho bạn tham gia. Vui lòng kiểm tra chi tiết."
                    //         );
                    //     }
                    // }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return new TourResponseDto
            {
                TourId = tour.Id,
                Status = tour.Status
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

    public async Task<List<TourResponseDto>> GetAllToursAsync(TourFilterModel filter)
    {
        try
        {
            var currentTime = _timeService.SystemTimeNow;

            var isValidRole = _userContextService.HasAnyRoleOrAnonymous(AppRole.MODERATOR, AppRole.ADMIN);

            var toursQuery = _unitOfWork.TourRepository.ActiveEntities
                .Include(t => t.TourSchedules)
                .Include(t => t.PromotionApplicables)
                .AsQueryable();

            if (!isValidRole)
            {
                toursQuery = toursQuery.Where(t => t.Status == TourStatus.Confirmed);
            }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var keyword = filter.Name.Trim().ToLower();
                toursQuery = toursQuery.Where(t => t.Name.ToLower().Contains(keyword));
            }

            if (filter.TotalDaysMin.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.TotalDays >= filter.TotalDaysMin.Value);
            }

            if (filter.TotalDaysMax.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.TotalDays <= filter.TotalDaysMax.Value);
            }

            if (filter.TourType.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.TourType == filter.TourType.Value);
            }

            if (filter.PriceMin.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.TourSchedules.Any(s => s.AdultPrice >= filter.PriceMin.Value));
            }

            if (filter.PriceMax.HasValue)
            {
                toursQuery = toursQuery.Where(t => t.TourSchedules.Any(s => s.AdultPrice <= filter.PriceMax.Value));
            }

            var tours = await toursQuery.ToListAsync();

            if (!tours.Any())
                throw CustomExceptionFactory.CreateNotFoundError("tours");

            var tourIds = tours.Select(t => t.Id).ToList();

            var bookings = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.TourId.HasValue && tourIds.Contains(b.TourId.Value))
                .Include(b => b.User)
                .ToListAsync();

            var tourPlanLocations = await _unitOfWork.TourPlanLocationRepository
                .ActiveEntities
                .Where(l => tourIds.Contains(l.TourId))
                .Include(l => l.Location)
                .ToListAsync();

            var reviews = await _unitOfWork.ReviewRepository
                .ActiveEntities
                .Include(r => r.Booking)
                .Where(r => r.Booking.TourId.HasValue && tourIds.Contains(r.Booking.TourId.Value))
                .ToListAsync();

            var tourResponses = new List<TourResponseDto>();

            foreach (var tour in tours)
            {
                var activeSchedules = tour.TourSchedules?.Where(s => !s.IsDeleted).ToList() ?? new();
                var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
                var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
                var finalPrice = adultPrice;

                var tourBookings = bookings.Where(b => b.TourId == tour.Id).ToList();
                var reviewsForTour = reviews.Where(r => r.Booking.TourId == tour.Id).ToList();

                var tourLocations = tourPlanLocations
                    .Where(l => l.TourId == tour.Id && !l.IsDeleted)
                    .ToList();

                var tourGuideInfo = GetTourGuidesInfo(tour);

                var validReviews = reviewsForTour
                    .Where(r => r != null && r.Booking != null && r.Rating >= 0)
                    .ToList();

                var averageRating = validReviews.Any()
                    ? validReviews.Average(r => r.Rating)
                    : 0.0;

                var medias = await GetMediaWithoutVideoByIdAsync(tour.Id, cancellationToken: default);

                string createUserName = TryParseGuid(tour.CreatedBy, out var createdByGuid)
                    ? await _unitOfWork.UserRepository.GetUserNameByIdAsync(createdByGuid) ?? "Unknown User"
                    : "Unknown User";

                string lastUpdateUserName = TryParseGuid(tour.LastUpdatedBy, out var lastUpdatedByGuid)
                    ? await _unitOfWork.UserRepository.GetUserNameByIdAsync(lastUpdatedByGuid) ?? "Unknown User"
                    : "Unknown User";

                bool TryParseGuid(string? input, out Guid result)
                {
                    return Guid.TryParse(input, out result);
                }
                tourResponses.Add(new TourResponseDto
                {
                    TourId = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Content = tour.Content,
                    TransportType = tour.TransportType,
                    StayInfo = tour.StayInfo,
                    PickupAddress = tour.PickupAddress,
                    TotalDays = tour.TotalDays,
                    TotalDaysText = tour.TotalDays == 1
                        ? "1 ngày"
                        : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                    TourType = tour.TourType,
                    TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                    AdultPrice = adultPrice,
                    ChildrenPrice = childrenPrice,
                    FinalPrice = finalPrice,
                    Status = tour.Status,
                    TotalReviews = reviewsForTour.Count,
                    AverageRating = averageRating,
                    CreatedBy = tour.CreatedBy,
                    CreatedByName = createUserName,
                    CreatedTime = tour.CreatedTime,
                    LastUpdatedBy = tour.LastUpdatedBy,
                    LastUpdatedByName = lastUpdateUserName,
                    LastUpdatedTime = tour.LastUpdatedTime,
                    Medias = medias ?? new List<MediaResponse>(),
                });
            }

            return tourResponses;
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

    public async Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Draft && !_userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Tour đang ở trạng thái nháp, không thể xem chi tiết.");
            }

            var tourSchedules = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Where(s => s.TourId == tourId)
                .Include(s => s.TourGuideSchedules)
                    .ThenInclude(tg => tg.TourGuide)
                        .ThenInclude(tg => tg.User)
                .ToListAsync();

            var tourPlanLocations = await _unitOfWork.TourPlanLocationRepository
                .ActiveEntities
                .Where(l => l.TourId == tourId)
                .Include(l => l.Location)
                .ToListAsync();

            var promotionApplicables = await _unitOfWork.PromotionApplicableRepository
                .ActiveEntities
                .Where(p => p.TourId == tourId)
                .Include(p => p.Promotion)
                .ToListAsync();

            var tourReviews = await _unitOfWork.ReviewRepository
                .ActiveEntities
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Where(r => r.Booking.TourId.HasValue && r.Booking.TourId.Value == tourId)
                .ToListAsync();

            var reviewByTourGuide = await _unitOfWork.ReviewRepository
                .ActiveEntities
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Where(r =>
                    r.Booking.TourId.HasValue &&
                    r.Booking.TourId.Value == tourId &&
                    r.Booking.TourGuideId.HasValue)
                .GroupBy(r => r.Booking.TourGuideId.Value)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => new
                    {
                        AverageRating = Math.Round(g.Average(r => r.Rating), 2),
                        TotalReviews = g.Count()
                    });

            // Tính giá
            var activeSchedules = tourSchedules.Where(s => !s.IsDeleted).ToList();
            var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
            var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;

            // Tính khuyến mãi
            var activePromotions = promotionApplicables
                .Where(p => !p.IsDeleted && p.Promotion != null && p.Promotion.StartDate <= DateTime.UtcNow && p.Promotion.EndDate >= DateTime.UtcNow)
                .Select(p => new PromotionDto
                {
                    Id = p.Promotion.Id,
                    Name = p.Promotion.PromotionName,
                    DiscountPercentage = p.Promotion.DiscountType == DiscountType.Percentage
                        ? p.Promotion.DiscountValue
                        : 0,
                    StartDate = p.Promotion.StartDate,
                    EndDate = p.Promotion.EndDate
                })
                .ToList();

            var isDiscount = activePromotions.Any();
            var maxDiscount = isDiscount ? activePromotions.Max(p => p.DiscountPercentage) : 0;
            var finalPrice = adultPrice * (1 - maxDiscount / 100);

            var groupedLocations = tourPlanLocations
                .Where(l => !l.IsDeleted)
                .GroupBy(l => l.DayOrder)
                .OrderBy(g => g.Key)
                .ToList();

            var dayDetails = await BuildDayDetails(tour);

            // đánh giá
            double averageRating = tourReviews.Any() ? tourReviews.Average(r => r.Rating) : 0.0;

            var rating = new RatingDetailsDto
            {
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = tourReviews.Count,
                Reviews = tourReviews.Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User?.FullName ?? string.Empty,
                    BookingId = r.BookingId,
                    TourId = r.Booking.TourId,
                    WorkshopId = r.Booking.WorkshopId,
                    TourGuideId = r.Booking.TourGuideId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedTime,
                    UpdatedAt = r.LastUpdatedTime
                }).ToList()
            };

            var medias = await GetMediaWithoutVideoByIdAsync(tourId, cancellationToken: default);

            var scheduleDtos = new List<TourScheduleResponseDto>();

            foreach (var schedule in activeSchedules)
            {
                var validTourGuide = schedule.TourGuideSchedules
                    .Where(tg => !tg.IsDeleted && tg.TourGuide != null && tg.TourGuide.User != null)
                    .Select(tg => new TourGuideDataModel
                    {
                        Id = tg.TourGuide.Id,
                        UserName = tg.TourGuide.User.FullName,
                        Email = tg.TourGuide.User.Email,
                        AverageRating = reviewByTourGuide.TryGetValue(tg.TourGuide.Id, out var rating) ? rating.AverageRating : 0.0,
                        TotalReviews = reviewByTourGuide.TryGetValue(tg.TourGuide.Id, out var r) ? r.TotalReviews : 0
                    })
                    .FirstOrDefault();
                var scheduleDto = new TourScheduleResponseDto
                {
                    ScheduleId = schedule.Id,
                    StartTime = schedule.DepartureDate,
                    EndTime = schedule.DepartureDate.AddDays(tour.TotalDays - 1),
                    MaxParticipant = schedule.MaxParticipant,
                    CurrentBooked = schedule.CurrentBooked,
                    AdultPrice = schedule.AdultPrice,
                    ChildrenPrice = schedule.ChildrenPrice,
                    TourGuide = validTourGuide
                };

                scheduleDtos.Add(scheduleDto);
            }

            var result = new TourDetailsResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TransportType = tour.TransportType,
                StayInfo = tour.StayInfo,
                PickupAddress = tour.PickupAddress,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourType = tour.TourType,
                TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                IsDiscount = isDiscount,
                Status = tour.Status,
                StatusText = _enumService.GetEnumDisplayName<TourStatus>(tour.Status),
                Schedules = scheduleDtos,
                Promotions = activePromotions,
                Days = dayDetails,
                Reviews = rating.Reviews,
                TotalReviews = rating.TotalReviews,
                AverageRating = rating.AverageRating,
                Medias = medias ?? new List<MediaResponse>()
            };

            if (dayDetails.Any())
            {
                var firstDay = dayDetails.First();
                var lastDay = dayDetails.Last();

                if (firstDay.Activities.Any())
                    result.StartLocation = firstDay.Activities.First();

                if (lastDay.Activities.Any())
                    result.EndLocation = lastDay.Activities.Last();
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

    public async Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId, Guid? scheduleId = null)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(l => l.Location)
                .Include(t => t.TourSchedules)
                    .ThenInclude(t => t.TourGuideSchedules)
                        .ThenInclude(tg => tg.TourGuide)
                            .ThenInclude(tg => tg.User)
                .Include(t => t.PromotionApplicables)
                    .ThenInclude(p => p.Promotion)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            // if (tour.Status == TourStatus.Draft && !_userContextService.HasAnyRoleOrAnonymous(AppRole.MODERATOR, AppRole.ADMIN))
            // {
            //     throw CustomExceptionFactory.CreateBadRequestError("Tour đang ở trạng thái nháp, không thể xem chi tiết.");
            // }

            var activeSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();

            decimal adultPrice, childrenPrice;
            if (scheduleId.HasValue)
            {
                var selectedSchedule = activeSchedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                if (selectedSchedule == null)
                    throw CustomExceptionFactory.CreateNotFoundError("Schedule không tồn tại trong tour này.");

                adultPrice = selectedSchedule.AdultPrice;
                childrenPrice = selectedSchedule.ChildrenPrice;
            }
            else
            {
                adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
                childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
            }

            var reviewByTourGuide = await _unitOfWork.ReviewRepository
               .ActiveEntities
               .Include(r => r.User)
               .Include(r => r.Booking)
               .Where(r =>
                   r.Booking.TourId.HasValue &&
                   r.Booking.TourId.Value == tourId &&
                   r.Booking.TourGuideId.HasValue)
               .GroupBy(r => r.Booking.TourGuideId.Value)
               .ToDictionaryAsync(
                   g => g.Key,
                   g => new
                   {
                       AverageRating = Math.Round(g.Average(r => r.Rating), 2),
                       TotalReviews = g.Count()
                   });

            // tính giá khuyến mãi
            var activePromotions = tour.PromotionApplicables
                .Where(p => !p.IsDeleted && p.Promotion != null
                    && p.Promotion.StartDate <= DateTime.UtcNow
                    && p.Promotion.EndDate >= DateTime.UtcNow)
                .Select(p => new PromotionDto
                {
                    Id = p.Promotion.Id,
                    Name = p.Promotion.PromotionName,
                    DiscountPercentage = p.Promotion.DiscountType == DiscountType.Percentage ? p.Promotion.DiscountValue : 0,
                    StartDate = p.Promotion.StartDate,
                    EndDate = p.Promotion.EndDate
                })
                .ToList();

            var isDiscount = activePromotions.Any();
            var maxDiscount = isDiscount ? activePromotions.Max(p => p.DiscountPercentage) : 0;
            var finalPrice = adultPrice * (1 - maxDiscount / 100);

            var tourGuide = GetTourGuidesInfo(tour);
            var dayDetails = await BuildDayDetails(tour);

            var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.User)
                .Where(r => r.Booking.TourId == tourId)
                .ToListAsync();

            double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;

            var rating = new RatingDetailsDto
            {
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = reviews.Count,
                Reviews = reviews.Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User?.FullName ?? string.Empty,
                    BookingId = r.BookingId,
                    TourId = r.Booking.TourId,
                    WorkshopId = r.Booking.WorkshopId,
                    TourGuideId = r.Booking.TourGuideId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedTime,
                    UpdatedAt = r.LastUpdatedTime
                }).ToList()
            };

            var medias = await GetMediaWithoutVideoByIdAsync(tourId, cancellationToken: default);

            var scheduleDtos = new List<TourScheduleResponseDto>();

            foreach (var schedule in activeSchedules)
            {
                var validTourGuide = schedule.TourGuideSchedules
                    .Where(tg => !tg.IsDeleted && tg.TourGuide != null && tg.TourGuide.User != null)
                    .Select(tg => new TourGuideDataModel
                    {
                        Id = tg.TourGuide.Id,
                        UserName = tg.TourGuide.User.FullName,
                        Email = tg.TourGuide.User.Email,
                        AverageRating = reviewByTourGuide.TryGetValue(tg.TourGuide.Id, out var rating) ? rating.AverageRating : 0.0,
                        TotalReviews = reviewByTourGuide.TryGetValue(tg.TourGuide.Id, out var r) ? r.TotalReviews : 0
                    })
                    .FirstOrDefault();
                var scheduleDto = new TourScheduleResponseDto
                {
                    ScheduleId = schedule.Id,
                    StartTime = schedule.DepartureDate,
                    EndTime = schedule.DepartureDate.AddDays(tour.TotalDays - 1),
                    MaxParticipant = schedule.MaxParticipant,
                    CurrentBooked = schedule.CurrentBooked,
                    AdultPrice = schedule.AdultPrice,
                    ChildrenPrice = schedule.ChildrenPrice,
                    TourGuide = validTourGuide
                };

                scheduleDtos.Add(scheduleDto);
            }

            return new TourDetailsResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TransportType = tour.TransportType,
                StayInfo = tour.StayInfo,
                PickupAddress = tour.PickupAddress,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourType = tour.TourType,
                TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                IsDiscount = isDiscount,
                Status = tour.Status,
                StatusText = _enumService.GetEnumDisplayName<TourStatus>(tour.Status),
                Schedules = scheduleDtos,
                Promotions = activePromotions,
                Days = dayDetails,
                Reviews = rating.Reviews,
                TotalReviews = rating.TotalReviews,
                AverageRating = rating.AverageRating,
                Medias = medias ?? new List<MediaResponse>()
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

    public async Task<bool> DeleteTourAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == id)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa tour đã bị hủy.");

            if (tour.TourSchedules.Any(s => s.CurrentBooked > 0))
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa tour đã có người đặt.");

            if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa tour đã có người đặt.");
            }

            // var remainingSchedules = tour.TourSchedules.Where(s => s.Id != scheduleId).Select(s => s.DepartureDate.Date).Distinct().ToList();
            // var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            // if (remainingSchedules.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không đủ số ngày lịch trình ({remainingSchedules.Count}) để bao phủ các ngày trong kế hoạch ({maxDayOrder}).");

            tour.IsDeleted = true;
            _unitOfWork.TourRepository.Update(tour);
            await _unitOfWork.SaveAsync();

            return true;
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

    #region TourPlanLocation 

    public async Task<List<TourPlanLocationResponseDto>> UpdateLocationsAsync(Guid tourId, List<UpdateTourPlanLocationDto> dtos)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(p => p.WorkshopDetail)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings).ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Cannot update locations of a cancelled tour.");

            foreach (var dto in dtos)
            {
                if (dto.TourPlanLocationId.HasValue && !tour.TourPlanLocations.Any(l => l.Id == dto.TourPlanLocationId && !l.IsDeleted))
                    throw CustomExceptionFactory.CreateBadRequestError($"Location with ID {dto.TourPlanLocationId} not found or already deleted.");
                if (dto.LocationId == Guid.Empty)
                    throw CustomExceptionFactory.CreateBadRequestError($"Destination ID is required for DayOrder {dto.DayOrder}.");
                if (dto.DayOrder < 1)
                    throw CustomExceptionFactory.CreateBadRequestError($"DayOrder must be positive for destination {dto.LocationId}.");
                if (dto.StartTime >= dto.EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"End time must be after start time for destination {dto.LocationId}.");
            }

            if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã có người đặt.");

            var locationIds = dtos.Select(d => d.LocationId).Distinct().ToList();

            // ---- Prefetch thông tin Location (kèm loại)
            var locations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Where(l => locationIds.Contains(l.Id))
                .Select(l => new
                {
                    l.Id,
                    l.OpenTime,
                    l.CloseTime,
                    l.LocationType
                })
                .ToListAsync();

            var validLocationIds = locations.Select(l => l.Id).ToList();
            var invalidLocations = locationIds.Except(validLocationIds).ToList();
            if (invalidLocations.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"Các ID địa điểm không hợp lệ: {string.Join(", ", invalidLocations)}");

            var craftVillageMap = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .Where(cv => locationIds.Contains(cv.LocationId))
                .Select(cv => new { cv.LocationId, CraftVillageId = cv.Id })
                .ToListAsync();

            var locToCv = craftVillageMap.ToDictionary(x => x.LocationId, x => x.CraftVillageId);

            var invalidTimeLocations = new List<string>();
            foreach (var dto in dtos)
            {
                var location = locations.FirstOrDefault(l => l.Id == dto.LocationId);
                if (location != null)
                {
                    if (dto.StartTime < location.OpenTime || dto.EndTime > location.CloseTime)
                    {
                        invalidTimeLocations.Add($"{dto.LocationId} (Start: {dto.StartTime}, End: {dto.EndTime}, Open: {location.OpenTime}, Close: {location.CloseTime})");
                    }
                }
            }
            if (invalidTimeLocations.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"Các địa điểm có giờ không hợp lệ: {string.Join("; ", invalidTimeLocations)}");

            var existingLocations = tour.TourPlanLocations.Where(l => !l.IsDeleted).ToList();

            var providedLocationIds = dtos.Where(d => d.TourPlanLocationId.HasValue).Select(d => d.TourPlanLocationId.Value).ToList();
            var toDelete = existingLocations.Where(l => !providedLocationIds.Contains(l.Id)).ToList();

            var toAdd = dtos
                .Where(d => !d.TourPlanLocationId.HasValue)
                .Select(d => new TourPlanLocation
                {
                    Id = Guid.NewGuid(),
                    TourId = tourId,
                    LocationId = d.LocationId,
                    DayOrder = d.DayOrder,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Notes = d.Notes,
                    TravelTimeFromPrev = d.TravelTimeFromPrev,
                    DistanceFromPrev = d.DistanceFromPrev,
                    EstimatedStartTime = d.EstimatedStartTime,
                    EstimatedEndTime = d.EstimatedEndTime,
                    ActivityType = d.ActivityType,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId.ToString(),
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedBy = userId.ToString(),
                    LastUpdatedTime = DateTimeOffset.UtcNow
                })
                .ToList();

            var toUpdate = dtos.Where(d => d.TourPlanLocationId.HasValue).ToList();
            var updateIds = toUpdate.Select(u => u.TourPlanLocationId.Value).ToHashSet();

            // Validate time overlaps
            var allLocations = existingLocations
                .Where(l => !toDelete.Contains(l) && !updateIds.Contains(l.Id))
                .Select(l => new { l.Id, l.StartTime, l.EndTime, l.DayOrder })
                .Concat(toAdd.Select(l => new { Id = Guid.Empty, l.StartTime, l.EndTime, l.DayOrder }))
                .Concat(toUpdate.Select(u => new { Id = u.TourPlanLocationId!.Value, u.StartTime, u.EndTime, u.DayOrder }))
                .GroupBy(l => l.DayOrder)
                .ToList();

            foreach (var group in allLocations)
            {
                var locationsInDay = group.OrderBy(l => l.StartTime).ToList();
                for (int i = 1; i < locationsInDay.Count; i++)
                {
                    if (locationsInDay[i].StartTime < locationsInDay[i - 1].EndTime)
                        throw CustomExceptionFactory.CreateBadRequestError($"Phát hiện trùng thời gian trong Ngày thứ {group.Key}.");
                }
            }

            tour.Status = TourStatus.Draft;
            _unitOfWork.TourRepository.Update(tour);

            var changes = new List<string>();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    foreach (var location in toDelete)
                    {
                        location.IsDeleted = true;
                        location.LastUpdatedTime = DateTimeOffset.UtcNow;
                        changes.Add($"Đã xóa địa điểm: {location.LocationId}");
                        _unitOfWork.TourPlanLocationRepository.Update(location);

                        if (location.WorkshopDetail != null && !location.WorkshopDetail.IsDeleted)
                        {
                            location.WorkshopDetail.IsDeleted = true;
                            location.WorkshopDetail.LastUpdatedBy = userId.ToString();
                            location.WorkshopDetail.LastUpdatedTime = DateTimeOffset.UtcNow;
                            _unitOfWork.TourPlanLocationWorkshopRepository.Update(location.WorkshopDetail);
                        }
                    }

                    if (toAdd.Count > 0)
                    {
                        await _unitOfWork.TourPlanLocationRepository.AddRangeAsync(toAdd);
                        foreach (var l in toAdd)
                            changes.Add($"Đã thêm địa điểm: {l.LocationId}");
                    }

                    foreach (var dto in dtos.Where(d => !d.TourPlanLocationId.HasValue))
                    {
                        if (dto.ActivityType != ActivityType.Workshop || !dto.WorkshopId.HasValue)
                            continue;

                        var locMeta = locations.First(l => l.Id == dto.LocationId);

                        if (locMeta.LocationType != LocationType.CraftVillage)
                            throw CustomExceptionFactory.CreateBadRequestError($"Địa điểm {dto.LocationId} không phải là làng nghề.");

                        if (!locToCv.TryGetValue(dto.LocationId, out var craftVillageId))
                            throw CustomExceptionFactory.CreateBadRequestError($"Không tìm thấy làng nghề cho địa điểm {dto.LocationId}.");

                        var wkOk = await _unitOfWork.WorkshopRepository.ActiveEntities
                            .AnyAsync(w => w.Id == dto.WorkshopId.Value && w.CraftVillageId == craftVillageId);
                        if (!wkOk)
                            throw CustomExceptionFactory.CreateBadRequestError("Workshop không thuộc làng nghề của địa điểm.");

                        if (dto.WorkshopTicketTypeId.HasValue)
                        {
                            var ttOk = await _unitOfWork.WorkshopTicketTypeRepository.ActiveEntities
                                .AnyAsync(tt => tt.Id == dto.WorkshopTicketTypeId.Value && tt.WorkshopId == dto.WorkshopId.Value);
                            if (!ttOk)
                                throw CustomExceptionFactory.CreateBadRequestError("Ticket type không thuộc workshop đã chọn.");
                        }

                        if (dto.WorkshopSessionRuleId.HasValue)
                        {
                            var srOk = await _unitOfWork.WorkshopSessionRuleRepository.ActiveEntities
                                .AnyAsync(sr => sr.Id == dto.WorkshopSessionRuleId.Value
                                             && sr.RecurringRule.WorkshopId == dto.WorkshopId.Value);
                            if (!srOk)
                                throw CustomExceptionFactory.CreateBadRequestError("Session rule không thuộc workshop đã chọn.");
                        }

                        var addedEntity = toAdd.First(a =>
                            a.LocationId == dto.LocationId &&
                            a.DayOrder == dto.DayOrder &&
                            a.StartTime == dto.StartTime &&
                            a.EndTime == dto.EndTime);

                        var plannedStart = dto.PreferredStartTime ?? dto.StartTime;
                        var plannedEnd = dto.PreferredEndTime ?? dto.EndTime;

                        var tplw = new TourPlanLocationWorkshop
                        {
                            Id = Guid.NewGuid(),
                            TourPlanLocationId = addedEntity.Id,
                            WorkshopId = dto.WorkshopId.Value,
                            WorkshopTicketTypeId = dto.WorkshopTicketTypeId,
                            WorkshopSessionRuleId = dto.WorkshopSessionRuleId,
                            PlannedStartTime = plannedStart,
                            PlannedEndTime = plannedEnd,
                            Notes = dto.Notes,
                            CreatedBy = userId.ToString(),
                            CreatedTime = DateTimeOffset.UtcNow,
                            LastUpdatedBy = userId.ToString(),
                            LastUpdatedTime = DateTimeOffset.UtcNow
                        };
                        await _unitOfWork.TourPlanLocationWorkshopRepository.AddAsync(tplw);
                    }

                    foreach (var dto in toUpdate)
                    {
                        var tourPlanLocation = existingLocations.First(l => l.Id == dto.TourPlanLocationId!.Value);

                        tourPlanLocation.LocationId = dto.LocationId;
                        tourPlanLocation.DayOrder = dto.DayOrder;
                        tourPlanLocation.StartTime = dto.StartTime;
                        tourPlanLocation.EndTime = dto.EndTime;
                        tourPlanLocation.Notes = dto.Notes;
                        tourPlanLocation.ActivityType = dto.ActivityType;
                        tourPlanLocation.TravelTimeFromPrev = dto.TravelTimeFromPrev;
                        tourPlanLocation.DistanceFromPrev = dto.DistanceFromPrev;
                        tourPlanLocation.EstimatedStartTime = dto.EstimatedStartTime;
                        tourPlanLocation.EstimatedEndTime = dto.EstimatedEndTime;
                        tourPlanLocation.LastUpdatedTime = DateTimeOffset.UtcNow;
                        _unitOfWork.TourPlanLocationRepository.Update(tourPlanLocation);
                        changes.Add($"Đã cập nhật địa điểm: {dto.LocationId}");

                        var locMeta = locations.First(l => l.Id == dto.LocationId);

                        if (dto.ActivityType != ActivityType.Workshop || !dto.WorkshopId.HasValue)
                        {
                            if (tourPlanLocation.WorkshopDetail != null && !tourPlanLocation.WorkshopDetail.IsDeleted)
                            {
                                tourPlanLocation.WorkshopDetail.IsDeleted = true;
                                tourPlanLocation.WorkshopDetail.LastUpdatedBy = userId.ToString();
                                tourPlanLocation.WorkshopDetail.LastUpdatedTime = DateTimeOffset.UtcNow;
                                _unitOfWork.TourPlanLocationWorkshopRepository.Update(tourPlanLocation.WorkshopDetail);
                            }
                        }
                        else
                        {
                            if (locMeta.LocationType != LocationType.CraftVillage)
                                throw CustomExceptionFactory.CreateBadRequestError($"Địa điểm {dto.LocationId} không phải là làng nghề.");

                            if (!locToCv.TryGetValue(dto.LocationId, out var craftVillageId))
                                throw CustomExceptionFactory.CreateBadRequestError($"Không tìm thấy làng nghề cho địa điểm {dto.LocationId}.");

                            var wkOk = await _unitOfWork.WorkshopRepository.ActiveEntities
                                .AnyAsync(w => w.Id == dto.WorkshopId.Value && w.CraftVillageId == craftVillageId);
                            if (!wkOk)
                                throw CustomExceptionFactory.CreateBadRequestError("Workshop không thuộc làng nghề của địa điểm.");

                            if (dto.WorkshopTicketTypeId.HasValue)
                            {
                                var ttOk = await _unitOfWork.WorkshopTicketTypeRepository.ActiveEntities
                                    .AnyAsync(tt => tt.Id == dto.WorkshopTicketTypeId.Value && tt.WorkshopId == dto.WorkshopId.Value);
                                if (!ttOk)
                                    throw CustomExceptionFactory.CreateBadRequestError("Ticket type không thuộc workshop đã chọn.");
                            }

                            if (dto.WorkshopSessionRuleId.HasValue)
                            {
                                var srOk = await _unitOfWork.WorkshopSessionRuleRepository.ActiveEntities
                                    .AnyAsync(sr => sr.Id == dto.WorkshopSessionRuleId.Value
                                                 && sr.RecurringRule.WorkshopId == dto.WorkshopId.Value);
                                if (!srOk)
                                    throw CustomExceptionFactory.CreateBadRequestError("Session rule không thuộc workshop đã chọn.");
                            }

                            var plannedStart = dto.PreferredStartTime ?? dto.StartTime;
                            var plannedEnd = dto.PreferredEndTime ?? dto.EndTime;

                            if (tourPlanLocation.WorkshopDetail == null || tourPlanLocation.WorkshopDetail.IsDeleted)
                            {
                                var tplw = new TourPlanLocationWorkshop
                                {
                                    Id = Guid.NewGuid(),
                                    TourPlanLocationId = tourPlanLocation.Id,
                                    WorkshopId = dto.WorkshopId.Value,
                                    WorkshopTicketTypeId = dto.WorkshopTicketTypeId,
                                    WorkshopSessionRuleId = dto.WorkshopSessionRuleId,
                                    PlannedStartTime = plannedStart,
                                    PlannedEndTime = plannedEnd,
                                    Notes = dto.Notes,
                                    CreatedBy = userId.ToString(),
                                    CreatedTime = DateTimeOffset.UtcNow,
                                    LastUpdatedBy = userId.ToString(),
                                    LastUpdatedTime = DateTimeOffset.UtcNow
                                };
                                await _unitOfWork.TourPlanLocationWorkshopRepository.AddAsync(tplw);
                            }
                            else
                            {
                                var d = tourPlanLocation.WorkshopDetail;
                                d.WorkshopId = dto.WorkshopId.Value;
                                d.WorkshopTicketTypeId = dto.WorkshopTicketTypeId;
                                d.WorkshopSessionRuleId = dto.WorkshopSessionRuleId;
                                d.PlannedStartTime = plannedStart;
                                d.PlannedEndTime = plannedEnd;
                                d.Notes = dto.Notes;
                                d.IsDeleted = false;
                                d.LastUpdatedBy = userId.ToString();
                                d.LastUpdatedTime = DateTimeOffset.UtcNow;
                                _unitOfWork.TourPlanLocationWorkshopRepository.Update(d);
                            }
                        }
                    }

                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        var changeSummary = string.Join("\n", changes);
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật Tour {tour.Name}",
                                $"Tour {tour.Name} đã có các thay đổi sau:\n{changeSummary}\nVui lòng kiểm tra chi tiết."
                            );
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var result = tour.TourPlanLocations
                .Where(l => !l.IsDeleted)
                .Select(l => new TourPlanLocationResponseDto
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    ActivityType = l.ActivityType,
                    ActivityTypeText = _enumService.GetEnumDisplayName<ActivityType>(l.ActivityType),
                    Notes = l.Notes,
                    TravelTimeFromPrev = l.TravelTimeFromPrev,
                    DistanceFromPrev = l.DistanceFromPrev,
                    EstimatedStartTime = l.EstimatedStartTime,
                    EstimatedEndTime = l.EstimatedEndTime,
                })
                .Concat(toAdd.Select(l => new TourPlanLocationResponseDto
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    ActivityType = l.ActivityType,
                    ActivityTypeText = _enumService.GetEnumDisplayName<ActivityType>(l.ActivityType),
                    Notes = l.Notes,
                    TravelTimeFromPrev = l.TravelTimeFromPrev,
                    DistanceFromPrev = l.DistanceFromPrev,
                    EstimatedStartTime = l.EstimatedStartTime,
                    EstimatedEndTime = l.EstimatedEndTime,
                }))
                .ToList();

            return result;
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<TourPlanLocationResponseDto>> GetLocationsAsync(Guid tourId)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var locations = await _unitOfWork.TourPlanLocationRepository
                .ActiveEntities
                .Where(l => l.TourId == tourId && !l.IsDeleted)
                .Select(l => new
                {
                    l.Id,
                    l.ActivityType,
                    l.LocationId,
                    l.DayOrder,
                    l.StartTime,
                    l.EndTime,
                    l.Notes,
                    l.TravelTimeFromPrev,
                    l.DistanceFromPrev,
                    l.EstimatedStartTime,
                    l.EstimatedEndTime
                })
                .ToListAsync();

            var result = locations.Select(l => new TourPlanLocationResponseDto
            {
                TourPlanLocationId = l.Id,
                ActivityType = l.ActivityType,
                ActivityTypeText = _enumService.GetEnumDisplayName<ActivityType>(l.ActivityType),
                LocationId = l.LocationId,
                DayOrder = l.DayOrder,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                Notes = l.Notes,
                TravelTimeFromPrev = l.TravelTimeFromPrev,
                DistanceFromPrev = l.DistanceFromPrev,
                EstimatedStartTime = l.EstimatedStartTime,
                EstimatedEndTime = l.EstimatedEndTime,
            }).ToList();

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

    #endregion

    #region TourSchedule

    public async Task<List<TourScheduleResponseDto>> CreateSchedulesAsync(Guid tourId, List<CreateTourScheduleDto> dtos)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
                throw CustomExceptionFactory.CreateForbiddenError();

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(p => p.WorkshopDetail)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings).ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã bị hủy.");

            var schedules = new List<TourSchedule>();
            var tourGuideSchedulesToAdd = new List<TourGuideSchedule>();
            var tswToAdd = new List<TourScheduleWorkshop>();

            foreach (var dto in dtos)
            {
                if (dto.DepartureDate < DateTime.UtcNow.Date)
                    throw CustomExceptionFactory.CreateBadRequestError($"Ngày khởi hành phải sau ngày hôm nay: {dto.DepartureDate:dd/MM/yyyy}.");

                if (dto.MaxParticipant <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");

                if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm: {dto.DepartureDate:dd/MM/yyyy}.");

                // Tạo TourSchedule (chưa save)
                var schedule = new TourSchedule
                {
                    TourId = tourId,
                    DepartureDate = dto.DepartureDate,
                    MaxParticipant = dto.MaxParticipant,
                    AdultPrice = dto.AdultPrice,
                    ChildrenPrice = dto.ChildrenPrice
                };
                schedules.Add(schedule);

                if (dto.TourGuideId != Guid.Empty)
                {
                    var tourGuide = await _unitOfWork.TourGuideRepository
                        .ActiveEntities
                        .Include(g => g.User)
                        .Include(g => g.TourGuideSchedules)
                        .FirstOrDefaultAsync(g => g.Id == dto.TourGuideId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

                    var startDate = dto.DepartureDate.Date;
                    var endDate = startDate.AddDays(tour.TotalDays);

                    var isConflict = tourGuide.TourGuideSchedules
                        .Where(s => !s.IsDeleted && s.BookingId != null)
                        .Any(s => s.Date.Date >= startDate && s.Date.Date < endDate);

                    if (isConflict)
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"TourGuide {tourGuide.User.FullName} không sẵn sàng trong khoảng {startDate:yyyy-MM-dd} đến {endDate.AddDays(-1):yyyy-MM-dd}."
                        );

                    for (int i = 0; i < tour.TotalDays; i++)
                    {
                        tourGuideSchedulesToAdd.Add(new TourGuideSchedule
                        {
                            TourSchedule = schedule,
                            TourGuideId = dto.TourGuideId,
                            Date = startDate.AddDays(i),
                            CreatedTime = DateTimeOffset.UtcNow,
                            LastUpdatedTime = DateTimeOffset.UtcNow
                        });
                    }
                }

                var planWorkshops = tour.TourPlanLocations
                    .Where(p => !p.IsDeleted && p.ActivityType == ActivityType.Workshop && p.WorkshopDetail != null && !p.WorkshopDetail.IsDeleted)
                    .ToList();

                foreach (var p in planWorkshops)
                {
                    var wd = p.WorkshopDetail!;
                    var plannedStart = wd.PlannedStartTime ?? p.StartTime;
                    var plannedEnd = wd.PlannedEndTime ?? p.EndTime;

                    var (winStart, winEnd) = ComputePlanWindow(dto.DepartureDate, p.DayOrder, plannedStart, plannedEnd);

                    var ws = await _unitOfWork.WorkshopScheduleRepository.ActiveEntities
                        .AsNoTracking()
                        .Where(s => s.WorkshopId == wd.WorkshopId && s.Status == ScheduleStatus.Active)
                        .Where(s =>
                            s.StartTime.Date >= winStart.Date.AddDays(-1) &&
                            s.StartTime.Date <= winEnd.Date.AddDays(1))
                        .Where(s => s.StartTime < winEnd && s.EndTime > winStart)
                        .OrderBy(s => s.StartTime)
                        .FirstOrDefaultAsync();

                    if (ws == null)
                    {
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Không tìm thấy lịch workshop phù hợp cho Ngày {p.DayOrder} (cửa sổ {winStart:dd/MM HH:mm}–{winEnd:dd/MM HH:mm}).");
                    }

                    if (ws.Capacity <= ws.CurrentBooked)
                    {
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Lịch workshop {ws.StartTime:dd/MM HH:mm} đã hết chỗ cho Ngày {p.DayOrder}.");
                    }

                    tswToAdd.Add(new TourScheduleWorkshop
                    {
                        TourSchedule = schedule,
                        TourPlanLocationId = p.Id,
                        WorkshopId = wd.WorkshopId,
                        WorkshopScheduleId = ws.Id,
                        WorkshopTicketTypeId = wd.WorkshopTicketTypeId,
                        CreatedBy = userId.ToString(),
                        CreatedTime = DateTimeOffset.UtcNow,
                        LastUpdatedBy = userId.ToString(),
                        LastUpdatedTime = DateTimeOffset.UtcNow
                    });
                }
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.TourScheduleRepository.AddRangeAsync(schedules);

                    if (tourGuideSchedulesToAdd.Any())
                        await _unitOfWork.TourGuideScheduleRepository.AddRangeAsync(tourGuideSchedulesToAdd);

                    if (tswToAdd.Any())
                        await _unitOfWork.TourScheduleWorkshopRepository.AddRangeAsync(tswToAdd);

                    tour.Status = TourStatus.Confirmed;
                    _unitOfWork.TourRepository.Update(tour);

                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật Tour {tour.Name}",
                                $"Tour {tour.Name} có lịch trình mới. Vui lòng kiểm tra chi tiết."
                            );
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return schedules.Select(s => new TourScheduleResponseDto
            {
                ScheduleId = s.Id,
                StartTime = s.DepartureDate,
                EndTime = s.DepartureDate.AddDays(tour.TotalDays - 1),
                MaxParticipant = s.MaxParticipant,
                CurrentBooked = s.CurrentBooked,
                AdultPrice = s.AdultPrice,
                ChildrenPrice = s.ChildrenPrice
            }).ToList();
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<TourScheduleResponseDto> UpdateScheduleAsync(Guid tourId, Guid scheduleId, CreateTourScheduleDto dto)
    {
        try
        {
            var userId = Guid.Parse(_userContextService.GetCurrentUserId());
            if (!_userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR))
                throw CustomExceptionFactory.CreateForbiddenError();

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourSchedules)
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(p => p.WorkshopDetail) // 👈 cần
                .Include(t => t.Bookings).ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã bị hủy.");

            var schedule = tour.TourSchedules.FirstOrDefault(s => s.Id == scheduleId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Schedule");

            // Validate
            if (dto.DepartureDate < DateTime.UtcNow.Date)
                throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải ở tương lai.");

            if (dto.MaxParticipant <= 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");

            if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm: {dto.DepartureDate:dd/MM/yyyy}.");

            if (dto.AdultPrice < 10000 || dto.ChildrenPrice < 10000)
                throw CustomExceptionFactory.CreateBadRequestError($"Giá không được nhỏ hơn 10000: {dto.DepartureDate:dd/MM/yyyy}.");

            if (tour.TourPlanLocations == null || !tour.TourPlanLocations.Any())
                throw CustomExceptionFactory.CreateBadRequestError("Tour chưa có lịch trình chi tiết, không thể tạo schedule.");

            var maxDayOrder = tour.TourPlanLocations.Any()
                ? tour.TourPlanLocations.Max(l => l.DayOrder)
                : 0;

            if (tour.TotalDays != maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError(
                    $"Không đủ lịch trình ({tour.TotalDays} ngày) để bao phủ Ngày thứ {maxDayOrder}."
                );

            // Tránh trùng ngày với schedule khác
            var otherScheduleDates = tour.TourSchedules
                .Where(s => s.Id != scheduleId)
                .Select(s => s.DepartureDate.Date)
                .Distinct()
                .ToList();
            if (otherScheduleDates.Contains(dto.DepartureDate.Date))
                throw CustomExceptionFactory.CreateBadRequestError(
                    $"Đã tồn tại lịch khởi hành vào ngày {dto.DepartureDate:dd/MM/yyyy} cho tour này."
                );

            // Cập nhật schedule cơ bản
            schedule.DepartureDate = dto.DepartureDate;
            schedule.MaxParticipant = dto.MaxParticipant;
            schedule.AdultPrice = dto.AdultPrice;
            schedule.ChildrenPrice = dto.ChildrenPrice;
            schedule.LastUpdatedTime = DateTimeOffset.UtcNow;

            // TourGuide
            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(g => g.User)
                .Include(g => g.TourGuideSchedules)
                .FirstOrDefaultAsync(g => g.Id == dto.TourGuideId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            var tourStart = dto.DepartureDate.Date;
            var tourEnd = dto.DepartureDate.AddDays(tour.TotalDays).Date;

            var conflictingSchedules = tourGuide.TourGuideSchedules
                .Where(s => !s.IsDeleted && s.BookingId != null)
                .Where(s => s.Date.Date >= tourStart && s.Date.Date < tourEnd)
                .Where(s => s.TourScheduleId != schedule.Id)
                .ToList();

            if (conflictingSchedules.Any())
            {
                throw CustomExceptionFactory.CreateBadRequestError(
                    $"TourGuide {tourGuide.User.FullName} không sẵn sàng trong khoảng {tourStart:yyyy-MM-dd} đến {tourEnd.AddDays(-1):yyyy-MM-dd}."
                );
            }

            var existingTgs = await _unitOfWork.TourGuideScheduleRepository
                .ActiveEntities
                .Where(s => s.TourScheduleId == schedule.Id && !s.IsDeleted)
                .ToListAsync();

            if (existingTgs.Any())
            {
                foreach (var s in existingTgs)
                {
                    s.IsDeleted = true;
                    s.LastUpdatedTime = DateTimeOffset.UtcNow;
                }
                _unitOfWork.TourGuideScheduleRepository.UpdateRange(existingTgs);
            }

            var newTgs = Enumerable.Range(0, tour.TotalDays)
                .Select(i => new TourGuideSchedule
                {
                    TourScheduleId = schedule.Id,
                    TourGuideId = dto.TourGuideId,
                    Date = tourStart.AddDays(i),
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                })
                .ToList();

            await _unitOfWork.TourGuideScheduleRepository.AddRangeAsync(newTgs);

            var existingTsw = await _unitOfWork.TourScheduleWorkshopRepository
                .ActiveEntities
                .Where(x => x.TourScheduleId == schedule.Id)
                .ToListAsync();

            var planWorkshops = tour.TourPlanLocations
                .Where(p => !p.IsDeleted && p.ActivityType == ActivityType.Workshop && p.WorkshopDetail != null && !p.WorkshopDetail.IsDeleted)
                .ToList();

            var planIds = planWorkshops.Select(p => p.Id).ToHashSet();
            var tswToRemove = existingTsw.Where(tsw => !planIds.Contains(tsw.TourPlanLocationId)).ToList();
            if (tswToRemove.Any())
                _unitOfWork.TourScheduleWorkshopRepository.RemoveRange(tswToRemove);

            var tswUpserts = new List<TourScheduleWorkshop>();

            foreach (var p in planWorkshops)
            {
                var wd = p.WorkshopDetail!;
                var plannedStart = wd.PlannedStartTime ?? p.StartTime;
                var plannedEnd = wd.PlannedEndTime ?? p.EndTime;

                var (winStart, winEnd) = ComputePlanWindow(dto.DepartureDate, p.DayOrder, plannedStart, plannedEnd);

                var ws = await _unitOfWork.WorkshopScheduleRepository.ActiveEntities
                    .AsNoTracking()
                    .Where(s => s.WorkshopId == wd.WorkshopId && s.Status == ScheduleStatus.Active)
                    .Where(s =>
                        s.StartTime.Date >= winStart.Date.AddDays(-1) &&
                        s.StartTime.Date <= winEnd.Date.AddDays(1))
                    .Where(s => s.StartTime < winEnd && s.EndTime > winStart)
                    .OrderBy(s => s.StartTime)
                    .FirstOrDefaultAsync();

                if (ws == null)
                {
                    throw CustomExceptionFactory.CreateBadRequestError(
                        $"Không tìm thấy lịch workshop phù hợp cho Ngày {p.DayOrder} (cửa sổ {winStart:dd/MM HH:mm}–{winEnd:dd/MM HH:mm}).");
                }

                if (ws.Capacity <= ws.CurrentBooked)
                    throw CustomExceptionFactory.CreateBadRequestError(
                        $"Lịch workshop {ws.StartTime:dd/MM HH:mm} đã hết chỗ cho Ngày {p.DayOrder}.");

                var existing = existingTsw.FirstOrDefault(x => x.TourPlanLocationId == p.Id);
                if (existing == null)
                {
                    tswUpserts.Add(new TourScheduleWorkshop
                    {
                        TourScheduleId = schedule.Id,
                        TourPlanLocationId = p.Id,
                        WorkshopId = wd.WorkshopId,
                        WorkshopScheduleId = ws.Id,
                        WorkshopTicketTypeId = wd.WorkshopTicketTypeId,
                        CreatedBy = userId.ToString(),
                        CreatedTime = DateTimeOffset.UtcNow,
                        LastUpdatedBy = userId.ToString(),
                        LastUpdatedTime = DateTimeOffset.UtcNow
                    });
                }
                else
                {
                    existing.WorkshopId = wd.WorkshopId;
                    existing.WorkshopScheduleId = ws.Id;
                    existing.WorkshopTicketTypeId = wd.WorkshopTicketTypeId;
                    existing.LastUpdatedBy = userId.ToString();
                    existing.LastUpdatedTime = DateTimeOffset.UtcNow;
                    _unitOfWork.TourScheduleWorkshopRepository.Update(existing);
                }
            }

            if (tswUpserts.Any())
                await _unitOfWork.TourScheduleWorkshopRepository.AddRangeAsync(tswUpserts);

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                tour.Status = TourStatus.Confirmed;
                _unitOfWork.TourRepository.Update(tour);
                await _unitOfWork.SaveAsync();

                var confirmedBookings = tour.Bookings
                    .Where(b => b.Status == BookingStatus.Confirmed)
                    .ToList();

                foreach (var booking in confirmedBookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User.Email },
                        $"Cập nhật Tour {tour.Name}",
                        $"Lịch trình ngày {schedule.DepartureDate:dd/MM/yyyy} của tour {tour.Name} đã được cập nhật. Vui lòng kiểm tra chi tiết."
                    );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new TourScheduleResponseDto
            {
                ScheduleId = schedule.Id,
                StartTime = schedule.DepartureDate,
                EndTime = schedule.DepartureDate.AddDays(tour.TotalDays - 1),
                MaxParticipant = schedule.MaxParticipant,
                CurrentBooked = schedule.CurrentBooked,
                AdultPrice = schedule.AdultPrice,
                ChildrenPrice = schedule.ChildrenPrice
            };
        }
        catch (CustomException) { throw; }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task DeleteScheduleAsync(Guid tourId, Guid scheduleId)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var schedule = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.TourId == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Schedule");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa lịch trình đã bị hủy.");

            if (schedule.CurrentBooked > 0)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa lịch trình đã có người đặt.");

            var remainingSchedules = tour.TourSchedules.Where(s => s.Id != scheduleId).Select(s => s.DepartureDate.Date).Distinct().ToList();
            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            // if (remainingSchedules.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không đủ số ngày lịch trình ({remainingSchedules.Count}) để bao phủ các ngày trong kế hoạch ({maxDayOrder}).");

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    schedule.IsDeleted = true;
                    _unitOfWork.TourScheduleRepository.Update(schedule);
                    await _unitOfWork.SaveAsync();

                    bool hasActiveSchedules = tour.TourSchedules.Any(s => !s.IsDeleted && s.Id != scheduleId);
                    if (!hasActiveSchedules)
                    {
                        tour.Status = TourStatus.Draft;
                        _unitOfWork.TourRepository.Update(tour);
                        await _unitOfWork.SaveAsync();
                    }

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật Tour {tour.Name}",
                                $"Lịch trình ngày {schedule.DepartureDate:dd/MM/yyyy} của tour {tour.Name} đã bị xóa. Vui lòng kiểm tra chi tiết."
                            );
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
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

    #region TourGuide

    // public async Task AddTourGuidesAsync(Guid tourId, List<Guid> guideIds)
    // {
    //     try
    //     {
    //         var tour = await _unitOfWork.TourRepository
    //             .ActiveEntities
    //             .Include(t => t.TourSchedules)
    //             .Include(t => t.TourGuideSchedules)
    //             .Include(t => t.Bookings)
    //             .ThenInclude(b => b.User)
    //             .FirstOrDefaultAsync(t => t.Id == tourId)
    //             ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

    //         if (tour.Status == TourStatus.Cancelled)
    //             throw CustomExceptionFactory.CreateBadRequestError("Không thể chỉ định hướng dẫn viên cho chuyến tham quan đã hủy.");

    //         // Kiểm tra TourGuide tồn tại và hợp lệ
    //         var validGuides = await _unitOfWork.TourGuideRepository
    //             .ActiveEntities
    //             .Include(tg => tg.User)
    //             .Include(tg => tg.TourGuideSchedules)
    //             .Where(tg => guideIds.Contains(tg.Id))
    //             .ToListAsync();
    //         var invalidGuideIds = guideIds.Except(validGuides.Select(tg => tg.Id)).ToList();
    //         if (invalidGuideIds.Any())
    //             throw CustomExceptionFactory.CreateBadRequestError($"TourGuide IDs không hợp lệ: {string.Join(", ", invalidGuideIds)}");

    //         // Kiểm tra lịch trống của TourGuide
    //         var tourSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
    //         if (tourSchedules.Any())
    //         {
    //             var tourStart = tourSchedules.Min(s => s.DepartureDate.Date);
    //             var tourEnd = tourSchedules.Max(s => s.DepartureDate.AddDays(s.TotalDays).Date);
    //             foreach (var guide in validGuides)
    //             {
    //                 var conflictingSchedules = guide.TourGuideSchedules
    //                     .Where(s => !s.IsDeleted)
    //                     .Where(s => s.BookingId != null) // Chỉ kiểm tra lịch đã gắn với booking
    //                     .Where(s => s.Date.Date >= tourStart && s.Date.Date <= tourEnd)
    //                     .ToList();
    //                 if (conflictingSchedules.Any())
    //                     throw CustomExceptionFactory.CreateBadRequestError($"TourGuide {guide.User.FullName} không sẵn sàng trong khoảng {tourStart:yyyy-MM-dd} tới {tourEnd:yyyy-MM-dd}.");
    //             }
    //         }

    //         // check tourGuide 
    //         var existingGuideIds = tour.TourGuideSchedules.Where(tg => !tg.IsDeleted).Select(tg => tg.GuideId).ToList();
    //         var newGuideIds = guideIds.Except(existingGuideIds).ToList();
    //         if (!newGuideIds.Any())
    //             return; // không có TourGuide mới để thêm

    //         // tour Guide Mapping
    //         var newMappings = newGuideIds.Select(guideId => new TourGuideMapping
    //         {
    //             TourId = tourId,
    //             GuideId = guideId,
    //             CreatedTime = DateTimeOffset.UtcNow,
    //             LastUpdatedTime = DateTimeOffset.UtcNow
    //         }).ToList();

    //         var changes = new List<string>();
    //         using (var transaction = await _unitOfWork.BeginTransactionAsync())
    //         {
    //             try
    //             {
    //                 await _unitOfWork.TourGuideMappingRepository.AddRangeAsync(newMappings);
    //                 foreach (var mapping in newMappings)
    //                 {
    //                     var guide = validGuides.First(g => g.Id == mapping.GuideId);
    //                     changes.Add($"Added TourGuide: {guide.User.FullName}");
    //                 }

    //                 await _unitOfWork.SaveAsync();

    //                 if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
    //                 {
    //                     var changeSummary = string.Join("\n", changes);
    //                     foreach (var booking in tour.Bookings)
    //                     {
    //                         await _emailService.SendEmailAsync(
    //                             new[] { booking.User.Email },
    //                             $"Cập nhật thông tin Tour {tour.Name}",
    //                             $"Tour {tour.Name} đã có các thay đổi sau:\n{changeSummary}\nVui lòng kiểm tra chi tiết."
    //                         );
    //                     }
    //                 }

    //                 await transaction.CommitAsync();
    //             }
    //             catch
    //             {
    //                 await transaction.RollbackAsync();
    //                 throw;
    //             }
    //         }
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    // }

    public async Task AddTourGuideToScheduleAsync(Guid tourScheduleId, Guid guideId)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tourSchedule = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Include(ts => ts.Tour)
                    .ThenInclude(t => t.Bookings)
                        .ThenInclude(b => b.User)
                .Include(ts => ts.TourGuideSchedules)
                .FirstOrDefaultAsync(ts => ts.Id == tourScheduleId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourSchedule");

            if (tourSchedule.Tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể chỉ định hướng dẫn viên cho tour đã bị hủy.");

            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(g => g.User)
                .Include(g => g.TourGuideSchedules)
                .FirstOrDefaultAsync(g => g.Id == guideId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuide");

            // Kiểm tra lịch bị trùng
            var tourStart = tourSchedule.DepartureDate.Date;
            var tourEnd = tourSchedule.DepartureDate.AddDays(tourSchedule.Tour.TotalDays).Date;

            var conflictingSchedules = tourGuide.TourGuideSchedules
                .Where(s => !s.IsDeleted)
                .Where(s => s.BookingId != null)
                .Where(s => s.Date.Date >= tourStart && s.Date.Date <= tourEnd)
                .ToList();

            if (conflictingSchedules.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"TourGuide {tourGuide.User.FullName} không sẵn sàng trong khoảng {tourStart:yyyy-MM-dd} đến {tourEnd:yyyy-MM-dd}.");

            // Check đã gán rồi chưa
            var isAlreadyAssigned = tourSchedule.TourGuideSchedules
                .Any(m => !m.IsDeleted && m.TourGuideId == guideId);

            if (isAlreadyAssigned)
                throw CustomExceptionFactory.CreateBadRequestError("Hướng dẫn viên này đã được gán cho lịch trình.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var schedules = new List<TourGuideSchedule>();

                for (var date = tourStart; date < tourEnd; date = date.AddDays(1))
                {
                    var schedule = new TourGuideSchedule
                    {
                        TourScheduleId = tourScheduleId,
                        TourGuideId = guideId,
                        Date = date,
                        CreatedTime = DateTimeOffset.UtcNow,
                        LastUpdatedTime = DateTimeOffset.UtcNow
                    };

                    schedules.Add(schedule);
                }
                await _unitOfWork.TourGuideScheduleRepository.AddRangeAsync(schedules);
                await _unitOfWork.SaveAsync();

                // Gửi email nếu tour đã có người đặt
                // if (tourSchedule.Tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                // {
                //     var changeMsg = $"TourGuide {tourGuide.User.FullName} đã được gán cho lịch trình ngày {tourSchedule.DepartureDate:yyyy-MM-dd}.";

                //     foreach (var booking in tourSchedule.Tour.Bookings)
                //     {
                //         await _emailService.SendEmailAsync(
                //             new[] { booking.User.Email },
                //             $"Cập nhật thông tin Tour {tourSchedule.Tour.Name}",
                //             $"Tour {tourSchedule.Tour.Name} đã được cập nhật:\n{changeMsg}"
                //         );
                //     }
                // }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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

    public async Task RemoveTourGuideAsync(Guid tourScheduleId, Guid guideId)
    {
        try
        {
            Guid userId = Guid.Parse(_userContextService.GetCurrentUserId());

            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tourSchedule = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Include(ts => ts.Tour)
                    .ThenInclude(t => t.Bookings)
                        .ThenInclude(b => b.User)
                .Include(ts => ts.TourGuideSchedules)
                .FirstOrDefaultAsync(ts => ts.Id == tourScheduleId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourSchedule");

            if (tourSchedule.Tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa tour guide khỏi lịch trình đã bị hủy.");

            var mapping = tourSchedule.TourGuideSchedules
                .FirstOrDefault(tg => tg.TourGuideId == guideId && !tg.IsDeleted)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuide không được chỉ định cho lịch trình này.");

            var guide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(tg => tg.User)
                .FirstOrDefaultAsync(tg => tg.Id == guideId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuide");

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    mapping.IsDeleted = true;
                    mapping.LastUpdatedTime = DateTimeOffset.UtcNow;

                    await _unitOfWork.SaveAsync();

                    // Gửi email nếu có khách đặt tour
                    if (tourSchedule.Tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tourSchedule.Tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật thông tin Tour {tourSchedule.Tour.Name}",
                                $"Tour {tourSchedule.Tour.Name} - Lịch trình ngày {tourSchedule.DepartureDate:dd/MM/yyyy} đã xóa hướng dẫn viên: {guide.User.FullName}."
                            );
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
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

    private async Task<string> GetLocationImageUrl(Guid locationId)
    {
        // Note: Nếu có isThumbnail thì lấy ảnh thumbnail, nếu không thì lấy ảnh đầu tiên
        var locationMedia = await _unitOfWork.LocationMediaRepository.ActiveEntities
            .FirstOrDefaultAsync(l => l.LocationId == locationId);

        return locationMedia != null ? locationMedia.MediaUrl ?? string.Empty : string.Empty;
    }

    private List<string> GetTourChanges(Tour tour, UpdateTourDto dto)
    {
        var changes = new List<string>();
        if (tour.Name != dto.Name)
            changes.Add($"Tên thay đổi từ '{tour.Name}' → '{dto.Name}'");
        // if (tour.Description != dto.Description)
        //     changes.Add($"Mô tả thay đổi từ '{tour.Description}' → '{dto.Description}'");
        // if (tour.Content != dto.Content)
        //     changes.Add($"Nội dung thay đổi từ '{tour.Content}' → '{dto.Content}'");
        if (tour.TotalDays != dto.TotalDays)
            changes.Add($"Số ngày thay đổi từ '{tour.TotalDays}' → '{dto.TotalDays}'");
        return changes;
    }

    private async Task<List<TourDayDetail>> BuildDayDetails(Tour tour)
    {
        var groupedByDay = tour.TourPlanLocations
            .Where(l => !l.IsDeleted)
            .GroupBy(l => l.DayOrder)
            .OrderBy(g => g.Key);

        var dayDetails = new List<TourDayDetail>();

        foreach (var group in groupedByDay)
        {
            var activities = new List<TourActivity>();

            foreach (var l in group)
            {
                var imageUrl = await GetLocationImageUrl(l.LocationId);

                var activity = new TourActivity
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    Type = "Location",
                    Name = l.Location?.Name ?? "Unknown",
                    Description = l.Location?.Description,
                    Address = l.Location?.Address,
                    ActivityType = l.ActivityType,
                    ActivityTypeText = _enumService.GetEnumDisplayName<ActivityType>(l.ActivityType),
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    StartTimeFormatted = l.StartTime.ToString(@"hh\:mm"),
                    EndTimeFormatted = l.EndTime.ToString(@"hh\:mm"),
                    Duration = $"{(l.EndTime - l.StartTime).TotalMinutes} phút",
                    Notes = l.Notes,
                    ImageUrl = imageUrl,
                    TravelTimeFromPrev = l.TravelTimeFromPrev,
                    DistanceFromPrev = l.DistanceFromPrev,
                    EstimatedStartTime = l.EstimatedStartTime,
                    EstimatedEndTime = l.EstimatedEndTime,
                };

                activities.Add(activity);
            }

            var orderedActivities = activities
                .OrderBy(a => a.StartTime ?? TimeSpan.Zero)
                .ThenBy(a => a.DayOrder)
                .ToList();

            dayDetails.Add(new TourDayDetail
            {
                DayNumber = group.Key,
                Activities = orderedActivities
            });
        }

        return dayDetails;
    }

    private List<TourGuideDataModel> GetTourGuidesInfo(Tour tour)
    {
        return tour.TourSchedules
            .Where(ts => !ts.IsDeleted)
            .SelectMany(ts => ts.TourGuideSchedules)
            .Where(tg => !tg.IsDeleted && tg.TourGuide != null)
            .Select(tg => new TourGuideDataModel
            {
                Id = tg.TourGuide.Id,
                UserName = tg.TourGuide.User.FullName,
                Email = tg.TourGuide.User.Email,
                Sex = tg.TourGuide.User.Sex,
                Address = tg.TourGuide.User.Address,
                AverageRating = tg.TourGuide.Rating,
                Price = tg.TourGuide.Price,
                Introduction = tg.TourGuide.Introduction,
                AvatarUrl = tg.TourGuide.User.AvatarUrl,
            })
            .DistinctBy(g => g.Id)
            .ToList();
    }

    private List<TourGuideDataModel> GetTourGuidesInfo(List<TourSchedule> tourSchedules)
    {
        return tourSchedules
            .Where(ts => !ts.IsDeleted)
            .SelectMany(ts => ts.TourGuideSchedules)
            .Where(tg => !tg.IsDeleted && tg.TourGuide != null && tg.TourGuide.User != null)
            .Select(tg => new TourGuideDataModel
            {
                Id = tg.TourGuide.Id,
                UserName = tg.TourGuide.User.FullName,
                Email = tg.TourGuide.User.Email,
                Sex = tg.TourGuide.User.Sex,
                Address = tg.TourGuide.User.Address,
                AverageRating = tg.TourGuide.Rating,
                Price = tg.TourGuide.Price,
                Introduction = tg.TourGuide.Introduction,
                AvatarUrl = tg.TourGuide.User.AvatarUrl,
            })
            .DistinctBy(g => g.Id)
            .ToList();
    }

    // Location
    private void ValidateDto(UpdateTourPlanLocationDto dto, List<TourPlanLocation> existingLocations)
    {
        if (dto.TourPlanLocationId.HasValue && !existingLocations.Any(l => l.Id == dto.TourPlanLocationId && !l.IsDeleted))
            throw CustomExceptionFactory.CreateBadRequestError($"Location with ID {dto.TourPlanLocationId} not found or already deleted.");
        if (dto.LocationId == Guid.Empty)
            throw CustomExceptionFactory.CreateBadRequestError($"Destination ID is required for DayOrder {dto.DayOrder}.");
        if (dto.DayOrder < 1)
            throw CustomExceptionFactory.CreateBadRequestError($"DayOrder must be positive for destination {dto.LocationId}.");
        if (dto.StartTime >= dto.EndTime)
            throw CustomExceptionFactory.CreateBadRequestError($"End time must be after start time for destination {dto.LocationId}.");
    }

    public async Task<PagedResult<TourResponseDto>> GetPagedToursByGuideEmailAsync(string email, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Email cannot be empty.");
            }

            email = email.Trim().ToLower();

            var toursQuery = _unitOfWork.TourRepository.ActiveEntities
                .Include(t => t.TourSchedules)
                    .ThenInclude(s => s.TourGuideSchedules)
                        .ThenInclude(m => m.TourGuide)
                            .ThenInclude(g => g.User)
                .Where(t => t.TourSchedules
                    .Any(s => !s.IsDeleted &&
                              s.TourGuideSchedules.Any(m =>
                                  !m.IsDeleted &&
                                  m.TourGuide != null &&
                                  m.TourGuide.User.Email.ToLower() == email
                              )
                    )
                );

            var totalCount = await toursQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<TourResponseDto>
                {
                    Items = new List<TourResponseDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var tourItems = await toursQuery
                .OrderBy(t => t.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var tourResponses = new List<TourResponseDto>();

            foreach (var tour in tourItems)
            {
                var activeSchedules = tour.TourSchedules?.Where(s => !s.IsDeleted).ToList() ?? new List<TourSchedule>();
                var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
                var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
                var finalPrice = adultPrice;

                tourResponses.Add(new TourResponseDto
                {
                    TourId = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Content = tour.Content,
                    TransportType = tour.TransportType,
                    StayInfo = tour.StayInfo,
                    PickupAddress = tour.PickupAddress,
                    TotalDays = tour.TotalDays,
                    TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                    TourType = tour.TourType,
                    TourTypeText = _enumService.GetEnumDisplayName<TourType>(tour.TourType),
                    AdultPrice = adultPrice,
                    ChildrenPrice = childrenPrice,
                    FinalPrice = finalPrice,
                    Status = tour.Status
                });
            }

            return new PagedResult<TourResponseDto>
            {
                Items = tourResponses,
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

    #region Tour Media
    public async Task<bool> DeleteTourMediaAsync(Guid tourMediaId)
    {
        try
        {
            var tourMedia = await _unitOfWork.TourMediaRepository
                .ActiveEntities
                .FirstOrDefaultAsync(tm => tm.Id == tourMediaId);

            if (tourMedia == null)
            {
                return false;
            }

            _unitOfWork.TourMediaRepository.Remove(tourMedia);
            await _unitOfWork.SaveAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TourMedia>> AddTourMediasAsync(Guid tourId, List<TourMediaCreateDto> createDtos)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == tourId);

            if (tour == null)
            {
                return new List<TourMedia>();
            }

            var newMedias = createDtos.Select(dto => new TourMedia
            {
                Id = Guid.NewGuid(),
                TourId = tourId,
                MediaUrl = dto.MediaUrl,
                FileName = dto.FileName,
                FileType = dto.FileType,
                SizeInBytes = dto.SizeInBytes,
                IsThumbnail = dto.IsThumbnail,
                CreatedTime = DateTime.UtcNow,
                LastUpdatedTime = DateTime.UtcNow
            }).ToList();

            await _unitOfWork.TourMediaRepository.AddRangeAsync(newMedias);
            await _unitOfWork.SaveAsync();
            return newMedias;
        }
        catch (Exception)
        {
            return new List<TourMedia>();
        }
    }

    public async Task<TourMediaResponse> UploadMediaAsync(
        Guid id,
        UploadMediasDto? uploadMediasDto,
        string? thumbnailSelected,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var existingTour = await _unitOfWork.TourRepository.GetByIdAsync(id, cancellationToken);
            if (existingTour == null || existingTour.IsDeleted)
            {
                throw CustomExceptionFactory.CreateNotFoundError("location");
            }

            if (uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("images");
            }

            var allMedia = _unitOfWork.TourMediaRepository.Entities
                .Where(dm => dm.TourId == existingTour.Id).ToList();

            // Nếu không có ảnh mới & không có thumbnailSelected => Chỉ cập nhật thông tin location
            if ((uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0) && string.IsNullOrEmpty(thumbnailSelected))
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new TourMediaResponse
                {
                    TourId = existingTour.Id,
                    TourName = existingTour.Name,
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
                    _unitOfWork.TourMediaRepository.Update(media);
                }
                isThumbnailUpdated = true; // Đánh dấu đã cập nhật thumbnail
            }

            // Nếu không có ảnh mới nhưng có thumbnailSelected là ảnh cũ -> Dừng ở đây
            if (uploadMediasDto.Files == null || uploadMediasDto.Files.Count == 0)
            {
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync(cancellationToken);
                return new TourMediaResponse
                {
                    TourId = existingTour.Id,
                    TourName = existingTour.Name,
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

                var newTourMedia = new TourMedia
                {
                    FileName = imageUpload.FileName,
                    FileType = imageUpload.ContentType,
                    TourId = existingTour.Id,
                    MediaUrl = imageUrls[i],
                    SizeInBytes = imageUpload.Length,
                    IsThumbnail = isThumbnail,
                    CreatedBy = currentUserId,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                };

                await _unitOfWork.TourMediaRepository.AddAsync(newTourMedia);
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
                        _unitOfWork.TourMediaRepository.Update(media);
                    }
                    isThumbnailUpdated = true;
                }
            }

            // Nếu chưa có ảnh nào được chọn làm thumbnail, đặt ảnh mới đầu tiên làm thumbnail
            if (!isThumbnailUpdated && mediaResponses.Count > 0)
            {
                var firstMedia = mediaResponses.First();
                var firstMediaEntity = await _unitOfWork.TourMediaRepository.ActiveEntities
                    .FirstOrDefaultAsync(m => m.MediaUrl == firstMedia.MediaUrl);

                if (firstMediaEntity != null)
                {
                    firstMediaEntity.IsThumbnail = true;
                    _unitOfWork.TourMediaRepository.Update(firstMediaEntity);
                }
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new TourMediaResponse
            {
                TourId = existingTour.Id,
                TourName = existingTour.Name,
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

    public async Task<List<MediaResponse>> GetMediaWithoutVideoByIdAsync(Guid tourId, CancellationToken cancellationToken)
    {
        var locationMedias = await _unitOfWork.TourMediaRepository
            .ActiveEntities
            .Where(em => em.TourId == tourId && !em.IsDeleted)
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

    private async Task UpdateTourMediasAsync(Guid tourId, List<MediaDto> mediaDtos, CancellationToken cancellationToken = default)
    {
        try
        {
            if (mediaDtos == null || !mediaDtos.Any())
                return;

            var existingMedias = await _unitOfWork.TourMediaRepository.ActiveEntities
                .Where(m => m.TourId == tourId)
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
                        _unitOfWork.TourMediaRepository.Update(existingMedia);
                    }
                }
                else
                {
                    var newMedia = new TourMedia
                    {
                        TourId = tourId,
                        MediaUrl = mediaDto.MediaUrl,
                        FileName = fileName,
                        FileType = fileType,
                        SizeInBytes = 0,
                        IsThumbnail = mediaDto.IsThumbnail,
                        CreatedTime = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.TourMediaRepository.AddAsync(newMedia);
                }
            }

            var allMediasAfterUpdate = await _unitOfWork.TourMediaRepository.ActiveEntities
                .Where(m => m.TourId == tourId)
                .ToListAsync(cancellationToken);

            var thumbnails = allMediasAfterUpdate.Where(m => m.IsThumbnail).ToList();

            if (!thumbnails.Any())
            {
                var first = allMediasAfterUpdate.FirstOrDefault();
                if (first != null)
                {
                    first.IsThumbnail = true;
                    _unitOfWork.TourMediaRepository.Update(first);
                }
            }
            else if (thumbnails.Count > 1)
            {
                foreach (var extraThumb in thumbnails.Skip(1))
                {
                    extraThumb.IsThumbnail = false;
                    _unitOfWork.TourMediaRepository.Update(extraThumb);
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

    private static (DateTime Start, DateTime End) ComputePlanWindow(
    DateTime departureDate, int dayOrder, TimeSpan start, TimeSpan end)
    {
        var dayDate = departureDate.Date.AddDays(dayOrder - 1);
        var s = dayDate.Add(start);
        var e = dayDate.Add(end);
        if (end <= start) e = e.AddDays(1);
        return (s, e);
    }

    private static bool Overlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        => aStart < bEnd && bStart < aEnd;


    #endregion
}