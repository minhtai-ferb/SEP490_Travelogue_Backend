using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ReviewModels;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.Implementations;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourGuideService
{
    Task<TourGuideDetailResponse?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken);
    Task<TourGuideDataModel?> AssignToTourGuideAsync(List<string> emails, CancellationToken cancellationToken);
    // Task<TourGuideDataModel> AddTourGuideAsync(TourGuideCreateModel tourGuideCreateModel, CancellationToken cancellationToken);
    Task<TourGuideDataModel?> UpdateTourGuideAsync(TourGuideUpdateModel tourGuideUpdateModel, CancellationToken cancellationToken);
    // Task DeleteTourGuideAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TourGuideDataModel>> GetTourGuidesByFilterAsync(TourGuideFilterRequest request, CancellationToken cancellationToken);
    Task<CertificationDto> AddCertificationAsync(CertificationDto dto, CancellationToken cancellationToken);
    Task<CertificationDto> SoftDeleteCertificationAsync(Guid certificationId, CancellationToken cancellationToken);
    Task<PagedResult<TourGuideDataModel>> GetPagedTourGuideWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<BookingPriceRequestResponseDto> CreateBookingPriceRequestAsync(BookingPriceRequestCreateDto dto);
    Task<BookingPriceRequestResponseDto> ApproveBookingPriceRequestAsync(Guid requestId);
    Task<BookingPriceRequestResponseDto> RejectBookingPriceRequestAsync(Guid requestId, RejectBookingPriceRequestDto dto);
    Task<List<TourGuideScheduleResponseDto>> GetSchedulesAsync(TourGuideScheduleFilterDto filter);
    Task<RejectionRequestResponseDto> CreateRejectionRequestAsync(RejectionRequestCreateDto dto);
    Task<RejectionRequestResponseDto> ApproveRejectionRequestAsync(Guid requestId, Guid newTourGuideId);
    Task<RejectionRequestResponseDto> RejectRejectionRequestAsync(Guid requestId, RejectRejectionRequestDto dto);
    Task<PagedResult<RejectionRequestResponseDto>> GetRejectionRequestsForAdminAsync(RejectionRequestFilter? filter, int pageNumber, int pageSize);
    Task<RejectionRequestResponseDto> GetRejectionRequestByIdAsync(Guid requestId);
    Task<ScheduleWithRejectionResponseDto> GetShedulesById(Guid tourGuideSchedulesId, CancellationToken cancellationToken);
}

