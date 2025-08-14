using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.BusinessModels.WorkshopModels;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IWorkshopService
{
    Task<List<WorkshopResponseDto>> GetFilteredWorkshopsAsync(string? name, Guid? craftVillageId, CancellationToken cancellationToken);
    Task<List<WorkshopResponseDto>> ModeratorGetFilteredWorkshopsAsync(FilterWorkshop filter, CancellationToken cancellationToken);
    Task<WorkshopResponseDto> CreateWorkshopAsync(CreateWorkshopDto dto);
    Task<WorkshopResponseDto> UpdateWorkshopAsync(Guid workshopId, UpdateWorkshopDto dto);
    Task<WorkshopResponseDto> SubmitWorkshopForReviewAsync(Guid workshopId, CancellationToken cancellationToken);
    Task<WorkshopResponseDto> ConfirmWorkshopAsync(Guid workshopId, ConfirmWorkshopDto dto);
    Task DeleteWorkshopAsync(Guid workshopId);
    // Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId);
    Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId, Guid? scheduleId = null);
    // Task<List<ActivityResponseDto>> CreateActivitiesAsync(Guid workshopId, List<CreateActivityDto> dtos);
    // Task<List<ActivityResponseDto>> GetActivitiesAsync(Guid workshopId);
    Task<List<ActivityResponseDto>> UpdateActivitiesAsync(Guid workshopId, List<UpdateActivityRequestDto> dtos);
    // Task<ActivityResponseDto> UpdateActivityAsync(Guid workshopId, Guid activityId, CreateActivityDto dto);
    // Task DeleteActivityAsync(Guid workshopId, Guid activityId);
    Task<List<ScheduleResponseDto>> CreateSchedulesAsync(Guid workshopId, List<CreateScheduleDto> dtos);
    // Task<List<ScheduleResponseDto>> GetSchedulesAsync(Guid workshopId);
    Task<ScheduleResponseDto> UpdateScheduleAsync(Guid workshopId, Guid scheduleId, CreateScheduleDto dto);
    Task DeleteScheduleAsync(Guid workshopId, Guid scheduleId);

    Task<List<WorkshopMedia>> AddWorkshopMediasAsync(Guid workshopId, List<WorkshopMediaCreateDto> createDtos);
    Task<bool> DeleteWorkshopMediaAsync(Guid workshopMediaId);
}

