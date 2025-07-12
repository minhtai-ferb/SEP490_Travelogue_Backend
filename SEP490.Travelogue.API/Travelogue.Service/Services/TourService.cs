using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.Commons.Implementations;

namespace Travelogue.Service.Services;

public interface ITourService
{
    Task<List<TourResponseDto>> GetAllToursAsync();
    Task<TourResponseDto> CreateTourAsync(CreateTourDto dto);
    Task<TourResponseDto> UpdateTourAsync(Guid tourId, UpdateTourDto dto);
    Task<TourResponseDto> ConfirmTourAsync(Guid tourId, ConfirmTourDto dto);
    Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId);
    Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId, Guid? scheduleId = null);
    Task<List<TourPlanLocationResponseDto>> UpdateLocationsAsync(Guid tourId, List<UpdateTourPlanLocationDto> dtos);
    Task<List<TourPlanLocationResponseDto>> GetLocationsAsync(Guid tourId);
    Task<List<TourScheduleResponseDto>> CreateSchedulesAsync(Guid tourId, List<CreateTourScheduleDto> dtos);
    Task<List<TourScheduleResponseDto>> GetSchedulesAsync(Guid tourId);
    Task<TourScheduleResponseDto> UpdateScheduleAsync(Guid tourId, Guid scheduleId, CreateTourScheduleDto dto);
    Task DeleteScheduleAsync(Guid tourId, Guid scheduleId);

    Task AddTourGuidesAsync(Guid tourId, List<Guid> guideIds);
    Task RemoveTourGuideAsync(Guid tourId, Guid guideId);
}

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public TourService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<TourResponseDto> CreateTourAsync(CreateTourDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên tour là bắt buộc.");
            if (dto.TourTypeId == Guid.Empty)
                throw CustomExceptionFactory.CreateBadRequestError("Mã loại tour là bắt buộc.");
            if (dto.TotalDays <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Số ngày phải lớn hơn 0.");

            var tourType = await _unitOfWork.TourTypeRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == dto.TourTypeId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour type");

            var tour = new Tour
            {
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                TotalDays = dto.TotalDays,
                TourTypeId = dto.TourTypeId,
                Status = TourStatus.Draft
            };

            await _unitOfWork.TourRepository.AddAsync(tour);
            await _unitOfWork.SaveAsync();

            return new TourResponseDto
            {
                TourId = tour.Id,
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                TotalDays = dto.TotalDays,
                TourTypeId = dto.TourTypeId,
                TourTypeText = tourType.Name,
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

    public async Task<TourResponseDto> UpdateTourAsync(Guid tourId, UpdateTourDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên tour là bắt buộc.");
            if (dto.TourTypeId == Guid.Empty)
                throw CustomExceptionFactory.CreateBadRequestError("Mã loại tour là bắt buộc.");
            if (dto.TotalDays <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Số ngày phải lớn hơn 0.");

            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .Include(t => t.TourPlanLocations)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var tourType = await _unitOfWork.TourTypeRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == dto.TourTypeId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour type");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã bị hủy.");

            // Validate TotalDays
            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            if (dto.TotalDays < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({dto.TotalDays}) không được nhỏ hơn ngày lớn nhất đã lên kế hoạch ({maxDayOrder}).");

            // Track changes for notification
            var changes = GetTourChanges(tour, dto);

            // Update tour properties
            tour.Name = dto.Name;
            tour.Description = dto.Description;
            tour.Content = dto.Content;
            tour.TourTypeId = dto.TourTypeId;
            tour.TotalDays = dto.TotalDays;
            tour.LastUpdatedTime = DateTimeOffset.UtcNow;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed) && changes.Any())
                    {
                        var changeSummary = string.Join("\n", changes);
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật thông tin Tour {tour.Name}",
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

            return new TourResponseDto
            {
                TourId = tour.Id,
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                TourTypeId = dto.TourTypeId,
                TotalDays = dto.TotalDays,
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

    public async Task<TourResponseDto> ConfirmTourAsync(Guid tourId, ConfirmTourDto dto)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            var scheduleDates = tour.TourSchedules.Select(s => s.DepartureDate.Date).Distinct().Count();
            if (scheduleDates < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Not enough schedules ({scheduleDates}) to cover DayOrder ({maxDayOrder}).");

            if (tour.Status == TourStatus.Confirmed)
                throw CustomExceptionFactory.CreateBadRequestError("Tour is already confirmed.");

            tour.Status = TourStatus.Confirmed;
            tour.LastUpdatedTime = DateTimeOffset.UtcNow;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Tour {tour.Name} Đã Được Xác Nhận",
                                $"Tour {tour.Name} đã được xác nhận và sẵn sàng cho bạn tham gia. Vui lòng kiểm tra chi tiết."
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

    public async Task<List<TourResponseDto>> GetAllToursAsync()
    {
        try
        {
            var tours = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourType)
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(l => l.Location)
                .Include(t => t.TourSchedules)
                .Include(t => t.TourGuideMappings)
                    .ThenInclude(tg => tg.TourGuide)
                        .ThenInclude(tg => tg.User)
                .Include(t => t.PromotionApplicables)
                    .ThenInclude(p => p.Promotion)
                .ToListAsync();

            if (!tours.Any())
            {
                throw CustomExceptionFactory.CreateNotFoundError("tours");
            }

            var tourResponses = new List<TourResponseDto>();

            foreach (var tour in tours)
            {
                // tính giá thấp nhất
                var activeSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
                var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
                var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
                var finalPrice = adultPrice;

                var tourGuide = GetTourGuideInfo(tour);

                var groupedLocations = tour.TourPlanLocations
                    .Where(l => !l.IsDeleted)
                    .GroupBy(l => l.DayOrder)
                    .OrderBy(g => g.Key)
                    .ToList();

                var dayDetails = BuildDayDetails(tour);

                tourResponses.Add(new TourResponseDto
                {
                    TourId = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Content = tour.Content,
                    TotalDays = tour.TotalDays,
                    TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                    TourTypeId = tour.TourTypeId,
                    TourTypeText = tour.TourType?.Name ?? "Unknown",
                    AdultPrice = adultPrice,
                    ChildrenPrice = childrenPrice,
                    FinalPrice = finalPrice,
                    Status = tour.Status,
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
                .Include(t => t.TourType)
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(l => l.Location)
                .Include(t => t.TourSchedules)
                .Include(t => t.TourGuideMappings)
                    .ThenInclude(tg => tg.TourGuide)
                        .ThenInclude(tg => tg.User)
                .Include(t => t.PromotionApplicables)
                    .ThenInclude(p => p.Promotion)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            // tính giá thấp nhất
            var activeSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
            var adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
            var childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;

            // tính khuyến mãi
            var activePromotions = tour.PromotionApplicables
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

            var tourGuide = GetTourGuideInfo(tour);

            var groupedLocations = tour.TourPlanLocations
                .Where(l => !l.IsDeleted)
                .GroupBy(l => l.DayOrder)
                .OrderBy(g => g.Key)
                .ToList();

            var dayDetails = BuildDayDetails(tour);

            return new TourDetailsResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourTypeId = tour.TourTypeId,
                TourTypeName = tour.TourType?.Name ?? "Unknown",
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                IsDiscount = isDiscount,
                Status = tour.Status,
                // Locations = tour.TourPlanLocations
                //     .Where(l => !l.IsDeleted)
                //     .Select(l => new TourPlanLocationResponseDto
                //     {
                //         TourPlanLocationId = l.Id,
                //         LocationId = l.LocationId,
                //         DayOrder = l.DayOrder,
                //         StartTime = l.StartTime,
                //         EndTime = l.EndTime,
                //         Notes = l.Notes
                //     }).ToList(),
                Schedules = activeSchedules
                    .Select(s => new TourScheduleResponseDto
                    {
                        ScheduleId = s.Id,
                        DepartureDate = s.DepartureDate,
                        MaxParticipant = s.MaxParticipant,
                        CurrentBooked = s.CurrentBooked,
                        TotalDays = s.TotalDays,
                        AdultPrice = s.AdultPrice,
                        ChildrenPrice = s.ChildrenPrice
                    }).ToList(),
                TourGuide = tourGuide,
                Promotions = activePromotions,
                Days = dayDetails
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

    public async Task<TourDetailsResponseDto> GetTourDetailsAsync(Guid tourId, Guid? scheduleId = null)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourType)
                .Include(t => t.TourPlanLocations)
                    .ThenInclude(l => l.Location)
                .Include(t => t.TourSchedules)
                .Include(t => t.TourGuideMappings)
                    .ThenInclude(tg => tg.TourGuide)
                        .ThenInclude(tg => tg.User)
                .Include(t => t.PromotionApplicables)
                    .ThenInclude(p => p.Promotion)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

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

            var tourGuide = GetTourGuideInfo(tour);
            var dayDetails = BuildDayDetails(tour);

            return new TourDetailsResponseDto
            {
                TourId = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Content = tour.Content,
                TotalDays = tour.TotalDays,
                TotalDaysText = tour.TotalDays == 1 ? "1 ngày" : $"{tour.TotalDays} ngày {tour.TotalDays - 1} đêm",
                TourTypeId = tour.TourTypeId,
                TourTypeName = tour.TourType?.Name ?? "Unknown",
                AdultPrice = adultPrice,
                ChildrenPrice = childrenPrice,
                FinalPrice = finalPrice,
                IsDiscount = isDiscount,
                Status = tour.Status,
                Schedules = activeSchedules
                    .Select(s => new TourScheduleResponseDto
                    {
                        ScheduleId = s.Id,
                        DepartureDate = s.DepartureDate,
                        MaxParticipant = s.MaxParticipant,
                        CurrentBooked = s.CurrentBooked,
                        TotalDays = s.TotalDays,
                        AdultPrice = s.AdultPrice,
                        ChildrenPrice = s.ChildrenPrice
                    }).ToList(),
                TourGuide = tourGuide,
                Promotions = activePromotions,
                Days = dayDetails
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

    #region TourPlanLocation 

    public async Task<List<TourPlanLocationResponseDto>> UpdateLocationsAsync(Guid tourId, List<UpdateTourPlanLocationDto> dtos)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
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

            // Validate DestinationId
            var locationIds = dtos.Select(d => d.LocationId).Distinct().ToList();
            var validLocations = await _unitOfWork.LocationRepository
                .ActiveEntities
                .Where(l => locationIds.Contains(l.Id))
                .Select(l => l.Id)
                .ToListAsync();
            var invalidLocations = locationIds.Except(validLocations).ToList();
            if (invalidLocations.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"Các ID địa điểm không hợp lệ: {string.Join(", ", invalidLocations)}");

            var existingLocations = tour.TourPlanLocations.Where(l => !l.IsDeleted).ToList();
            var providedLocationIds = dtos.Where(d => d.TourPlanLocationId.HasValue).Select(d => d.TourPlanLocationId.Value).ToList();
            var toDelete = existingLocations.Where(l => !providedLocationIds.Contains(l.Id)).ToList();
            var toAdd = dtos.Where(d => !d.TourPlanLocationId.HasValue)
                .Select(d => new TourPlanLocation
                {
                    TourId = tourId,
                    LocationId = d.LocationId,
                    DayOrder = d.DayOrder,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Notes = d.Notes,
                    IsDeleted = false
                }).ToList();
            var toUpdate = dtos.Where(d => d.TourPlanLocationId.HasValue).ToList();

            // Validate time overlaps
            var allLocations = existingLocations
                .Where(l => !toDelete.Contains(l))
                .Select(l => new { l.Id, l.StartTime, l.EndTime, l.DayOrder })
                .Concat(toAdd.Select(l => new { Id = Guid.Empty, l.StartTime, l.EndTime, l.DayOrder }))
                .Concat(toUpdate.Select(u => new { Id = u.TourPlanLocationId.Value, u.StartTime, u.EndTime, u.DayOrder }))
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

            // Validate TotalDays
            var maxDayOrder = Math.Max(
                existingLocations.Any() ? existingLocations.Max(l => l.DayOrder) : 0,
                toAdd.Any() ? toAdd.Max(l => l.DayOrder) : 0
            );
            maxDayOrder = Math.Max(maxDayOrder, toUpdate.Any() ? toUpdate.Max(u => u.DayOrder) : 0);
            if (tour.TotalDays < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({tour.TotalDays}) không được nhỏ hơn Ngày lớn nhất ({maxDayOrder}).");

            // Kiểm tra số lượng lịch trình
            var scheduleDates = tour.TourSchedules.Select(s => s.DepartureDate.Date).Distinct().Count();
            if (scheduleDates < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Không đủ lịch trình ({scheduleDates}) để bao phủ Ngày thứ {maxDayOrder}.");

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
                    }

                    await _unitOfWork.TourPlanLocationRepository.AddRangeAsync(toAdd);
                    foreach (var location in toAdd)
                        changes.Add($"Đã thêm địa điểm: {location.LocationId}");

                    foreach (var dto in toUpdate)
                    {
                        var location = existingLocations.First(l => l.Id == dto.TourPlanLocationId.Value);
                        location.LocationId = dto.LocationId;
                        location.DayOrder = dto.DayOrder;
                        location.StartTime = dto.StartTime;
                        location.EndTime = dto.EndTime;
                        location.Notes = dto.Notes;
                        location.LastUpdatedTime = DateTimeOffset.UtcNow;
                        changes.Add($"Đã cập nhật địa điểm: {dto.LocationId}");
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

            var result = existingLocations
                .Where(l => !l.IsDeleted)
                .Select(l => new TourPlanLocationResponseDto
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    Notes = l.Notes
                })
                .Concat(toAdd.Select(l => new TourPlanLocationResponseDto
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    Notes = l.Notes
                }))
                .ToList();

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
                .Select(l => new TourPlanLocationResponseDto
                {
                    TourPlanLocationId = l.Id,
                    LocationId = l.LocationId,
                    DayOrder = l.DayOrder,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    Notes = l.Notes
                })
                .ToListAsync();

            return locations;
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
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourPlanLocations)
                .Include(t => t.TourSchedules)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Cannot add schedules to a cancelled tour.");

            foreach (var dto in dtos)
            {
                if (dto.DepartureDate < DateTime.UtcNow.Date)
                    throw CustomExceptionFactory.CreateBadRequestError($"Ngày khởi hành phải sau ngày hôm nay: {dto.DepartureDate:dd/MM/yyyy}.");
                if (dto.MaxParticipant <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");
                if (dto.TotalDays <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");
                if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm: {dto.DepartureDate:dd/MM/yyyy}.");
                if (dto.TotalDays != tour.TotalDays)
                    throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({dto.TotalDays}) không khớp với tour ({tour.TotalDays}).");
            }

            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            var newScheduleDates = dtos.Select(d => d.DepartureDate.Date).Distinct().ToList();
            var existingScheduleDates = tour.TourSchedules.Select(s => s.DepartureDate.Date).Distinct().ToList();
            var allScheduleDates = newScheduleDates.Concat(existingScheduleDates).Distinct().ToList();
            if (allScheduleDates.Count < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Không đủ số ngày lịch trình ({allScheduleDates.Count}) để bao phủ các ngày trong kế hoạch ({maxDayOrder}).");
            var schedules = dtos.Select(dto => new TourSchedule
            {
                TourId = tourId,
                DepartureDate = dto.DepartureDate,
                MaxParticipant = dto.MaxParticipant,
                TotalDays = dto.TotalDays,
                AdultPrice = dto.AdultPrice,
                ChildrenPrice = dto.ChildrenPrice
            }).ToList();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.TourScheduleRepository.AddRangeAsync(schedules);
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
                DepartureDate = s.DepartureDate,
                MaxParticipant = s.MaxParticipant,
                CurrentBooked = s.CurrentBooked,
                TotalDays = s.TotalDays,
                AdultPrice = s.AdultPrice,
                ChildrenPrice = s.ChildrenPrice
            }).ToList();
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

    public async Task<List<TourScheduleResponseDto>> GetSchedulesAsync(Guid tourId)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            var schedules = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Where(s => s.TourId == tourId && !s.IsDeleted)
                .Select(s => new TourScheduleResponseDto
                {
                    ScheduleId = s.Id,
                    DepartureDate = s.DepartureDate,
                    MaxParticipant = s.MaxParticipant,
                    CurrentBooked = s.CurrentBooked,
                    TotalDays = s.TotalDays,
                    AdultPrice = s.AdultPrice,
                    ChildrenPrice = s.ChildrenPrice
                })
                .ToListAsync();

            return schedules;
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

    public async Task<TourScheduleResponseDto> UpdateScheduleAsync(Guid tourId, Guid scheduleId, CreateTourScheduleDto dto)
    {
        try
        {
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
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật tour đã bị hủy.");

            if (dto.DepartureDate < DateTime.UtcNow.Date)
                throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải ở tương lai.");
            if (dto.MaxParticipant <= 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");
            if (dto.TotalDays <= 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày phải lớn hơn 0: {dto.DepartureDate:dd/MM/yyyy}.");
            if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm: {dto.DepartureDate:dd/MM/yyyy}.");
            if (dto.TotalDays != tour.TotalDays)
                throw CustomExceptionFactory.CreateBadRequestError($"Tổng số ngày ({dto.TotalDays}) không khớp với tour ({tour.TotalDays}).");

            var otherSchedules = tour.TourSchedules.Where(s => s.Id != scheduleId).Select(s => s.DepartureDate.Date).Distinct().ToList();
            var allScheduleDates = otherSchedules.Concat(new[] { dto.DepartureDate.Date }).Distinct().ToList();
            var maxDayOrder = tour.TourPlanLocations.Any() ? tour.TourPlanLocations.Max(l => l.DayOrder) : 0;
            if (allScheduleDates.Count < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Không đủ số ngày lịch trình ({allScheduleDates.Count}) để bao phủ các ngày trong kế hoạch ({maxDayOrder}).");

            schedule.DepartureDate = dto.DepartureDate;
            schedule.MaxParticipant = dto.MaxParticipant;
            schedule.TotalDays = dto.TotalDays;
            schedule.AdultPrice = dto.AdultPrice;
            schedule.ChildrenPrice = dto.ChildrenPrice;
            schedule.LastUpdatedTime = DateTimeOffset.UtcNow;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                 new[] { booking.User.Email },
                                $"Cập nhật Tour {tour.Name}",
                                $"Lịch trình ngày {schedule.DepartureDate:dd/MM/yyyy} của tour {tour.Name} đã được cập nhật. Vui lòng kiểm tra chi tiết."
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

            return new TourScheduleResponseDto
            {
                ScheduleId = schedule.Id,
                DepartureDate = schedule.DepartureDate,
                MaxParticipant = schedule.MaxParticipant,
                CurrentBooked = schedule.CurrentBooked,
                TotalDays = schedule.TotalDays,
                AdultPrice = schedule.AdultPrice,
                ChildrenPrice = schedule.ChildrenPrice
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

    public async Task DeleteScheduleAsync(Guid tourId, Guid scheduleId)
    {
        try
        {
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
            if (remainingSchedules.Count < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Không đủ số ngày lịch trình ({remainingSchedules.Count}) để bao phủ các ngày trong kế hoạch ({maxDayOrder}).");

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.TourScheduleRepository.Remove(schedule);
                    await _unitOfWork.SaveAsync();

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

    public async Task AddTourGuidesAsync(Guid tourId, List<Guid> guideIds)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourSchedules)
                .Include(t => t.TourGuideMappings)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể chỉ định hướng dẫn viên cho chuyến tham quan đã hủy.");

            // Kiểm tra TourGuide tồn tại và hợp lệ
            var validGuides = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(tg => tg.User)
                .Include(tg => tg.TourGuideSchedules)
                .Where(tg => guideIds.Contains(tg.Id))
                .ToListAsync();
            var invalidGuideIds = guideIds.Except(validGuides.Select(tg => tg.Id)).ToList();
            if (invalidGuideIds.Any())
                throw CustomExceptionFactory.CreateBadRequestError($"TourGuide IDs không hợp lệ: {string.Join(", ", invalidGuideIds)}");

            // Kiểm tra lịch trống của TourGuide
            var tourSchedules = tour.TourSchedules.Where(s => !s.IsDeleted).ToList();
            if (tourSchedules.Any())
            {
                var tourStart = tourSchedules.Min(s => s.DepartureDate.Date);
                var tourEnd = tourSchedules.Max(s => s.DepartureDate.AddDays(s.TotalDays).Date);
                foreach (var guide in validGuides)
                {
                    var conflictingSchedules = guide.TourGuideSchedules
                        .Where(s => !s.IsDeleted)
                        .Where(s => s.BookingId != null) // Chỉ kiểm tra lịch đã gắn với booking
                        .Where(s => s.Date.Date >= tourStart && s.Date.Date <= tourEnd)
                        .ToList();
                    if (conflictingSchedules.Any())
                        throw CustomExceptionFactory.CreateBadRequestError($"TourGuide {guide.User.FullName} không sẵn sàng trong khoảng {tourStart:yyyy-MM-dd} tới {tourEnd:yyyy-MM-dd}.");
                }
            }

            // check tourGuide 
            var existingGuideIds = tour.TourGuideMappings.Where(tg => !tg.IsDeleted).Select(tg => tg.GuideId).ToList();
            var newGuideIds = guideIds.Except(existingGuideIds).ToList();
            if (!newGuideIds.Any())
                return; // không có TourGuide mới để thêm

            // tour Guide Mapping
            var newMappings = newGuideIds.Select(guideId => new TourGuideMapping
            {
                TourId = tourId,
                GuideId = guideId,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow
            }).ToList();

            var changes = new List<string>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.TourGuideMappingRepository.AddRangeAsync(newMappings);
                    foreach (var mapping in newMappings)
                    {
                        var guide = validGuides.First(g => g.Id == mapping.GuideId);
                        changes.Add($"Added TourGuide: {guide.User.FullName}");
                    }

                    await _unitOfWork.SaveAsync();

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        var changeSummary = string.Join("\n", changes);
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                new[] { booking.User.Email },
                                $"Cập nhật thông tin Tour {tour.Name}",
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

    public async Task RemoveTourGuideAsync(Guid tourId, Guid guideId)
    {
        try
        {
            var tour = await _unitOfWork.TourRepository
                .ActiveEntities
                .Include(t => t.TourGuideMappings)
                .Include(t => t.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(t => t.Id == tourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour");

            if (tour.Status == TourStatus.Cancelled)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa tour guide khỏi lịch trình đã bị hủy.");

            var mapping = tour.TourGuideMappings
                .FirstOrDefault(tg => tg.GuideId == guideId && !tg.IsDeleted)
                ?? throw CustomExceptionFactory.CreateNotFoundError("TourGuide không được chỉ định cho tour này.");

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

                    if (tour.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                    {
                        foreach (var booking in tour.Bookings)
                        {
                            await _emailService.SendEmailAsync(
                                 new[] { booking.User.Email },
                                $"Cập nhật thông tin Tour {tour.Name}",
                                $"Tour {tour.Name} đã xóa hướng dẫn viên: {guide.User.FullName}."
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
        if (tour.Description != dto.Description)
            changes.Add($"Mô tả thay đổi từ '{tour.Description}' → '{dto.Description}'");
        if (tour.Content != dto.Content)
            changes.Add($"Nội dung thay đổi từ '{tour.Content}' → '{dto.Content}'");
        if (tour.TourTypeId != dto.TourTypeId)
            changes.Add($"Loại tour thay đổi từ '{tour.TourTypeId}' → '{dto.TourTypeId}'");
        if (tour.TotalDays != dto.TotalDays)
            changes.Add($"Số ngày thay đổi từ '{tour.TotalDays}' → '{dto.TotalDays}'");
        return changes;
    }

    private List<TourDayDetail> BuildDayDetails(Tour tour)
    {
        var groupedByDay = tour.TourPlanLocations
            .Where(l => !l.IsDeleted)
            .GroupBy(l => l.DayOrder)
            .OrderBy(g => g.Key);

        var dayDetails = new List<TourDayDetail>();

        foreach (var group in groupedByDay)
        {
            var activities = group.Select(l => new TourActivity
            {
                Id = l.Id,
                Type = "Location",
                Name = l.Location?.Name ?? "Unknown",
                Description = l.Location?.Description,
                Address = l.Location?.Address,
                DayOrder = l.DayOrder,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                StartTimeFormatted = l.StartTime.ToString(@"hh\:mm"),
                EndTimeFormatted = l.EndTime.ToString(@"hh\:mm"),
                Duration = $"{(l.EndTime - l.StartTime).TotalMinutes} phút",
            }).ToList();

            dayDetails.Add(new TourDayDetail
            {
                DayNumber = group.Key,
                Activities = activities
            });
        }

        return dayDetails;
    }

    private TourGuideDataModel? GetTourGuideInfo(Tour tour)
    {
        return tour.TourGuideMappings
            .Where(tg => !tg.IsDeleted && tg.TourGuide != null)
            .Select(tg => new TourGuideDataModel
            {
                Id = tg.TourGuide.Id,
                UserName = tg.TourGuide.User.FullName,
                Email = tg.TourGuide.User.Email,
                Sex = tg.TourGuide.User.Sex,
                Address = tg.TourGuide.User.Address,
                Rating = tg.TourGuide.Rating,
                Price = tg.TourGuide.Price,
                Introduction = tg.TourGuide.Introduction,
                AvatarUrl = tg.TourGuide.User.AvatarUrl,
            })
            .FirstOrDefault();
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
}