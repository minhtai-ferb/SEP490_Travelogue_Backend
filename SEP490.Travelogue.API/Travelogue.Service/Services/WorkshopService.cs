using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourModels;
using Travelogue.Service.BusinessModels.WorkshopModels;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IWorkshopService
{
    Task<List<WorkshopResponseDto>> GetFilteredWorkshopsAsync(string? name, Guid? craftVillageId, CancellationToken cancellationToken);
    Task<WorkshopResponseDto> CreateWorkshopAsync(CreateWorkshopDto dto);
    Task<WorkshopResponseDto> UpdateWorkshopAsync(Guid workshopId, UpdateWorkshopDto dto);
    Task<WorkshopResponseDto> SubmitWorkshopForReviewAsync(Guid workshopId, CancellationToken cancellationToken);
    Task<WorkshopResponseDto> ConfirmWorkshopAsync(Guid workshopId, ConfirmWorkshopDto dto);
    Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId);
    Task<WorkshopDetailsResponseDto> GetWorkshopDetailsAsync(Guid workshopId, Guid? scheduleId = null);
    Task<List<ActivityResponseDto>> CreateActivitiesAsync(Guid workshopId, List<CreateActivityDto> dtos);
    Task<List<ActivityResponseDto>> GetActivitiesAsync(Guid workshopId);
    Task<List<ActivityResponseDto>> UpdateActivitiesAsync(Guid workshopId, List<UpdateActivityRequestDto> dtos);
    Task<ActivityResponseDto> UpdateActivityAsync(Guid workshopId, Guid activityId, CreateActivityDto dto);
    Task DeleteActivityAsync(Guid workshopId, Guid activityId);
    Task<List<ScheduleResponseDto>> CreateSchedulesAsync(Guid workshopId, List<CreateScheduleDto> dtos);
    Task<List<ScheduleResponseDto>> GetSchedulesAsync(Guid workshopId);
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
    private readonly IMapper _mapper;

    public WorkshopService(IUnitOfWork unitOfWork, IEmailService emailService, IUserContextService userContextService, ITimeService timeService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userContextService = userContextService;
        _timeService = timeService;
        _mapper = mapper;
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

            // Lấy tất cả workshopId
            var workshopIds = workshopItems.Select(w => w.Id).ToList();

            // Lấy tất cả review liên quan tới các booking có WorkshopId nằm trong danh sách
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

                var response = new WorkshopResponseDto
                {
                    Id = workshop.Id,
                    Name = workshop.Name ?? string.Empty,
                    Description = workshop.Description,
                    Content = workshop.Content,
                    Status = workshop.Status,
                    CraftVillageId = workshop.CraftVillageId,
                    CraftVillageName = workshop.CraftVillage?.Location?.Name,
                    AverageRating = Math.Round(averageRating, 2),
                    TotalReviews = totalReviews,
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

    public async Task<WorkshopResponseDto> CreateWorkshopAsync(CreateWorkshopDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên workshop là bắt buộc.");

            if (dto.CraftVillageId == Guid.Empty)
                throw CustomExceptionFactory.CreateBadRequestError("ID làng nghề là bắt buộc.");

            var craftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(c => c.Id == dto.CraftVillageId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Craft village");

            var workshop = new Workshop
            {
                Name = dto.Name,
                Description = dto.Description,
                Content = dto.Content,
                CraftVillageId = dto.CraftVillageId,
                Status = WorkshopStatus.Draft
            };

            await _unitOfWork.WorkshopRepository.AddAsync(workshop);
            await _unitOfWork.SaveAsync();

            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                CraftVillageId = workshop.CraftVillageId
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
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw CustomExceptionFactory.CreateBadRequestError("Tên workshop là bắt buộc.");

            if (dto.CraftVillageId == Guid.Empty)
                throw CustomExceptionFactory.CreateBadRequestError("ID làng nghề là bắt buộc.");

            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

            var craftVillage = await _unitOfWork.CraftVillageRepository
                .ActiveEntities
                .FirstOrDefaultAsync(c => c.Id == dto.CraftVillageId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Craft village");

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
            if (workshop.CraftVillageId != dto.CraftVillageId)
                changes.Add($"CraftVillageId đã thay đổi từ '{workshop.CraftVillageId}' thành '{dto.CraftVillageId}'");

            workshop.Name = dto.Name;
            workshop.Description = dto.Description;
            workshop.Content = dto.Content;
            workshop.CraftVillageId = dto.CraftVillageId;
            workshop.LastUpdatedTime = DateTimeOffset.UtcNow;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
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
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                CraftVillageId = workshop.CraftVillageId
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

    public async Task<WorkshopResponseDto> ConfirmWorkshopAsync(Guid workshopId, ConfirmWorkshopDto dto)
    {
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
        var workshop = await _unitOfWork.WorkshopRepository
            .ActiveEntities
            .Include(w => w.CraftVillage)
                .ThenInclude(w => w.Location)
            .Include(w => w.WorkshopActivities)
            .Include(w => w.WorkshopSchedules)
            .Include(w => w.PromotionApplicables)
                .ThenInclude(p => p.Promotion)
            .Include(w => w.Bookings)
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(w => w.Id == workshopId)
            ?? throw CustomExceptionFactory.CreateNotFoundError("Workshop");

        var activeSchedules = workshop.WorkshopSchedules.Where(s => !s.IsDeleted).ToList();

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

        var activePromotions = workshop.PromotionApplicables
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

        var dayDetails = BuildDayDetails(workshop);

        var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
            .Include(r => r.User)
            .Include(r => r.Booking)
            .Where(r => r.Booking.WorkshopId == workshopId && !r.IsDeleted)
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

        var response = new WorkshopDetailsResponseDto
        {
            WorkshopId = workshop.Id,
            Name = workshop.Name,
            Description = workshop.Description,
            Content = workshop.Content,
            CraftVillageId = workshop.CraftVillageId,
            CraftVillageName = workshop.CraftVillage?.Location?.Name,
            Status = workshop.Status,
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
            Days = dayDetails,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = totalReviews,
            Reviews = reviewDtos
        };

        return response;
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
                CraftVillageId = workshop.CraftVillageId
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

    public async Task<WorkshopResponseDto> ModerateWorkshopAsync(ModerateWorkshopRequest request, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;
            var checkRole = _userContextService.HasRole(AppRole.MODERATOR);
            if (!checkRole)
                throw CustomExceptionFactory.CreateForbiddenError();

            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .Include(ws => ws.CraftVillage)
                .FirstOrDefaultAsync(w => w.Id == request.WorkshopId, cancellationToken);
            if (workshop == null || workshop.CraftVillage.OwnerId != Guid.Parse(currentUserId))
            {
                throw CustomExceptionFactory.CreateNotFoundError("Workshop not found or not owned by user.");
            }

            if (workshop.Status != WorkshopStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Workshop is not in Pending status.");
            }


            workshop.Status = WorkshopStatus.Pending;
            workshop.LastUpdatedBy = currentUserId;
            workshop.LastUpdatedTime = currentTime;
            _unitOfWork.WorkshopRepository.Update(workshop);

            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            if (!moderators.Any())
            {
                throw CustomExceptionFactory.CreateNotFoundError("No moderators found to notify.");
            }
            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                   new[] { moderator.Email },
                    "Có người dùng cần đăng workshop",
                    "Có người dùng cần đđăng workshop"
                );
            }

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new WorkshopResponseDto
            {
                Id = workshop.Id,
                Name = workshop.Name,
                Description = workshop.Description,
                Content = workshop.Content,
                Status = workshop.Status,
                CraftVillageId = workshop.CraftVillageId
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

    #region WorkshopActivityService
    public async Task<List<ActivityResponseDto>> CreateActivitiesAsync(Guid workshopId, List<CreateActivityDto> dtos)
    {
        var workshop = await _unitOfWork.WorkshopRepository
        .ActiveEntities
        .Include(w => w.WorkshopActivities)
        .Include(w => w.Bookings)
            .ThenInclude(b => b.User)
        .FirstOrDefaultAsync(w => w.Id == workshopId)
        ?? throw CustomExceptionFactory.CreateBadRequestError("Workshop không tồn tại.");

        // Validate 
        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.Activity))
                throw CustomExceptionFactory.CreateBadRequestError($"Tên hoạt động là bắt buộc cho hoạt động trong ngày {dto.DayOrder}.");
            if (dto.DayOrder < 1)
                throw CustomExceptionFactory.CreateBadRequestError($"DayOrder phải là số dương cho hoạt động {dto.Activity}.");
            if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
                throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho hoạt động {dto.Activity}.");
        }

        // Check thời gian trong cùng 1 ngày dayorder
        var groupedByDay = dtos.GroupBy(d => d.DayOrder);
        foreach (var group in groupedByDay)
        {
            var activitiesInDay = group.OrderBy(a => a.StartTime).ToList();
            for (int i = 1; i < activitiesInDay.Count; i++)
            {
                if (activitiesInDay[i].StartTime < activitiesInDay[i - 1].EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"Phát hiện thời gian trùng lặp trong DayOrder {group.Key}.");
            }
        }

        // Check tổng
        var existingActivities = workshop.WorkshopActivities
            .Where(a => dtos.Any(d => d.DayOrder == a.DayOrder))
            .ToList();
        foreach (var group in groupedByDay)
        {
            var existingInDay = existingActivities.Where(a => a.DayOrder == group.Key).OrderBy(a => a.StartTime).ToList();
            var newInDay = group.OrderBy(a => a.StartTime).ToList();
            var allInDay = existingInDay.Select(a => (a.StartTime, a.EndTime)).Concat(newInDay.Select(a => (a.StartTime, a.EndTime))).OrderBy(t => t.StartTime).ToList();
            for (int i = 1; i < allInDay.Count; i++)
            {
                if (allInDay[i].StartTime < allInDay[i - 1].EndTime)
                    throw CustomExceptionFactory.CreateBadRequestError($"Hoạt động mới trùng lặp với hoạt động hiện có trong DayOrder {group.Key}.");
            }
        }

        var activities = dtos.Select(dto => new WorkshopActivity
        {
            WorkshopId = workshopId,
            Activity = dto.Activity,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            DayOrder = dto.DayOrder
        }).ToList();

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                foreach (var activity in activities)
                {
                    await _unitOfWork.WorkshopActivityRepository.AddAsync(activity);
                }
                await _unitOfWork.SaveAsync();

                // Send notification if there are bookings
                if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                {
                    foreach (var booking in workshop.Bookings)
                    {
                        await _emailService.SendEmailAsync(
                            new[] { booking.User!.Email },
                            $"Cập nhật Workshop {workshop.Name}",
                            $"Workshop {workshop.Name} có thêm hoạt động mới. Vui lòng kiểm tra chi tiết."
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

        return activities.Select(a => new ActivityResponseDto
        {
            ActivityId = a.Id,
            Activity = a.Activity,
            Description = a.Description,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Notes = a.Notes,
            DayOrder = a.DayOrder
        }).ToList();
    }

    public async Task<List<ActivityResponseDto>> GetActivitiesAsync(Guid workshopId)
    {
        try
        {
            var workshop = await _unitOfWork.WorkshopRepository
                .ActiveEntities
                .FirstOrDefaultAsync(w => w.Id == workshopId)
                ?? throw CustomExceptionFactory.CreateBadRequestError("Workshop");

            var activities = await _unitOfWork.WorkshopActivityRepository
                .ActiveEntities
                .Where(a => a.WorkshopId == workshopId && !a.IsDeleted)
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
                .ToListAsync();

            return activities;
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

    public async Task<ActivityResponseDto> UpdateActivityAsync(Guid workshopId, Guid activityId, CreateActivityDto dto)
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

        if (string.IsNullOrWhiteSpace(dto.Activity))
            throw CustomExceptionFactory.CreateBadRequestError($"Tên hoạt động là bắt buộc cho DayOrder {dto.DayOrder}.");
        if (dto.DayOrder < 1)
            throw CustomExceptionFactory.CreateBadRequestError($"DayOrder phải là số dương cho hoạt động {dto.Activity}.");
        if (dto.StartTime.HasValue && dto.EndTime.HasValue && dto.StartTime >= dto.EndTime)
            throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho hoạt động {dto.Activity}.");

        // Check for overlapping times
        var otherActivities = workshop.WorkshopActivities
            .Where(a => a.Id != activityId && a.DayOrder == dto.DayOrder)
            .OrderBy(a => a.StartTime)
            .ToList();
        if (dto.StartTime.HasValue && dto.EndTime.HasValue)
        {
            foreach (var other in otherActivities)
            {
                if (other.StartTime.HasValue && other.EndTime.HasValue &&
                    (dto.StartTime < other.EndTime && dto.EndTime > other.StartTime))
                    throw CustomExceptionFactory.CreateBadRequestError($"Hoạt động được cập nhật trùng lặp với hoạt động hiện có trong DayOrder {dto.DayOrder}.");
            }
        }

        activity.Activity = dto.Activity;
        activity.Description = dto.Description;
        activity.StartTime = dto.StartTime;
        activity.EndTime = dto.EndTime;
        activity.Notes = dto.Notes;
        activity.DayOrder = dto.DayOrder;
        activity.LastUpdatedTime = DateTimeOffset.UtcNow;

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                await _unitOfWork.SaveAsync();

                // Send notification if there are bookings
                if (workshop.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
                {
                    foreach (var booking in workshop.Bookings)
                    {
                        await _emailService.SendEmailAsync(
                            new[] { booking.User!.Email },
                            $"Cập nhật Workshop {workshop.Name}",
                            $"Hoạt động {activity.Activity} trong workshop {workshop.Name} đã được cập nhật. Vui lòng kiểm tra chi tiết."
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

        return new ActivityResponseDto
        {
            ActivityId = activity.Id,
            Activity = activity.Activity,
            Description = activity.Description,
            StartTime = activity.StartTime,
            EndTime = activity.EndTime,
            Notes = activity.Notes,
            DayOrder = activity.DayOrder
        };
    }

    public async Task<List<ActivityResponseDto>> UpdateActivitiesAsync(Guid workshopId, List<UpdateActivityRequestDto> dtos)
    {
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

            var allActivities = existingActivities
                .Where(a => !toDelete.Contains(a))
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

            var maxDayOrder = Math.Max(
                existingActivities.Any() ? existingActivities.Max(a => a.DayOrder) : 0,
                toAdd.Any() ? toAdd.Max(a => a.DayOrder) : 0
            );
            maxDayOrder = Math.Max(
                maxDayOrder,
                toUpdate.Any() ? toUpdate.Max(u => u.DayOrder) : 0
            );
            var scheduleDates = workshop.WorkshopSchedules.Select(s => s.StartTime.Date).Distinct().Count();
            if (scheduleDates < maxDayOrder)
                throw CustomExceptionFactory.CreateBadRequestError($"Không đủ lịch trình ({scheduleDates}) để đáp ứng DayOrder ({maxDayOrder}).");

            // Apply changes in a transaction
            var changes = new List<string>();
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
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
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

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
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task DeleteActivityAsync(Guid workshopId, Guid activityId)
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

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
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
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    #endregion

    #region WorkshopScheduleService

    public async Task<List<ScheduleResponseDto>> CreateSchedulesAsync(Guid workshopId, List<CreateScheduleDto> dtos)
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
            if (dto.StartTime >= dto.EndTime)
                throw CustomExceptionFactory.CreateBadRequestError($"Thời gian kết thúc phải sau thời gian bắt đầu cho lịch trình bắt đầu lúc {dto.StartTime}.");
            if (dto.MaxParticipant <= 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Số lượng người tham gia tối đa phải là số dương cho lịch trình bắt đầu lúc {dto.StartTime}.");
            if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
                throw CustomExceptionFactory.CreateBadRequestError($"Giá không được âm cho lịch trình bắt đầu lúc {dto.StartTime}.");
        }

        var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
        var newScheduleDates = dtos.Select(d => d.StartTime.Date).Distinct().ToList();
        var existingScheduleDates = workshop.WorkshopSchedules.Select(s => s.StartTime.Date).Distinct().ToList();
        var allScheduleDates = newScheduleDates.Concat(existingScheduleDates).Distinct().ToList();
        if (allScheduleDates.Count < maxDayOrder)
            throw CustomExceptionFactory.CreateBadRequestError($"Không đủ ngày lịch trình ({allScheduleDates.Count}) để đáp ứng DayOrder ({maxDayOrder}).");

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

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
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
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

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

        if (dto.StartTime >= dto.EndTime)
            throw CustomExceptionFactory.CreateBadRequestError("Thời gian kết thúc phải sau thời gian bắt đầu.");
        if (dto.MaxParticipant <= 0)
            throw CustomExceptionFactory.CreateBadRequestError("Số lượng người tham gia tối đa phải là số dương.");
        if (dto.AdultPrice < 0 || dto.ChildrenPrice < 0)
            throw CustomExceptionFactory.CreateBadRequestError("Giá không được âm.");

        // check
        var otherSchedules = workshop.WorkshopSchedules.Where(s => s.Id != scheduleId).Select(s => s.StartTime.Date).Distinct().ToList();
        var allScheduleDates = otherSchedules.Concat(new[] { dto.StartTime.Date }).Distinct().ToList();
        var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
        if (allScheduleDates.Count < maxDayOrder)
            throw CustomExceptionFactory.CreateBadRequestError($"Không đủ ngày lịch trình ({allScheduleDates.Count}) để đáp ứng DayOrder ({maxDayOrder}).");

        schedule.StartTime = dto.StartTime;
        schedule.EndTime = dto.EndTime;
        schedule.MaxParticipant = dto.MaxParticipant;
        schedule.AdultPrice = dto.AdultPrice;
        schedule.ChildrenPrice = dto.ChildrenPrice;
        schedule.Notes = dto.Notes;
        schedule.LastUpdatedTime = DateTimeOffset.UtcNow;

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
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
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

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

    public async Task DeleteScheduleAsync(Guid workshopId, Guid scheduleId)
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

        if (schedule.CurrentBooked > 0)
            throw new InvalidOperationException("Không thể xóa lịch trình đã có người đặt.");

        var remainingSchedules = workshop.WorkshopSchedules.Where(s => s.Id != scheduleId).Select(s => s.StartTime.Date).Distinct().ToList();
        var maxDayOrder = workshop.WorkshopActivities.Any() ? workshop.WorkshopActivities.Max(a => a.DayOrder) : 0;
        if (remainingSchedules.Count < maxDayOrder)
            throw CustomExceptionFactory.CreateBadRequestError($"Không thể xóa schedule: Not enough remaining schedules ({remainingSchedules.Count}) to cover DayOrder ({maxDayOrder}).");

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
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
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

    #endregion
}