public class WorkshopService : IWorkshopService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IEnumService _enumService;
    private readonly IMapper _mapper;

    public WorkshopService(IUnitOfWork unitOfWork, IEmailService emailService, IUserContextService userContextService, ITimeService timeService, IMapper mapper, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userContextService = userContextService;
        _timeService = timeService;
        _mapper = mapper;
        _enumService = enumService;
    }

    public async Task<List<WorkshopResponseDto>> GetFilteredWorkshopsAsync(string? name, Guid? craftVillageId, CancellationToken cancellationToken)
    {
        try
        {
            var workshopsQuery = _unitOfWork.WorkshopRepository.ActiveEntities
                .Include(w => w.CraftVillage)
                    .ThenInclude(cv => cv.Location)
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .Where(ws => ws.Status == WorkshopStatus.Approved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                workshopsQuery = workshopsQuery.Where(w => w.Name.ToLower().Contains(name.ToLower()));
            }

            if (craftVillageId.HasValue)
            {
                workshopsQuery = workshopsQuery.Where(w => w.CraftVillageId == craftVillageId.Value);
            }

            var workshopItems = await workshopsQuery
                .OrderBy(w => w.Name)
                .ToListAsync(cancellationToken);

            if (!workshopItems.Any())
            {
                return new List<WorkshopResponseDto>();
            }

            var workshopIds = workshopItems.Select(w => w.Id).ToList();

            var allReviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Booking)
                .Where(r => r.Booking.WorkshopId.HasValue && workshopIds.Contains(r.Booking.WorkshopId.Value))
                .ToListAsync(cancellationToken);

            var workshopResponses = new List<WorkshopResponseDto>();

            foreach (var workshop in workshopItems)
            {
                var reviewsForWorkshop = allReviews
                    .Where(r => r.Booking.WorkshopId == workshop.Id)
                    .ToList();

                var averageRating = reviewsForWorkshop.Any() ? reviewsForWorkshop.Average(r => r.Rating) : 0.0;
                var totalReviews = reviewsForWorkshop.Count;

                var medias = await GetMediaWithoutVideoByIdAsync(workshop.Id, cancellationToken: default);

                var response = new WorkshopResponseDto
                {
                    Id = workshop.Id,
                    Name = workshop.Name ?? string.Empty,
                    Description = workshop.Description,
                    Content = workshop.Content,
                    Status = workshop.Status,
                    StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
                    CraftVillageId = workshop.CraftVillageId,
                    CraftVillageName = workshop.CraftVillage?.Location?.Name,
                    AverageRating = Math.Round(averageRating, 2),
                    TotalReviews = totalReviews,
                    Medias = medias ?? new List<MediaResponse>(),
                };

                workshopResponses.Add(response);
            }

            return workshopResponses;
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

    public async Task<List<WorkshopResponseDto>> ModeratorGetFilteredWorkshopsAsync(FilterWorkshop filter, CancellationToken cancellationToken)
    {
        try
        {
            var workshopsQuery = _unitOfWork.WorkshopRepository.ActiveEntities
                .Include(w => w.CraftVillage)
                    .ThenInclude(cv => cv.Location)
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.Name))
            {
                var nameLower = filter.Name.ToLower();
                workshopsQuery = workshopsQuery.Where(w => w.Name.ToLower().Contains(nameLower));
            }

            if (filter?.CraftVillageId.HasValue == true)
            {
                workshopsQuery = workshopsQuery.Where(w => w.CraftVillageId == filter.CraftVillageId.Value);
            }

            if (filter?.Status.HasValue == true)
            {
                workshopsQuery = workshopsQuery.Where(w => w.Status == filter.Status.Value);
            }

            var workshopItems = await workshopsQuery
                .OrderBy(w => w.Name)
                .ToListAsync(cancellationToken);

            if (!workshopItems.Any())
            {
                return new List<WorkshopResponseDto>();
            }

            var workshopIds = workshopItems.Select(w => w.Id).ToList();

            var allReviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Booking)
                .Where(r => r.Booking.WorkshopId.HasValue && workshopIds.Contains(r.Booking.WorkshopId.Value))
                .ToListAsync(cancellationToken);

            var workshopResponses = new List<WorkshopResponseDto>();

            foreach (var workshop in workshopItems)
            {
                var reviewsForWorkshop = allReviews
                    .Where(r => r.Booking.WorkshopId == workshop.Id)
                    .ToList();

                var averageRating = reviewsForWorkshop.Any() ? reviewsForWorkshop.Average(r => r.Rating) : 0.0;
                var totalReviews = reviewsForWorkshop.Count;

                var medias = await GetMediaWithoutVideoByIdAsync(workshop.Id, cancellationToken: default);

                workshopResponses.Add(new WorkshopResponseDto
                {
                    Id = workshop.Id,
                    Name = workshop.Name ?? string.Empty,
                    Description = workshop.Description,
                    Content = workshop.Content,
                    Status = workshop.Status,
                    StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
                    CraftVillageId = workshop.CraftVillageId,
                    CraftVillageName = workshop.CraftVillage?.Location?.Name,
                    AverageRating = Math.Round(averageRating, 2),
                    TotalReviews = totalReviews,
                    Medias = medias ?? new List<MediaResponse>(),
                });
            }

            return workshopResponses;
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

    public async Task<WorkshopResponseDto> CreateWorkshopAsync(CreateWorkshopDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;
            var isCraftVillageOwner = _userContextService.HasAnyRole(AppRole.CRAFT_VILLAGE_OWNER);

            var craftVillageId = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .Where(cv => cv.OwnerId == currentUserId)
                .Select(cv => cv.Id)
                .FirstOrDefaultAsync();

            if (!isCraftVillageOwner)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên workshop là bắt buộc.");

            if (craftVillageId == Guid.Empty)
                throw CustomExceptionFactory.CreateBadRequestError("Bạn không phải là chủ của làng nghề nào");

            var craftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(c => c.Id == craftVillageId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Craft village");

            var workshop = new Workshop
            {
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                CraftVillageId = craftVillageId,
                Status = WorkshopStatus.Draft
            };

            await _unitOfWork.WorkshopRepository.AddAsync(workshop);
            await _unitOfWork.SaveAsync();

            List<WorkshopMedia> workshopMedias = new();
            if (dto.MediaDtos.Any())
            {
                workshopMedias = dto.MediaDtos.Select(media => new WorkshopMedia
                {
                    Id = Guid.NewGuid(),
                    WorkshopId = workshop.Id,
                    MediaUrl = media.MediaUrl,
                    IsThumbnail = media.IsThumbnail,
                    CreatedBy = currentUserId.ToString(),
                    LastUpdatedBy = currentUserId.ToString(),
                    CreatedTime = currentTime,
                    LastUpdatedTime = currentTime
                }).ToList();

                await _unitOfWork.WorkshopMediaRepository.AddRangeAsync(workshopMedias);
                await _unitOfWork.SaveAsync();
            }

            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
                CraftVillageId = workshop.CraftVillageId,
                Medias = workshopMedias.Select(m => new MediaResponse
                {
                    MediaUrl = m.MediaUrl,
                    FileName = Path.GetFileName(m.MediaUrl),
                    FileType = Path.GetExtension(m.MediaUrl)?.TrimStart('.'),
                    IsThumbnail = m.IsThumbnail,
                    SizeInBytes = 0,
                    CreatedTime = m.CreatedTime
                }).ToList()
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

    public async Task<WorkshopResponseDto> UpdateWorkshopAsync(Guid workshopId, UpdateWorkshopDto dto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isValidRole = _userContextService.HasAnyRole(AppRole.CRAFT_VILLAGE_OWNER);
            if (!isValidRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên workshop là bắt buộc.");

            // if (dto.CraftVillageId == Guid.Empty)
            //     throw CustomExceptionFactory.CreateBadRequestError("ID làng nghề là bắt buộc.");

            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var craftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(c => c.Id == workshop.CraftVillageId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Craft village");

            if (craftVillage.OwnerId != currentUserId)
                throw CustomExceptionFactory.CreateForbiddenError();

            if (workshop.Status == WorkshopStatus.Rejected)
                throw new InvalidOperationException("Không thể cập nhật một workshop đã bị hủy.");

            // thay đổi những gì
            var changes = new List<string>();
            if (workshop.Name != dto.Name)
                changes.Add($"Tên đã thay đổi từ '{workshop.Name}' thành '{dto.Name}'");
            if (workshop.Description != dto.Description)
                changes.Add($"Mô tả đã thay đổi từ '{workshop.Description}' thành '{dto.Description}'");
            if (workshop.Content != dto.Content)
                changes.Add($"Nội dung đã thay đổi từ '{workshop.Content}' thành '{dto.Content}'");
            // if (workshop.CraftVillageId != dto.CraftVillageId)
            //     changes.Add($"CraftVillageId đã thay đổi từ '{workshop.CraftVillageId}' thành '{dto.CraftVillageId}'");

            workshop.Name = dto.Name;
            workshop.Description = dto.Description;
            workshop.Content = dto.Content;
            // workshop.CraftVillageId = dto.CraftVillageId;
            workshop.LastUpdatedTime = DateTimeOffset.UtcNow;

            await UpdateWorkshopMediasAsync(workshopId, dto.MediaDtos, new CancellationToken());

            await _unitOfWork.SaveAsync();

            // gửi mail nếu đã đặt -> bị ảnh hưởng
            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed) && changes.Any())
            {
                var changeSummary = string.Join("\n", changes);
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật thông tin Workshop {workshop.Name}",
                        $"Workshop {workshop.Name} đã có các thay đổi sau:\n{changeSummary}\nVui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();
            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
                CraftVillageId = workshop.CraftVillageId
            };
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
    }

    public async Task<WorkshopResponseDto> ConfirmWorkshopAsync(Guid workshopId, ConfirmWorkshopDto dto)
    {
        var currentUserId = _userContextService.GetCurrentUserId();
        var currentTime = _timeService.SystemTimeNow;

        var isValidRole = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
        if (!isValidRole)
            throw CustomExceptionFactory.CreateForbiddenError();

        var workshop = await _unitOfWork.WorkshopRepository
            .ActiveEntities
            .Include(w => w.WorkshopActivities)
            .Include(w => w.WorkshopSchedules)
            .FirstOrDefaultAsync(w => w.Id == workshopId)
            ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

        // Validate
        var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
        if (workshop.WorkshopSchedules.Count < maxDayOrder)
            throw CustomExceptionFactory.CreateBadRequestError($"Không đủ lịch trình ({workshop.WorkshopSchedules.Count}) để đáp ứng DayOrder ({maxDayOrder}).");

        workshop.Status = WorkshopStatus.Approved;
        workshop.LastUpdatedTime = DateTimeOffset.UtcNow;

        await _unitOfWork.SaveAsync();

        return new WorkshopResponseDto
        {
            Id = workshop.Id,
            Name = workshop.Name,
            Description = workshop.Description,
            Content = workshop.Content,
            Status = workshop.Status,
            StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
            CraftVillageId = workshop.CraftVillageId
        };
    }

    public async Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId)
    {
        var workshop = await _unitOfWork.WorkshopRepository
            .ActiveEntities
            .Include(w => w.CraftVillage)
                .ThenInclude(w => w.Location)
            .Include(w => w.WorkshopActivities)
            .Include(w => w.WorkshopSchedules)
            .FirstOrDefaultAsync(w => w.Id == workshopId)
            ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

        if (workshop.Status == WorkshopStatus.Draft || workshop.Status == WorkshopStatus.Pending)
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var checkRoleCraftVillageOwner = _userContextService.HasRole(AppRole.CRAFT_VILLAGE_OWNER);
            if (!checkRoleCraftVillageOwner)
                throw CustomExceptionFactory.CreateForbiddenError();

            var checkRoleModerator = _userContextService.HasRole(AppRole.MODERATOR);
            if (!checkRoleModerator)
                throw CustomExceptionFactory.CreateForbiddenError();
        }

        var dayDetails = BuildDayDetails(workshop);

        return new WorkshopDetailsResponseDto
        {
            WorkshopId = workshop.Id,
            Name = workshop.Name,
            Description = workshop.Description,
            Content = workshop.Content,
            CraftVillageId = workshop.CraftVillageId,
            CraftVillageName = workshop.CraftVillage.Location.Name,
            Status = workshop.Status,
            StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status) ?? string.Empty,
            // Activities = workshop.WorkshopActivities.Select(a => new ActivityResponseDto
            // {
            //     ActivityId = a.Id,
            //     Activity = a.Activity,
            //     Description = a.Description,
            //     StartTime = a.StartTime,
            //     EndTime = a.EndTime,
            //     Notes = a.Notes,
            //     DayOrder = a.DayOrder
            // }).ToList(),
            Schedules = workshop.WorkshopSchedules.Select(s => new ScheduleResponseDto
            {
                ScheduleId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxParticipant = s.MaxParticipant,
                CurrentBooked = s.CurrentBooked,
                AdultPrice = s.AdultPrice,
                ChildrenPrice = s.ChildrenPrice,
                Notes = s.Notes
            }).ToList(),
            Days = dayDetails
        };
    }

    public async Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId, Guid? scheduleId = null)
    {
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository.ActiveEntities
                .Include(ws => ws.WorkshopActivities)
                .Where(w => w.Id == workshopId)
                .Select(w => new
                {
                    Workshop = w,
                    CraftVillageName = w.CraftVillage.Location.Name
                })
                .FirstOrDefaultAsync()
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var activeSchedules = await _unitOfWork.WorkshopScheduleRepository.ActiveEntities
                .Where(s => s.WorkshopId == workshopId && !s.IsDeleted)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var activePromotions = await _unitOfWork.PromotionApplicableRepository.ActiveEntities
                .Where(p => p.WorkshopId == workshopId
                    && !p.IsDeleted
                    && p.Promotion != null
                    && p.Promotion.StartDate <= now
                    && p.Promotion.EndDate >= now)
                .Select(p => new PromotionDto
                {
                    Id = p.Promotion.Id,
                    Name = p.Promotion.PromotionName,
                    DiscountPercentage = p.Promotion.DiscountType == DiscountType.Percentage ? p.Promotion.DiscountValue : 0,
                    StartDate = p.Promotion.StartDate,
                    EndDate = p.Promotion.EndDate
                })
                .ToListAsync();

            decimal adultPrice, childrenPrice;
            if (scheduleId.HasValue)
            {
                var selectedSchedule = activeSchedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                if (selectedSchedule == null)
                    throw CustomExceptionFactory.CreateNotFoundError("Schedule không tồn tại trong workshop này.");

                adultPrice = selectedSchedule.AdultPrice;
                childrenPrice = selectedSchedule.ChildrenPrice;
            }
            else
            {
                adultPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.AdultPrice) : 0m;
                childrenPrice = activeSchedules.Any() ? activeSchedules.Min(s => s.ChildrenPrice) : 0m;
            }

            var isDiscount = activePromotions.Any();
            var maxDiscount = isDiscount ? activePromotions.Max(p => p.DiscountPercentage) : 0;
            var finalPrice = adultPrice * (1 - maxDiscount / 100);

            var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Where(r => r.Booking.WorkshopId == workshopId && !r.IsDeleted)
                .Include(r => r.User)
                .Include(r => r.Booking)
                .ToListAsync();

            double averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0.0;
            int totalReviews = reviews.Count;

            var reviewDtos = reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedTime,
                UserName = r.User?.FullName ?? "Unknown",
                UserId = r.UserId,
                BookingId = r.BookingId,
                WorkshopId = r.Booking?.WorkshopId
            }).ToList();

            var medias = await GetMediaWithoutVideoByIdAsync(workshopId, cancellationToken: default);

            var response = new WorkshopDetailsResponseDto
            {
                WorkshopId = workshop.Workshop.Id,
                Name = workshop.Workshop.Name,
                Description = workshop.Workshop.Description,
                Content = workshop.Workshop.Content,
                CraftVillageId = workshop.Workshop.CraftVillageId,
                CraftVillageName = workshop.CraftVillageName,
                Status = workshop.Workshop.Status,
                StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Workshop.Status) ?? string.Empty,
                Schedules = activeSchedules.Select(s => new ScheduleResponseDto
                {
                    ScheduleId = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MaxParticipant = s.MaxParticipant,
                    CurrentBooked = s.CurrentBooked,
                    AdultPrice = s.AdultPrice,
                    ChildrenPrice = s.ChildrenPrice,
                    Notes = s.Notes
                }).ToList(),
                Days = BuildDayDetails(workshop.Workshop),
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = totalReviews,
                Reviews = reviewDtos,
                Medias = medias ?? new List<MediaResponse>(),
                Promotions = activePromotions
            };

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
    }

    public async Task<WorkshopResponseDto> SubmitWorkshopForReviewAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;
            var checkRole = _userContextService.HasRole(AppRole.CRAFT_VILLAGE_OWNER);
            if (!checkRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(ws => ws.CraftVillage)
                    .ThenInclude(cv => cv.Owner)
                .FirstOrDefaultAsync(w => w.Id == workshopId, cancellationToken);
            if (workshop == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Workshop not found or not owned by user.");
            }

            if (workshop.Status != WorkshopStatus.Draft)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Workshop is not in Draft status.");
            }

            workshop.Status = WorkshopStatus.Pending;
            workshop.LastUpdatedBy = currentUserId;
            workshop.LastUpdatedTime = currentTime;
            _unitOfWork.WorkshopRepository.Update(workshop);

            var ownerEmail = workshop.CraftVillage.Owner.Email;
            if (ownerEmail == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("No user found to notify.");
            }
            await _emailService.SendEmailAsync(
               new[] { ownerEmail },
                "Cập nhật trạng thái workshop",
                "Cập nhật trạng thái workshop"
            );

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                StatusText = _enumService.GetEnumDisplayName<WorkshopStatus>(workshop.Status),
                CraftVillageId = workshop.CraftVillageId
            };
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
    }

    public async Task DeleteWorkshopAsync(Guid workshopId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.WorkshopSchedules)
                .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            if (workshop.WorkshopSchedules.Any(s => s.CurrentBooked > 0))
                throw new InvalidOperationException("Không thể xóa lịch trình đã có người đặt.");
            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                throw new InvalidOperationException("Không thể xóa lịch trình đã có người đặt.");
            // var remainingSchedules = workshop.WorkshopSchedules.Where(s => s.Id != scheduleId).Select(s => s.StartTime.Date).Distinct().ToList();
            // var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
            // if (remainingSchedules.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không thể xóa schedule: Not enough remaining schedules ({remainingSchedules.Count}) to cover DayOrder ({maxDayOrder}).");

            workshop.IsDeleted = true;
            _unitOfWork.WorkshopRepository.Update(workshop);
            await _unitOfWork.SaveAsync();

            // if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            // {
            //     foreach (var booking in workshop.Bookings)
            //     {
            //         await _emailService.SendEmailAsync(
            //             new[] { booking.User!.Email },
            //             $"Cập nhật Workshop {workshop.Name}",
            //             $"Lịch trình ngày {schedule.StartTime:dd/MM/yyyy} của workshop {workshop.Name} đã bị xóa. Vui lòng kiểm tra chi tiết."
            //         );
            //     }
            // }

            await transaction.CommitAsync();
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
    }

    #region WorkshopActivityService
    // public async Task<List<ActivityResponseDto>> CreateActivitiesAsync(Guid workshopId, List<CreateActivityDto> dtos)
    // {
    //     var workshop = await _unitOfWork.WorkshopRepository
    //         .ActiveEntities
    //         .Include(w => w.WorkshopActivities)
    //         .Include(w => w.Bookings)
    //             .ThenInclude(b => b.User)
    //         .FirstOrDefaultAsync(w => w.Id == workshopId)
    //         ?? throw CustomExceptionFactory.CreateBadRequestError("Workshop không tồn tại.");

    //     // Validate 
    //     foreach (var dto in dtos)
    //     {
    //         if (string.IsNullOrWhiteSpace(dto.Activity))
    //             throw CustomExceptionFactory.CreateBadRequestError($"Tên hoạt động là bắt buộc cho hoạt động trong ngày {dto.DayOrder}.");
    //         if (dto.DayOrder < 1)
    //             throw CustomExceptionFactory.CreateBadRequestError($"DayOrder phải là số dương cho hoạt động {dto.Activity}.");
    //         if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
    //             throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho hoạt động {dto.Activity}.");
    //     }

    //     // Check thời gian trong cùng 1 ngày dayorder
    //     var groupedByDay = dtos.GroupBy(d => d.DayOrder);
    //     foreach (var group in groupedByDay)
    //     {
    //         var activitiesInDay = group.OrderBy(a => a.StartTime).ToList();
    //         for (int i = 1; i < activitiesInDay.Count; i++)
    //         {
    //             if (activitiesInDay[i].StartTime < activitiesInDay[i - 1].EndTime)
    //                 throw CustomExceptionFactory.CreateBadRequestError($"Phát hiện thời gian trùng lặp trong DayOrder {group.Key}.");
    //         }
    //     }

    //     // Check tổng
    //     var existingActivities = workshop.WorkshopActivities
    //         .Where(a => dtos.Any(d => d.DayOrder == a.DayOrder))
    //         .ToList();
    //     foreach (var group in groupedByDay)
    //     {
    //         var existingInDay = existingActivities.Where(a => a.DayOrder == group.Key).OrderBy(a => a.StartTime).ToList();
    //         var newInDay = group.OrderBy(a => a.StartTime).ToList();
    //         var allInDay = existingInDay.Select(a => (a.StartTime, a.EndTime)).Concat(newInDay.Select(a => (a.StartTime, a.EndTime))).OrderBy(t => t.StartTime).ToList();
    //         for (int i = 1; i < allInDay.Count; i++)
    //         {
    //             if (allInDay[i].StartTime < allInDay[i - 1].EndTime)
    //                 throw CustomExceptionFactory.CreateBadRequestError($"Hoạt động mới trùng lặp với hoạt động hiện có trong DayOrder {group.Key}.");
    //         }
    //     }

    //     var activities = dtos.Select(dto => new WorkshopActivity
    //     {
    //         WorkshopId = workshopId,
    //         Activity = dto.Activity,
    //         Description = dto.Description,
    //         StartTime = dto.StartTime,
    //         EndTime = dto.EndTime,
    //         Notes = dto.Notes,
    //         DayOrder = dto.DayOrder
    //     }).ToList();

    //     using (var transaction = await _unitOfWork.BeginTransactionAsync())
    //     {
    //         try
    //         {
    //             foreach (var activity in activities)
    //             {
    //                 await _unitOfWork.WorkshopActivityRepository.AddAsync(activity);
    //             }
    //             await _unitOfWork.SaveAsync();

    //             // Send notification if there are bookings
    //             if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
    //             {
    //                 foreach (var booking in workshop.Bookings)
    //                 {
    //                     await _emailService.SendEmailAsync(
    //                         new[] { booking.User!.Email },
    //                         $"Cập nhật Workshop {workshop.Name}",
    //                         $"Workshop {workshop.Name} có thêm hoạt động mới. Vui lòng kiểm tra chi tiết."
    //                     );
    //                 }
    //             }

    //             await transaction.CommitAsync();
    //         }
    //         catch
    //         {
    //             await transaction.RollbackAsync();
    //             throw;
    //         }
    //     }

    //     return activities.Select(a => new ActivityResponseDto
    //     {
    //         ActivityId = a.Id,
    //         Activity = a.Activity,
    //         Description = a.Description,
    //         StartTime = a.StartTime,
    //         EndTime = a.EndTime,
    //         Notes = a.Notes,
    //         DayOrder = a.DayOrder
    //     }).ToList();
    // }

    // public async Task<List<ActivityResponseDto>> GetActivitiesAsync(Guid workshopId)
    // {
    //     try
    //     {
    //         var workshop = await _unitOfWork.WorkshopRepository
    //             .ActiveEntities
    //             .FirstOrDefaultAsync(w => w.Id == workshopId)
    //             ?? throw CustomExceptionFactory.CreateBadRequestError("Workshop");

    //         var activities = await _unitOfWork.WorkshopActivityRepository
    //             .ActiveEntities
    //             .Where(a => a.WorkshopId == workshopId && !a.IsDeleted)
    //             .Select(a => new ActivityResponseDto
    //             {
    //                 ActivityId = a.Id,
    //                 Activity = a.Activity,
    //                 Description = a.Description,
    //                 StartTime = a.StartTime,
    //                 EndTime = a.EndTime,
    //                 Notes = a.Notes,
    //                 DayOrder = a.DayOrder
    //             })
    //             .ToListAsync();

    //         return activities;
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

    // public async Task<ActivityResponseDto> UpdateActivityAsync(Guid workshopId, Guid activityId, CreateActivityDto dto)
    // {
    //     var workshop = await _unitOfWork.WorkshopRepository
    //         .ActiveEntities
    //         .Include(w => w.WorkshopActivities)
    //         .Include(w => w.Bookings)
    //         .ThenInclude(b => b.User)
    //         .FirstOrDefaultAsync(w => w.Id == workshopId)
    //         ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

    //     var activity = await _unitOfWork.WorkshopActivityRepository
    //         .ActiveEntities
    //         .FirstOrDefaultAsync(a => a.Id == activityId && a.WorkshopId == workshopId)
    //         ?? throw CustomExceptionFactory.CreateNotFoundError("Activity");

    //     if (string.IsNullOrWhiteSpace(dto.Activity))
    //         throw CustomExceptionFactory.CreateBadRequestError($"Tên hoạt động là bắt buộc cho DayOrder {dto.DayOrder}.");
    //     if (dto.DayOrder < 1)
    //         throw CustomExceptionFactory.CreateBadRequestError($"DayOrder phải là số dương cho hoạt động {dto.Activity}.");
    //     if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
    //         throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho hoạt động {dto.Activity}.");

    //     // Check for overlapping times
    //     var otherActivities = workshop.WorkshopActivities
    //         .Where(a => a.Id != activityId && a.DayOrder == dto.DayOrder)
    //         .OrderBy(a => a.StartTime)
    //         .ToList();
    //     if (dto.StartTime.HasValue && dto.EndTime.HasValue)
    //     {
    //         foreach (var other in otherActivities)
    //         {
    //             if (other.StartTime.HasValue && other.EndTime.HasValue &&
    //                 (dto.StartTime < other.EndTime && dto.EndTime > other.StartTime))
    //                 throw CustomExceptionFactory.CreateBadRequestError($"Hoạt động được cập nhật trùng lặp với hoạt động hiện có trong DayOrder {dto.DayOrder}.");
    //         }
    //     }

    //     activity.Activity = dto.Activity;
    //     activity.Description = dto.Description;
    //     activity.StartTime = dto.StartTime;
    //     activity.EndTime = dto.EndTime;
    //     activity.Notes = dto.Notes;
    //     activity.DayOrder = dto.DayOrder;
    //     activity.LastUpdatedTime = DateTimeOffset.UtcNow;

    //     using (var transaction = await _unitOfWork.BeginTransactionAsync())
    //     {
    //         try
    //         {
    //             await _unitOfWork.SaveAsync();

    //             // Send notification if there are bookings
    //             if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
    //             {
    //                 foreach (var booking in workshop.Bookings)
    //                 {
    //                     await _emailService.SendEmailAsync(
    //                         new[] { booking.User!.Email },
    //                         $"Cập nhật Workshop {workshop.Name}",
    //                         $"Hoạt động {activity.Activity} trong workshop {workshop.Name} đã được cập nhật. Vui lòng kiểm tra chi tiết."
    //                     );
    //                 }
    //             }

    //             await transaction.CommitAsync();
    //         }
    //         catch
    //         {
    //             await transaction.RollbackAsync();
    //             throw;
    //         }
    //     }

    //     return new ActivityResponseDto
    //     {
    //         ActivityId = activity.Id,
    //         Activity = activity.Activity,
    //         Description = activity.Description,
    //         StartTime = activity.StartTime,
    //         EndTime = activity.EndTime,
    //         Notes = activity.Notes,
    //         DayOrder = activity.DayOrder
    //     };
    // }

    public async Task<List<ActivityResponseDto>> UpdateActivitiesAsync(Guid workshopId, List<UpdateActivityRequestDto> dtos)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.WorkshopSchedules)
                .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            // Validate 
            foreach (var dto in dtos)
            {
                if (dto.ActivityId.HasValue)
                {
                    if (!workshop.WorkshopActivities.Any(a => a.Id == dto.ActivityId && !a.IsDeleted))
                        throw CustomExceptionFactory.CreateBadRequestError($"Activity với ID {dto.ActivityId} không tồn tại hoặc đã bị xóa.");
                }
                if (string.IsNullOrWhiteSpace(dto.Activity))
                    throw CustomExceptionFactory.CreateBadRequestError($"Tên hoạt động là bắt buộc cho DayOrder {dto.DayOrder}.");
                if (dto.DayOrder < 1)
                    throw CustomExceptionFactory.CreateBadRequestError($"DayOrder phải là số dương cho hoạt động {dto.Activity}.");
                if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho hoạt động {dto.Activity}.");
            }

            // lọc cái nào để thêm, update, delete
            var existingActivities = workshop.WorkshopActivities.Where(a => !a.IsDeleted).ToList();
            var providedActivityIds = dtos.Where(d => d.ActivityId.HasValue).Select(d => d.ActivityId!.Value).ToList();
            var toDelete = existingActivities.Where(a => !providedActivityIds.Contains(a.Id)).ToList();
            var toAdd = dtos.Where(d => !d.ActivityId.HasValue)
                .Select(d => new WorkshopActivity
                {
                    WorkshopId = workshopId,
                    Activity = d.Activity,
                    Description = d.Description,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Notes = d.Notes,
                    DayOrder = d.DayOrder,
                    IsDeleted = false
                }).ToList();
            var toUpdate = dtos.Where(d => d.ActivityId.HasValue).ToList();

            var updateIds = toUpdate.Select(u => u.ActivityId.Value).ToHashSet();

            var allActivities = existingActivities
                .Where(a => !toDelete.Contains(a) && !updateIds.Contains(a.Id))
                .Select(a => new { a.Id, a.StartTime, a.EndTime, a.DayOrder })
                .Concat(toAdd.Select(a => new { Id = Guid.Empty, a.StartTime, a.EndTime, a.DayOrder }))
                .Concat(toUpdate.Select(u => new { Id = u.ActivityId!.Value, u.StartTime, u.EndTime, u.DayOrder }))
                .Where(a => a.StartTime.HasValue && a.EndTime.HasValue)
                .GroupBy(a => a.DayOrder)
                .ToList();

            foreach (var group in allActivities)
            {
                var activitiesInDay = group.OrderBy(a => a.StartTime).ToList();
                for (int i = 1; i < activitiesInDay.Count; i++)
                {
                    if (activitiesInDay[i].StartTime < activitiesInDay[i - 1].EndTime)
                        throw CustomExceptionFactory.CreateBadRequestError($"Phát hiện thời gian trùng lặp trong DayOrder {group.Key}.");
                }
            }

            // var maxDayOrder = Math.Max(
            //     existingActivities.Any() ? existingActivities.Max(a => a.DayOrder) : 0,
            //     toAdd.Any() ? toAdd.Max(a => a.DayOrder) : 0
            // );
            // maxDayOrder = Math.Max(
            //     maxDayOrder,
            //     toUpdate.Any() ? toUpdate.Max(u => u.DayOrder) : 0
            // );
            // var scheduleDates = workshop.WorkshopSchedules.Select(s => s.StartTime.Date).Distinct().Count();
            // if (scheduleDates < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không đủ lịch trình ({scheduleDates}) để đáp ứng DayOrder ({maxDayOrder}).");

            // Apply changes in a transaction
            var changes = new List<string>();

            // delete
            foreach (var activity in toDelete)
            {
                activity.IsDeleted = true;
                activity.LastUpdatedTime = DateTimeOffset.UtcNow;
                changes.Add($"Đã xóa hoạt động: {activity.Activity}");
            }

            // add  
            foreach (var activity in toAdd)
            {
                await _unitOfWork.WorkshopActivityRepository.AddAsync(activity);
                changes.Add($"Đã thêm hoạt động: {activity.Activity}");
            }

            // update
            foreach (var dto in toUpdate)
            {
                var activity = existingActivities.First(a => a.Id == dto.ActivityId!.Value);
                activity.Activity = dto.Activity;
                activity.Description = dto.Description;
                activity.StartTime = dto.StartTime;
                activity.EndTime = dto.EndTime;
                activity.Notes = dto.Notes;
                activity.DayOrder = dto.DayOrder;
                activity.LastUpdatedTime = DateTimeOffset.UtcNow;
                changes.Add($"Đã cập nhật hoạt động: {dto.Activity}");
            }

            await _unitOfWork.SaveAsync();

            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                var changeSummary = string.Join("\n", changes);
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật Workshop {workshop.Name}",
                        $"Workshop {workshop.Name} đã có các thay đổi sau:\n{changeSummary}\nVui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();

            var result = existingActivities
                .Where(a => !a.IsDeleted)
                .Select(a => new ActivityResponseDto
                {
                    ActivityId = a.Id,
                    Activity = a.Activity,
                    Description = a.Description,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Notes = a.Notes,
                    DayOrder = a.DayOrder
                })
                .Concat(toAdd.Select(a => new ActivityResponseDto
                {
                    ActivityId = a.Id,
                    Activity = a.Activity,
                    Description = a.Description,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Notes = a.Notes,
                    DayOrder = a.DayOrder
                }))
                .ToList();

            return result;
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
    }

    public async Task DeleteActivityAsync(Guid workshopId, Guid activityId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var activity = await _unitOfWork.WorkshopActivityRepository
                .ActiveEntities
                .FirstOrDefaultAsync(a => a.Id == activityId && a.WorkshopId == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Activity");


            _unitOfWork.WorkshopActivityRepository.Remove(activity);
            await _unitOfWork.SaveAsync();

            // Send notification if there are bookings
            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật Workshop {workshop.Name}",
                        $"Hoạt động {activity.Activity} trong workshop {workshop.Name} đã được xóa. Vui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();
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
    }

    #endregion

    #region WorkshopScheduleService

    public async Task<List<ScheduleResponseDto>> CreateSchedulesAsync(Guid workshopId, List<CreateScheduleDto> dtos)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.WorkshopSchedules)
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;

            // Validate 
            foreach (var dto in dtos)
            {
                if (dto.StartTime >= dto.EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho lịch trình bắt đầu lúc {dto.StartTime}.");
                if (dto.MaxParticipant <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia tối đa phải là số dương cho lịch trình bắt đầu lúc {dto.StartTime}.");
                if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                    throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm cho lịch trình bắt đầu lúc {dto.StartTime}.");

                if (maxDayOrder > 0)
                {
                    var durationDays = (dto.EndTime.Date - dto.StartTime.Date).Days + 1;
                    if (durationDays < maxDayOrder)
                    {
                        throw CustomExceptionFactory.CreateBadRequestError(
                            $"Lịch trình từ {dto.StartTime} đến {dto.EndTime} chỉ có {durationDays} ngày, " +
                            $"yêu cầu tối thiểu {maxDayOrder} ngày để đáp ứng các hoạt động."
                        );
                    }
                }

                ValidateScheduleConflicts(workshop.WorkshopSchedules, dto.StartTime, dto.EndTime);
            }

            // var newScheduleDates = dtos.Select(d => d.StartTime.Date).Distinct().ToList();
            // var existingScheduleDates = workshop.WorkshopSchedules.Select(s => s.StartTime.Date).Distinct().ToList();
            // var allScheduleDates = newScheduleDates.Concat(existingScheduleDates).Distinct().ToList();
            // if (allScheduleDates.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không đủ ngày lịch trình ({allScheduleDates.Count}) để đáp ứng DayOrder ({maxDayOrder}).");

            var schedules = dtos.Select(dto => new WorkshopSchedule
            {
                WorkshopId = workshopId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MaxParticipant = dto.MaxParticipant,
                AdultPrice = dto.AdultPrice,
                ChildrenPrice = dto.ChildrenPrice,
                Notes = dto.Notes
            }).ToList();


            foreach (var schedule in schedules)
            {
                await _unitOfWork.WorkshopScheduleRepository.AddAsync(schedule);
            }
            await _unitOfWork.SaveAsync();

            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật Workshop {workshop.Name}",
                        $"Workshop {workshop.Name} có lịch trình mới. Vui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();


            return schedules.Select(s => new ScheduleResponseDto
            {
                ScheduleId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxParticipant = s.MaxParticipant,
                CurrentBooked = s.CurrentBooked,
                AdultPrice = s.AdultPrice,
                ChildrenPrice = s.ChildrenPrice,
                Notes = s.Notes
            }).ToList();
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
    }

    public async Task<List<ScheduleResponseDto>> GetSchedulesAsync(Guid workshopId)
    {
        var workshop = await _unitOfWork.WorkshopRepository
            .ActiveEntities
            .FirstOrDefaultAsync(w => w.Id == workshopId)
            ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

        var schedules = await _unitOfWork.WorkshopScheduleRepository
            .ActiveEntities
            .Where(s => s.WorkshopId == workshopId)
            .Select(s => new ScheduleResponseDto
            {
                ScheduleId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxParticipant = s.MaxParticipant,
                CurrentBooked = s.CurrentBooked,
                AdultPrice = s.AdultPrice,
                ChildrenPrice = s.ChildrenPrice,
                Notes = s.Notes
            })
            .ToListAsync();

        return schedules;
    }

    public async Task<ScheduleResponseDto> UpdateScheduleAsync(Guid workshopId, Guid scheduleId, CreateScheduleDto dto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.WorkshopSchedules)
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var schedule = await _unitOfWork.WorkshopScheduleRepository
                .ActiveEntities
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.WorkshopId == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Schedule");

            // if (schedule.Bookings.Any(b => b.Status != BookingStatus.Pending))
            if (schedule.Bookings.Any())
                throw CustomExceptionFactory.CreateBadRequestError("Không thể cập nhật lịch trình đã có người đặt.");

            if (dto.StartTime >= dto.EndTime)
                throw CustomExceptionFactory.CreateBadRequestError("Thời gian kết thúc phải sau thời gian bắt đầu.");
            if (dto.MaxParticipant <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Số lượng người tham gia tối đa phải là số dương.");
            if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                throw CustomExceptionFactory.CreateBadRequestError("Giá không được âm.");

            // check
            // var otherSchedules = workshop.WorkshopSchedules.Where(s => s.Id != scheduleId).Select(s => s.StartTime.Date).Distinct().ToList();
            // var allScheduleDates = otherSchedules.Concat(new[] { dto.StartTime.Date }).Distinct().ToList();
            var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
            // if (allScheduleDates.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không đủ ngày lịch trình ({allScheduleDates.Count}) để đáp ứng DayOrder ({maxDayOrder}).");
            if (maxDayOrder > 0)
            {
                var durationDays = (dto.EndTime.Date - dto.StartTime.Date).Days + 1;
                if (durationDays < maxDayOrder)
                {
                    throw CustomExceptionFactory.CreateBadRequestError(
                        $"Khoảng thời gian của lịch trình ({durationDays} ngày) không đủ để đáp ứng hoạt động yêu cầu {maxDayOrder} ngày."
                    );
                }
            }

            ValidateScheduleConflicts(workshop.WorkshopSchedules, dto.StartTime, dto.EndTime, schedule.Id);

            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.MaxParticipant = dto.MaxParticipant;
            schedule.AdultPrice = dto.AdultPrice;
            schedule.ChildrenPrice = dto.ChildrenPrice;
            schedule.Notes = dto.Notes;
            schedule.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveAsync();

            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật Workshop {workshop.Name}",
                        $"Lịch trình ngày {schedule.StartTime:dd/MM/yyyy} của workshop {workshop.Name} đã được cập nhật. Vui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();

            return new ScheduleResponseDto
            {
                ScheduleId = schedule.Id,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                MaxParticipant = schedule.MaxParticipant,
                CurrentBooked = schedule.CurrentBooked,
                AdultPrice = schedule.AdultPrice,
                ChildrenPrice = schedule.ChildrenPrice,
                Notes = schedule.Notes
            };
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
    }

    public async Task DeleteScheduleAsync(Guid workshopId, Guid scheduleId)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.WorkshopActivities)
                .Include(w => w.WorkshopSchedules)
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var schedule = await _unitOfWork.WorkshopScheduleRepository
                .ActiveEntities
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.WorkshopId == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Schedule");

            if (schedule.Bookings.Any())
                throw CustomExceptionFactory.CreateBadRequestError("Không thể xóa lịch trình đã có người đặt.");

            // var remainingSchedules = workshop.WorkshopSchedules.Where(s => s.Id != scheduleId).Select(s => s.StartTime.Date).Distinct().ToList();
            // var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
            // if (remainingSchedules.Count < maxDayOrder)
            //     throw CustomExceptionFactory.CreateBadRequestError($"Không thể xóa schedule: Not enough remaining schedules ({remainingSchedules.Count}) to cover DayOrder ({maxDayOrder}).");


            _unitOfWork.WorkshopScheduleRepository.Remove(schedule);
            await _unitOfWork.SaveAsync();

            if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            {
                foreach (var booking in workshop.Bookings)
                {
                    await _emailService.SendEmailAsync(
                        new[] { booking.User!.Email },
                        $"Cập nhật Workshop {workshop.Name}",
                        $"Lịch trình ngày {schedule.StartTime:dd/MM/yyyy} của workshop {workshop.Name} đã bị xóa. Vui lòng kiểm tra chi tiết."
                    );
                }
            }

            await transaction.CommitAsync();
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
    }

    #endregion

    private List<WorkshopDayDetail> BuildDayDetails(Workshop workshop)
    {
        var groupedByDay = workshop.WorkshopActivities
            .Where(l => !l.IsDeleted)
            .GroupBy(l => l.DayOrder)
            .OrderBy(g => g.Key);

        var dayDetails = new List<WorkshopDayDetail>();

        foreach (var group in groupedByDay)
        {
            var activities = group.Select(ws => new WorkshopActivityDto
            {
                Id = ws.Id,
                Activity = ws.Activity ?? "Unknown",
                Description = ws.Description,
                DayOrder = ws.DayOrder,
                StartTime = ws.StartTime,
                EndTime = ws.EndTime,
                StartTimeFormatted = ws.StartTime?.ToString(@"hh\:mm") ?? "",
                EndTimeFormatted = ws.EndTime?.ToString(@"hh\:mm") ?? "",
            }).ToList();

            dayDetails.Add(new WorkshopDayDetail
            {
                DayNumber = group.Key,
                Activities = activities
            });
        }

        return dayDetails;
    }

    #region Workshop Media

    public async Task<bool> DeleteWorkshopMediaAsync(Guid workshopMediaId)
    {
        try
        {
            var workshopMedia = await _unitOfWork.WorkshopMediaRepository
                .ActiveEntities
                .FirstOrDefaultAsync(wm => wm.Id == workshopMediaId);

            if (workshopMedia == null)
            {
                return false;
            }

            _unitOfWork.WorkshopMediaRepository.Remove(workshopMedia);
            await _unitOfWork.SaveAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<WorkshopMedia>> AddWorkshopMediasAsync(Guid workshopId, List<WorkshopMediaCreateDto> createDtos)
    {
        try
        {
            var workshop = await _unitOfWork.WorkshopMediaRepository
                .ActiveEntities
                .FirstOrDefaultAsync(w => w.Id == workshopId);

            if (workshop == null)
            {
                return new List<WorkshopMedia>();
            }

            var newMedias = createDtos.Select(dto => new WorkshopMedia
            {
                Id = Guid.NewGuid(),
                WorkshopId = workshopId,
                MediaUrl = dto.MediaUrl,
                FileName = dto.FileName,
                FileType = dto.FileType,
                SizeInBytes = dto.SizeInBytes,
                IsThumbnail = dto.IsThumbnail,
                CreatedTime = DateTime.UtcNow,
                LastUpdatedTime = DateTime.UtcNow
            }).ToList();

            await _unitOfWork.WorkshopMediaRepository.AddRangeAsync(newMedias);
            await _unitOfWork.SaveAsync();
            return newMedias;
        }
        catch (Exception)
        {
            return new List<WorkshopMedia>();
        }
    }

    public async Task<List<MediaResponse>> GetMediaWithoutVideoByIdAsync(Guid workshopId, CancellationToken cancellationToken)
    {
        var locationMedias = await _unitOfWork.WorkshopMediaRepository
            .ActiveEntities
            .Where(em => em.WorkshopId == workshopId && !em.IsDeleted)
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

    private async Task UpdateWorkshopMediasAsync(Guid workshopId, List<MediaDto> mediaDtos, CancellationToken cancellationToken = default)
    {
        try
        {
            if (mediaDtos == null || !mediaDtos.Any())
                return;

            var existingMedias = await _unitOfWork.WorkshopMediaRepository.ActiveEntities
                .Where(m => m.WorkshopId == workshopId)
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
                        _unitOfWork.WorkshopMediaRepository.Update(existingMedia);
                    }
                }
                else
                {
                    var newMedia = new WorkshopMedia
                    {
                        WorkshopId = workshopId,
                        MediaUrl = mediaDto.MediaUrl,
                        FileName = fileName,
                        FileType = fileType,
                        SizeInBytes = 0,
                        IsThumbnail = mediaDto.IsThumbnail,
                        CreatedTime = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.WorkshopMediaRepository.AddAsync(newMedia);
                }
            }

            var allMediasAfterUpdate = await _unitOfWork.WorkshopMediaRepository.ActiveEntities
                .Where(m => m.WorkshopId == workshopId)
                .ToListAsync(cancellationToken);

            var thumbnails = allMediasAfterUpdate.Where(m => m.IsThumbnail).ToList();

            if (!thumbnails.Any())
            {
                var first = allMediasAfterUpdate.FirstOrDefault();
                if (first != null)
                {
                    first.IsThumbnail = true;
                    _unitOfWork.WorkshopMediaRepository.Update(first);
                }
            }
            else if (thumbnails.Count > 1)
            {
                foreach (var extraThumb in thumbnails.Skip(1))
                {
                    extraThumb.IsThumbnail = false;
                    _unitOfWork.WorkshopMediaRepository.Update(extraThumb);
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

    private void ValidateScheduleConflicts(IEnumerable<WorkshopSchedule> existingSchedules,
                                       DateTime newStart, DateTime newEnd,
                                       Guid? excludeScheduleId = null)
    {
        try
        {
            var conflict = existingSchedules
                .Where(s => excludeScheduleId == null || s.Id != excludeScheduleId.Value)
                .Any(s => newStart < s.EndTime && newEnd > s.StartTime);

            if (conflict)
            {
                throw CustomExceptionFactory.CreateBadRequestError(
                    "Thời gian lịch trình bị trùng hoặc chồng chéo với một lịch trình khác."
                );
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