public class TourGuideService : ITourGuideService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IEmailService _emailService;
    private readonly IEnumService _enumService;

    public TourGuideService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, IEmailService emailService, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        // ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _emailService = emailService;
        _enumService = enumService;
    }

    public async Task<TourGuideDataModel?> AssignToTourGuideAsync(List<string> emails, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var users = await _unitOfWork.UserRepository.ActiveEntities
                .Where(u => emails.Contains(u.Email))
                .ToListAsync(cancellationToken);

            if (users == null || users.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("No users found with the provided emails.");
            }

            var roleTourGuide = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.TOUR_GUIDE);

            if (roleTourGuide == null)
            {

                roleTourGuide = new Role(AppRole.TOUR_GUIDE, true);
                // Thêm mới role tour guide
                roleTourGuide.Description = "Tour Guide Role";
                roleTourGuide.CreatedBy = currentUserId;
                roleTourGuide.LastUpdatedBy = currentUserId;
                roleTourGuide.CreatedTime = currentTime;
                roleTourGuide.LastUpdatedTime = currentTime;

                await _unitOfWork.RoleRepository.AddAsync(roleTourGuide);
                await _unitOfWork.SaveAsync();
            }

            var tourGuides = new List<TourGuide>();
            foreach (var user in users)
            {
                var existingTourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(user.Id);
                if (existingTourGuide != null)
                {
                    continue; // User is already a tour guide
                }

                var newTourGuide = new TourGuide
                {
                    UserId = user.Id,
                    User = user
                };
                tourGuides.Add(newTourGuide);

                // Check if the user already has the tour guide role
                // If not, create a new UserRole for the tour guide
                var existingUserRole = await _unitOfWork.UserRoleRepository.ActiveEntities
                    .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == roleTourGuide.Id, cancellationToken);

                if (existingUserRole == null)
                {
                    var newUserRoles = new UserRole
                    {
                        RoleId = roleTourGuide.Id,
                        UserId = user.Id,
                        CreatedBy = currentUserId,
                        LastUpdatedBy = currentUserId,
                        CreatedTime = currentTime,
                        LastUpdatedTime = currentTime
                    };
                    await _unitOfWork.UserRoleRepository.AddAsync(newUserRoles);
                }
            }

            if (tourGuides.Count > 0)
            {
                await _unitOfWork.TourGuideRepository.AddRangeAsync(tourGuides);
                await _unitOfWork.SaveAsync();
            }

            await transaction.CommitAsync(cancellationToken);
            return _mapper.Map<TourGuideDataModel>(tourGuides.FirstOrDefault());
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

    public async Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTourGuides = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .ToListAsync(cancellationToken);

            if (existingTourGuides == null || !existingTourGuides.Any())
            {
                return new List<TourGuideDataModel>();
            }

            var tourGuideIds = existingTourGuides.Select(tg => tg.Id).ToList();

            var allReviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Booking)
                .Where(r => r.Booking.TourGuideId.HasValue && tourGuideIds.Contains(r.Booking.TourGuideId.Value))
                .ToListAsync(cancellationToken);

            var result = new List<TourGuideDataModel>();

            foreach (var tg in existingTourGuides)
            {
                var reviewsForGuide = allReviews.Where(r => r.Booking.TourGuideId == tg.Id).ToList();

                var model = new TourGuideDataModel
                {
                    Id = tg.Id,
                    Email = tg.User.Email,
                    UserName = tg.User.FullName,
                    Sex = tg.User.Sex,
                    Address = tg.User.Address,
                    // AverageRating = tg.Rating,
                    Price = tg.Price,
                    Introduction = tg.Introduction,
                    AvatarUrl = tg.User.AvatarUrl,
                    TotalReviews = reviewsForGuide.Count,
                    AverageRating = reviewsForGuide.Any() ? reviewsForGuide.Average(r => r.Rating) : 0.0
                };

                result.Add(model);
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

    public async Task<List<TourGuideDataModel>> GetTourGuidesByFilterAsync(TourGuideFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var existingTourGuides = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .Include(tg => tg.TourGuideSchedules)
                .ToListAsync(cancellationToken);
            if (existingTourGuides == null || existingTourGuides.Count() == 0)
            {
                return new List<TourGuideDataModel>();
            }

            existingTourGuides = existingTourGuides
                .Where(tg => string.IsNullOrEmpty(request.FullName) || tg.User.FullName.ToLower().Contains(request.FullName.ToLower()))
                .Where(tg => !request.MinRating.HasValue || tg.Rating >= request.MinRating.Value)
                .Where(tg => !request.MaxRating.HasValue || tg.Rating <= request.MaxRating.Value)
                .Where(tg => !request.MinPrice.HasValue || tg.Price >= request.MinPrice.Value)
                .Where(tg => !request.MaxPrice.HasValue || tg.Price <= request.MaxPrice.Value)
                .Where(tg => !request.Gender.HasValue || tg.User.Sex == request.Gender.Value)
                .Where(tg => !request.StartDate.HasValue || !request.EndDate.HasValue ||
                     !tg.TourGuideSchedules.Any(s => s.Date >= request.StartDate.Value && s.Date <= request.EndDate.Value))
                .ToList();

            var tourGuideIds = existingTourGuides.Select(tg => tg.Id).ToList();

            var allReviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.Booking)
                .Where(r => r.Booking.TourGuideId.HasValue && tourGuideIds.Contains(r.Booking.TourGuideId.Value))
                .ToListAsync(cancellationToken);

            var result = new List<TourGuideDataModel>();

            foreach (var tg in existingTourGuides)
            {
                var reviewsForGuide = allReviews.Where(r => r.Booking.TourGuideId == tg.Id).ToList();

                var guideModel = new TourGuideDataModel
                {
                    Id = tg.Id,
                    Email = tg.User.Email,
                    UserName = tg.User.FullName,
                    Sex = tg.User.Sex,
                    Address = tg.User.Address,
                    // AverageRating = tg.Rating,
                    Price = tg.Price,
                    Introduction = tg.Introduction,
                    AvatarUrl = tg.User.AvatarUrl,
                    TotalReviews = reviewsForGuide.Count,
                    AverageRating = reviewsForGuide.Any() ? Math.Round(reviewsForGuide.Average(r => r.Rating), 2) : 0.0
                };

                result.Add(guideModel);
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
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<TourGuideDataModel>> GetPagedTourGuideWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var tourGuide = _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .Where(tg => string.IsNullOrEmpty(name) || tg.User.FullName.ToLower().Contains(name.ToLower()));

            if (tourGuide == null)
            {
                return new PagedResult<TourGuideDataModel>
                {
                    Items = new List<TourGuideDataModel>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var tourGuideDetailResponse = _mapper.Map<TourGuideDataModel>(tourGuide);

            var totalCount = await tourGuide.CountAsync(cancellationToken);

            return new PagedResult<TourGuideDataModel>
            {
                Items = await tourGuide
                    .OrderBy(tg => tg.User.FullName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(tg => _mapper.Map<TourGuideDataModel>(tg))
                    .ToListAsync(cancellationToken),
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

    public async Task<TourGuideDetailResponse?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .Include(tg => tg.Bookings)
                .FirstOrDefaultAsync(tg => tg.Id == id, cancellationToken);

            if (tourGuide == null)
            {
                return null;
            }

            var reviews = await _unitOfWork.ReviewRepository.ActiveEntities
                .Include(r => r.User)
                .Where(r => r.Booking.TourGuideId == id)
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

            var tourGuideDetail = new TourGuideDetailResponse
            {
                Id = tourGuide.Id,
                Email = tourGuide.User.Email,
                UserName = tourGuide.User.FullName,
                Sex = tourGuide.User.Sex,
                Address = tourGuide.User.Address,
                Rating = tourGuide.Rating,
                Price = tourGuide.Price,
                Introduction = tourGuide.Introduction,
                AvatarUrl = tourGuide.User.AvatarUrl,
                Reviews = rating.Reviews,
                TotalReviews = rating.TotalReviews,
                AverageRating = rating.AverageRating,
            };

            await transaction.CommitAsync(cancellationToken);
            return tourGuideDetail;
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

    public async Task<TourGuideDataModel?> UpdateTourGuideAsync(TourGuideUpdateModel tourGuideUpdateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("user");

            user.Email = tourGuideUpdateModel.Email;
            user.PhoneNumber = tourGuideUpdateModel.PhoneNumber;
            user.FullName = tourGuideUpdateModel.FullName;
            user.Sex = tourGuideUpdateModel.Sex;
            user.Address = tourGuideUpdateModel.Address;

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(currentUserId)
                ?? throw CustomExceptionFactory.CreateForbiddenError();

            tourGuide.Introduction = tourGuideUpdateModel.Introduction;
            // tourGuide.Languages = tourGuideUpdateModel.Languages;
            // tourGuide.Tags = tourGuideUpdateModel.Tags;

            await _unitOfWork.UserRepository.UpdateAsync(user);
            _unitOfWork.TourGuideRepository.Update(tourGuide);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var updatedTourGuide = _mapper.Map<TourGuideDataModel>(tourGuide);
            return updatedTourGuide;
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

    public async Task<BookingPriceRequestResponseDto> CreateBookingPriceRequestAsync(BookingPriceRequestCreateDto dto)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentUserIdGuid = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isTourGuide = _userContextService.HasRole(AppRole.TOUR_GUIDE);
            if (!isTourGuide)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.UserId == currentUserIdGuid)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            // còn yêu cầu đang pending
            var existingPendingRequest = await _unitOfWork.BookingPriceRequestRepository
                .ActiveEntities
                .AnyAsync(r => r.TourGuideId == tourGuide.Id && r.Status == BookingPriceRequestStatus.Pending);
            if (existingPendingRequest)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đã có một yêu cầu giá đang chờ duyệt. Vui lòng chờ hoặc liên hệ Moderator.");
            }

            if (dto.Price < 10000)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Giá phải lớn hơn hoặc bằng 10000.");
            }

            var request = new BookingPriceRequest
            {
                Id = Guid.NewGuid(),
                TourGuideId = tourGuide.Id,
                Price = dto.Price,
                Status = BookingPriceRequestStatus.Pending,
            };

            await _unitOfWork.BookingPriceRequestRepository.AddAsync(request);
            await _unitOfWork.SaveAsync();

            // Gửi email thông báo cho Moderator
            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                   new[] { moderator.Email },
                    "Có người dùng cần đăng ký role",
                    "Có người dùng cần đăng ký role"
                );
            }

            var response = new BookingPriceRequestResponseDto
            {
                TourGuideId = request.TourGuideId,
                Price = request.Price,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingPriceRequestStatus>(request.Status),
                RejectionReason = request.RejectionReason,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<BookingPriceRequestResponseDto> ApproveBookingPriceRequestAsync(Guid requestId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isModerator = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!isModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var request = await _unitOfWork.BookingPriceRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Yêu cầu giá không tồn tại.");

            if (request.Status != BookingPriceRequestStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu giá đã được xử lý.");
            }

            request.Status = BookingPriceRequestStatus.Approved;
            request.ReviewedBy = currentUserId;
            request.ReviewedAt = currentTime;

            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == request.TourGuideId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            tourGuide.Price = request.Price;

            await _unitOfWork.SaveAsync();
            await _emailService.SendEmailAsync(
                new[] { tourGuide.User.Email },
                $"Giá booking của bạn đã được duyệt",
                $"Giá {request.Price} đã được duyệt và sẽ hiển thị cho khách hàng."
            );

            var response = new BookingPriceRequestResponseDto
            {
                TourGuideId = request.TourGuideId,
                Price = request.Price,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingPriceRequestStatus>(request.Status),
                RejectionReason = request.RejectionReason,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<BookingPriceRequestResponseDto> RejectBookingPriceRequestAsync(Guid requestId, RejectBookingPriceRequestDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isModerator = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!isModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var request = await _unitOfWork.BookingPriceRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Yêu cầu giá không tồn tại.");

            if (request.Status != BookingPriceRequestStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu giá đã được xử lý.");
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Lý do từ chối không được để trống.");
            }

            request.Status = BookingPriceRequestStatus.Rejected;
            request.ReviewedBy = currentUserId;
            request.ReviewedAt = currentTime;
            request.RejectionReason = dto.Reason;

            await _unitOfWork.SaveAsync();

            // Gửi email thông báo cho Tour Guide
            await _emailService.SendEmailAsync(
                new[] { request.TourGuide.User.Email },
                $"Giá booking của bạn đã bị từ chối",
                $"Giá {request.Price} đã bị từ chối. Lý do: {request.RejectionReason}. Vui lòng chỉnh sửa và gửi lại."
            );

            var response = new BookingPriceRequestResponseDto
            {
                TourGuideId = request.TourGuideId,
                Price = request.Price,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingPriceRequestStatus>(request.Status),
                RejectionReason = request.RejectionReason,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<RejectionRequestResponseDto> CreateRejectionRequestAsync(RejectionRequestCreateDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isTourGuide = _userContextService.HasRole(AppRole.TOUR_GUIDE);
            if (!isTourGuide)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            // Lấy TourGuide kèm User
            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == currentUserId)
                .ConfigureAwait(false)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            // kiểm tra xem có yêu cầu trước đó chưa duyệt
            var existingPendingRequest = await _unitOfWork.RejectionRequestRepository
                .ActiveEntities
                .AnyAsync(r => r.TourGuideId == tourGuide.Id &&
                    r.Status == RejectionRequestStatus.Pending &&
                    ((dto.RequestType == RejectionRequestType.TourSchedule && r.TourScheduleId == dto.TourScheduleId) ||
                     (dto.RequestType == RejectionRequestType.Booking && r.BookingId == dto.BookingId)));

            if (existingPendingRequest)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đã có một yêu cầu từ chối đang chờ duyệt cho mục này.");
            }

            // Kiểm tra input
            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Lý do từ chối không được để trống.");
            }

            if (dto.RequestType == RejectionRequestType.TourSchedule)
            {
                var scheduleExists = await _unitOfWork.TourGuideScheduleRepository
                    .ActiveEntities
                    .AnyAsync(s => s.TourScheduleId == dto.TourScheduleId && s.TourGuideId == tourGuide.Id)
                    .ConfigureAwait(false);

                if (!scheduleExists)
                    throw CustomExceptionFactory.CreateNotFoundError("Tour Schedule không tồn tại hoặc không thuộc về bạn.");
            }
            else if (dto.RequestType == RejectionRequestType.Booking)
            {
                var booking = await _unitOfWork.BookingRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.TourGuideId == tourGuide.Id)
                    .ConfigureAwait(false)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Booking không tồn tại hoặc không thuộc về bạn.");
            }
            else
            {
                throw CustomExceptionFactory.CreateBadRequestError("Loại yêu cầu không hợp lệ.");
            }

            var request = new RejectionRequest
            {
                Id = Guid.NewGuid(),
                TourGuideId = tourGuide.Id,
                RequestType = dto.RequestType,
                TourScheduleId = dto.RequestType == RejectionRequestType.TourSchedule ? dto.TourScheduleId : null,
                BookingId = dto.RequestType == RejectionRequestType.Booking ? dto.BookingId : null,
                Reason = dto.Reason,
                Status = RejectionRequestStatus.Pending,
                CreatedTime = currentTime
            };

            await _unitOfWork.RejectionRequestRepository.AddAsync(request);
            await _unitOfWork.SaveAsync();

            // Gửi email thông báo cho Moderator
            var moderators = await _unitOfWork.UserRepository.GetUsersByRoleAsync(AppRole.MODERATOR);
            var tourGuideName = tourGuide.User?.FullName ?? "Tour Guide";

            foreach (var moderator in moderators)
            {
                await _emailService.SendEmailAsync(
                    new[] { moderator.Email },
                    $"Yêu cầu từ chối {dto.RequestType} từ Tour Guide {tourGuideName}",
                    $"Tour Guide {tourGuideName} muốn từ chối {dto.RequestType} (ID: {(dto.RequestType == RejectionRequestType.TourSchedule ? dto.TourScheduleId : dto.BookingId)}). " +
                    $"Lý do: {dto.Reason}. Xem chi tiết tại /admin/rejection-requests/{request.Id}"
                );
            }

            var response = new RejectionRequestResponseDto
            {
                Id = request.Id,
                TourGuideId = request.TourGuideId,
                RequestType = request.RequestType,
                TourScheduleId = request.TourScheduleId,
                BookingId = request.BookingId,
                Reason = request.Reason,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(request.Status),
                ModeratorComment = request.ModeratorComment,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<RejectionRequestResponseDto> ApproveRejectionRequestAsync(Guid requestId, Guid newTourGuideId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isModerator = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!isModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var request = await _unitOfWork.RejectionRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                .ThenInclude(t => t.User)
                .Include(r => r.TourSchedule)
                .Include(r => r.Booking)
                .ThenInclude(b => b != null ? b.User : null)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Yêu cầu từ chối không tồn tại.");

            if (request.Status != RejectionRequestStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu từ chối đã được xử lý.");
            }

            request.Status = RejectionRequestStatus.Approved;
            request.ReviewedBy = currentUserId;
            request.ReviewedAt = currentTime;

            if (request.RequestType == RejectionRequestType.TourSchedule && request.TourSchedule != null)
            {
                var guideSchedules = await _unitOfWork.TourGuideScheduleRepository
                    .ActiveEntities
                    .Where(x => x.TourScheduleId == request.TourSchedule.Id)
                    .ToListAsync();

                foreach (var guideSchedule in guideSchedules)
                {
                    guideSchedule.IsDeleted = true;
                    guideSchedule.DeletedBy = currentUserId.ToString();
                    guideSchedule.DeletedTime = currentTime;
                    _unitOfWork.TourGuideScheduleRepository.Update(guideSchedule);
                }

                var newTourGuide = await _unitOfWork.TourGuideRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(tg => tg.Id == newTourGuideId)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide mới");

                if (newTourGuide.Id == request.TourGuideId)
                    throw CustomExceptionFactory.CreateBadRequestError("Tour Guide mới không được trùng với người từ chối.");

                bool alreadyAssigned = await _unitOfWork.TourGuideScheduleRepository
                    .ActiveEntities
                    .AnyAsync(s => s.TourScheduleId == request.TourSchedule.Id && s.TourGuideId == newTourGuideId);
                if (alreadyAssigned)
                    throw CustomExceptionFactory.CreateBadRequestError("Tour Guide mới đã có lịch này.");

                var newSchedule = new TourGuideSchedule
                {
                    Id = Guid.NewGuid(),
                    TourScheduleId = request.TourSchedule.Id,
                    TourGuideId = newTourGuideId,
                    CreatedBy = currentUserId.ToString(),
                    CreatedTime = currentTime
                };
                await _unitOfWork.TourGuideScheduleRepository.AddAsync(newSchedule);
            }
            else if (request.RequestType == RejectionRequestType.Booking && request.Booking != null)
            {
                request.Booking.Status = BookingStatus.Cancelled;
                _unitOfWork.BookingRepository.Update(request.Booking);

                await _emailService.SendEmailAsync(
                    new[] { request.Booking.User.Email },
                    $"Booking của bạn đã bị từ chối",
                    $"Booking (ID: {request.BookingId}) đã bị từ chối bởi Tour Guide {request.TourGuide.User.FullName}. Lý do: {request.Reason}."
                );
            }

            await _unitOfWork.SaveAsync();

            // Gửi email 
            await _emailService.SendEmailAsync(
                new[] { request.TourGuide.User.Email },
                $"Yêu cầu từ chối {request.RequestType} của bạn đã được duyệt",
                $"Yêu cầu từ chối {request.RequestType} (ID: {(request.RequestType == RejectionRequestType.TourSchedule ? request.TourScheduleId : request.BookingId)}) đã được duyệt."
            );

            var response = new RejectionRequestResponseDto
            {
                TourGuideId = request.TourGuideId,
                RequestType = request.RequestType,
                TourScheduleId = request.TourScheduleId,
                BookingId = request.BookingId,
                Reason = request.Reason,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(request.Status),
                ModeratorComment = request.ModeratorComment,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<RejectionRequestResponseDto> RejectRejectionRequestAsync(Guid requestId, RejectRejectionRequestDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var isModerator = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!isModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var request = await _unitOfWork.RejectionRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Yêu cầu từ chối không tồn tại.");

            if (request.Status != RejectionRequestStatus.Pending)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu từ chối đã được xử lý.");
            }

            if (string.IsNullOrWhiteSpace(dto.ModeratorComment))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Lý do từ chối không được để trống.");
            }

            request.Status = RejectionRequestStatus.Rejected;
            request.ReviewedBy = currentUserId;
            request.ReviewedAt = currentTime;
            request.ModeratorComment = dto.ModeratorComment;

            await _unitOfWork.SaveAsync();

            // Gửi email 
            await _emailService.SendEmailAsync(
                new[] { request.TourGuide.User.Email },
                $"Yêu cầu từ chối {request.RequestType} của bạn đã bị từ chối",
                $"Yêu cầu từ chối {request.RequestType} (ID: {(request.RequestType == RejectionRequestType.TourSchedule ? request.TourScheduleId : request.BookingId)}) đã bị từ chối. Lý do: {request.ModeratorComment}."
            );

            var response = new RejectionRequestResponseDto
            {
                TourGuideId = request.TourGuideId,
                RequestType = request.RequestType,
                TourScheduleId = request.TourScheduleId,
                BookingId = request.BookingId,
                Reason = request.Reason,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(request.Status),
                ModeratorComment = request.ModeratorComment,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
            };

            return response;
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

    public async Task<List<TourGuideScheduleResponseDto>> GetSchedulesAsync(TourGuideScheduleFilterDto filter)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isTourGuide = _userContextService.HasRole(AppRole.TOUR_GUIDE);
            if (!isTourGuide)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.UserId == currentUserId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            IQueryable<TourGuideSchedule> query = _unitOfWork.TourGuideScheduleRepository
                .ActiveEntities
                .Where(s => s.TourGuideId == tourGuide.Id)
                .Include(s => s.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User);

            if (filter.FilterType == ScheduleFilterType.TourSchedule)
            {
                query = query.Where(s => s.TourScheduleId != null);
            }
            else if (filter.FilterType == ScheduleFilterType.Booking)
            {
                query = query.Where(s => s.BookingId != null);
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                if (filter.StartDate > filter.EndDate)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                }
                query = query.Where(s => s.Date >= filter.StartDate.Value && s.Date <= filter.EndDate.Value);
            }
            else if (filter.StartDate.HasValue)
            {
                query = query.Where(s => s.Date >= filter.StartDate.Value);
            }
            else if (filter.EndDate.HasValue)
            {
                query = query.Where(s => s.Date <= filter.EndDate.Value);
            }

            var schedules = await query
                .OrderBy(s => s.Date)
                .ToListAsync();

            var result = schedules.Select(s => new TourGuideScheduleResponseDto
            {
                Id = s.Id,
                TourGuideId = s.TourGuideId,
                TourScheduleId = s.TourScheduleId,
                BookingId = s.BookingId,
                Date = s.Date,
                Note = s.Note,
                TourName = s.TourSchedule != null ? s.TourSchedule.Tour?.Name : null,
                CustomerName = s.Booking != null ? s.Booking.User?.FullName : null,
                Price = s.TourSchedule != null ? s.TourSchedule.AdultPrice : (s.Booking != null ? s.Booking.FinalPrice : null),
                ScheduleType = s.TourScheduleId != null ? "TourSchedule" : (s.BookingId != null ? "Booking" : "Unknown")
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

    public async Task<CertificationDto> AddCertificationAsync(CertificationDto dto, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(currentUserId)
                ?? throw CustomExceptionFactory.CreateForbiddenError();

            var cert = new Certification
            {
                Id = Guid.NewGuid(),
                TourGuideId = tourGuide.Id,
                Name = dto.Name,
                CertificateUrl = dto.CertificateUrl,
                CreatedTime = currentTime,
                CreatedBy = currentUserId.ToString()
            };

            await _unitOfWork.CertificationRepository.AddAsync(cert);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new CertificationDto
            {
                Name = cert.Name,
                CertificateUrl = cert.CertificateUrl
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


    public async Task<CertificationDto> SoftDeleteCertificationAsync(Guid certificationId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var cert = await _unitOfWork.CertificationRepository.GetByIdAsync(certificationId, cancellationToken)
                ?? throw new Exception("Certification not found");

            // Optional: Kiểm tra quyền sở hữu certification
            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIdAsync(cert.TourGuideId, cancellationToken);
            if (tourGuide?.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this certification.");

            cert.IsDeleted = true;
            cert.DeletedBy = userId.ToString();
            cert.DeletedTime = currentTime;

            _unitOfWork.CertificationRepository.Update(cert);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            return new CertificationDto
            {
                Name = cert.Name,
                CertificateUrl = cert.CertificateUrl
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

    public async Task<PagedResult<RejectionRequestResponseDto>> GetRejectionRequestsForAdminAsync(RejectionRequestFilter? filter, int pageNumber, int pageSize)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isAllowed = _userContextService.HasAnyRole(AppRole.TOUR_GUIDE, AppRole.MODERATOR, AppRole.ADMIN);
            if (!isAllowed)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            IQueryable<RejectionRequest> query = _unitOfWork.RejectionRequestRepository.ActiveEntities
                .Include(r => r.TourGuide)
                    .ThenInclude(t => t.User)
                .Include(r => r.TourSchedule)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User);

            //Lọc theo trạng thái
            if (filter.Status.HasValue)
            {
                query = query.Where(r => r.Status == filter.Status.Value);
            }

            //Lọc theo tour guide id
            if (filter.TourGuideId.HasValue)
            {
                query = query.Where(r => r.TourGuideId == filter.TourGuideId.Value);
            }

            //Lọc theo ngày
            if (filter.FromDate.HasValue && filter.ToDate.HasValue)
            {
                if (filter.FromDate > filter.ToDate)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                }
                query = query.Where(r => r.CreatedTime >= filter.FromDate.Value && r.CreatedTime <= filter.ToDate.Value);
            }
            else if (filter.FromDate.HasValue)
            {
                query = query.Where(r => r.CreatedTime >= filter.FromDate.Value);
            }
            else if (filter.ToDate.HasValue)
            {
                query = query.Where(r => r.CreatedTime <= filter.ToDate.Value);
            }

            // Tổng số bản ghi trước khi phân trang
            var totalRecords = await query.CountAsync();

            // Phân trang
            var rejectionRequests = await query
                .OrderByDescending(r => r.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<RejectionRequestResponseDto>();

            foreach (var item in rejectionRequests)
            {
                var response = new RejectionRequestResponseDto
                {
                    Id = item.Id,
                    TourGuideId = item.TourGuideId,
                    RequestType = item.RequestType,
                    TourScheduleId = item.TourScheduleId,
                    BookingId = item.BookingId,
                    Reason = item.Reason,
                    Status = item.Status,
                    StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(item.Status),
                    ModeratorComment = item.ModeratorComment,
                    ReviewedAt = item.ReviewedAt,
                    ReviewedBy = item.ReviewedBy,
                };
                result.Add(response);
            }

            return new PagedResult<RejectionRequestResponseDto>
            {
                Items = result,
                TotalCount = totalRecords,
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

    public async Task<RejectionRequestResponseDto> GetRejectionRequestByIdAsync(Guid requestId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var isAllowed = _userContextService.HasAnyRole(AppRole.TOUR_GUIDE, AppRole.MODERATOR, AppRole.ADMIN);
            if (!isAllowed)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var request = await _unitOfWork.RejectionRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                    .ThenInclude(t => t.User)
                .Include(r => r.TourSchedule)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Yêu cầu từ chối của hướng dẫn viên không tồn tại.");
            }

            return new RejectionRequestResponseDto
            {
                Id = request.Id,
                TourGuideId = request.TourGuideId,
                RequestType = request.RequestType,
                TourScheduleId = request.TourScheduleId,
                BookingId = request.BookingId,
                Reason = request.Reason,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(request.Status),
                ModeratorComment = request.ModeratorComment,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy
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

    public async Task<ScheduleWithRejectionResponseDto> GetShedulesById(Guid tourGuideSchedulesId, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isTourGuide = _userContextService.HasRole(AppRole.TOUR_GUIDE);
            if (!isTourGuide)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .FirstOrDefaultAsync(t => t.UserId == currentUserId, cancellationToken)
                .ConfigureAwait(false)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour Guide");

            var tourGuideSchedule = await _unitOfWork.TourGuideScheduleRepository
                .ActiveEntities
                .Include(s => s.TourSchedule)
                    .ThenInclude(ts => ts.Tour)
                .Include(s => s.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(s => s.TourGuideId == tourGuide.Id && s.Id == tourGuideSchedulesId, cancellationToken)
                .ConfigureAwait(false);

            if (tourGuideSchedule == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour Guide Schedule");
            }

            var rejectionQuery = _unitOfWork.RejectionRequestRepository
                .ActiveEntities
                .Include(r => r.TourGuide)
                .Include(r => r.TourSchedule)
                .Include(r => r.Booking)
                .Where(r => r.TourGuideId == tourGuide.Id);

            if (tourGuideSchedule.TourScheduleId != null)
            {
                rejectionQuery = rejectionQuery.Where(r => r.TourScheduleId == tourGuideSchedule.TourScheduleId);
            }
            else if (tourGuideSchedule.BookingId != null)
            {
                rejectionQuery = rejectionQuery.Where(r => r.BookingId == tourGuideSchedule.BookingId);
            }

            var rejection = await rejectionQuery.FirstOrDefaultAsync();

            var result = new ScheduleWithRejectionResponseDto
            {
                Id = tourGuideSchedule.Id,
                TourGuideId = tourGuideSchedule.TourGuideId,
                TourScheduleId = tourGuideSchedule.TourScheduleId,
                BookingId = tourGuideSchedule.BookingId,
                Date = tourGuideSchedule.Date,
                Note = tourGuideSchedule.Note,
                TourName = tourGuideSchedule.TourSchedule?.Tour?.Name,
                CustomerName = tourGuideSchedule.Booking?.User?.FullName,
                Price = tourGuideSchedule.TourSchedule != null
                    ? tourGuideSchedule.TourSchedule.AdultPrice
                    : tourGuideSchedule.Booking?.FinalPrice,
                ScheduleType = tourGuideSchedule.TourScheduleId != null
                    ? "TourSchedule"
                    : tourGuideSchedule.BookingId != null ? "Booking" : "Unknown",
                RejectionRequest = rejection != null
                    ? new RejectionRequestResponseDto
                    {
                        Id = rejection.Id,
                        TourGuideId = rejection.TourGuideId,
                        RequestType = rejection.RequestType,
                        TourScheduleId = rejection.TourScheduleId,
                        BookingId = rejection.BookingId,
                        Reason = rejection.Reason,
                        Status = rejection.Status,
                        StatusText = _enumService.GetEnumDisplayName<RejectionRequestStatus>(rejection.Status),
                        ModeratorComment = rejection.ModeratorComment,
                        ReviewedAt = rejection.ReviewedAt,
                        ReviewedBy = rejection.ReviewedBy
                    }
                    : null
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
    }
}
