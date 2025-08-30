using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.BusinessModels.RefundRequestModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IRefundRequestService
{
    Task<RefundRequestDto> CreateRefundRequestAsync(RefundRequestCreateDto dto);
    Task<RefundRequestDto> ApproveRefundRequestAsync(Guid refundRequestId, string? note);
    Task<RefundRequestDto> RejectRefundRequestAsync(Guid refundRequestId, string rejectionReason);
    Task<List<RefundRequestDto>> GetRefundRequestsForAdminAsync(RefundRequestAdminFilter filter);
    Task<List<RefundRequestDto>> GetRefundRequestsForUserAsync(RefundRequestUserFilter filter);
    Task<RefundRequestDto> GetRefundRequestDetailAsync(Guid refundRequestId);
    Task DeleteRefundRequestAsync(Guid refundRequestId);
}

public class RefundRequestService : IRefundRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IEnumService _enumService;

    public RefundRequestService(IUnitOfWork unitOfWork, IUserContextService userContextService, ITimeService timeService, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _timeService = timeService;
        _enumService = enumService;
    }

    public async Task<RefundRequestDto> CreateRefundRequestAsync(RefundRequestCreateDto dto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var booking = await _unitOfWork.BookingRepository
                .ActiveEntities
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == currentUserId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking");

            var existingRequest = await _unitOfWork.RefundRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId &&
                    (r.Status == RefundRequestStatus.Pending || r.Status == RefundRequestStatus.Approved));

            if (existingRequest != null)
                throw CustomExceptionFactory.CreateBadRequestError("Đã tồn tại yêu cầu hoàn tiền cho booking này.");

            if (!(booking.Status == BookingStatus.Cancelled
                || booking.Status == BookingStatus.CancelledByProvider))
            {
                throw CustomExceptionFactory.CreateBadRequestError("Booking không đủ điều kiện hoàn tiền.");
            }

            // thời gian hoàn tiền trước 48 tiếng
            if (booking.Status == BookingStatus.Confirmed)
            {
                var hoursBefore = (booking.StartDate - DateTime.UtcNow).TotalHours;
                if (hoursBefore < 48)
                    throw CustomExceptionFactory.CreateBadRequestError("Không thể hoàn tiền do hủy quá sát giờ.");
            }

            if (dto.RefundAmount > booking.FinalPrice)
                throw CustomExceptionFactory.CreateBadRequestError("Số tiền hoàn vượt quá số tiền đã thanh toán.");

            var existing = await _unitOfWork.RefundRequestRepository
                .ActiveEntities
                .AnyAsync(r => r.BookingId == dto.BookingId &&
                    (r.Status == RefundRequestStatus.Pending || r.Status == RefundRequestStatus.Approved));

            if (existing)
                throw CustomExceptionFactory.CreateBadRequestError("Đã tồn tại yêu cầu hoàn tiền cho booking này.");

            var refundRequest = new RefundRequest
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                BookingId = dto.BookingId,
                Reason = dto.Reason,
                RefundAmount = dto.RefundAmount,
                Status = RefundRequestStatus.Pending,
                RequestedAt = currentTime,
            };

            await _unitOfWork.RefundRequestRepository.AddAsync(refundRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            var username = await _unitOfWork.UserRepository
                .ActiveEntities
                .Where(u => u.Id == refundRequest.UserId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            return new RefundRequestDto
            {
                Id = refundRequest.Id,
                BookingId = refundRequest.BookingId,
                Reason = refundRequest.Reason,
                UserId = refundRequest.UserId,
                UserName = username ?? string.Empty,
                RefundAmount = refundRequest.RefundAmount,
                Status = refundRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                Note = refundRequest.Note,
                RequestedAt = refundRequest.RequestedAt,
                RespondedAt = refundRequest.RespondedAt,
                CreatedTime = refundRequest.CreatedTime,
                LastUpdatedTime = refundRequest.LastUpdatedTime,
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

    public async Task<RefundRequestDto> ApproveRefundRequestAsync(Guid refundRequestId, string? note)
    {
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var currentTime = _timeService.SystemTimeNow;
                var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
                if (!hasPermission)
                    throw CustomExceptionFactory.CreateForbiddenError();

                var refundRequest = await _unitOfWork.RefundRequestRepository
                    .ActiveEntities
                    .Include(r => r.User)
                        .ThenInclude(u => u.Wallet)
                    .FirstOrDefaultAsync(r => r.Id == refundRequestId)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Refund Request");

                if (refundRequest.Status != RefundRequestStatus.Pending)
                    throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu không ở trạng thái Pending.");

                refundRequest.RespondedAt = currentTime;
                refundRequest.Note = note;
                refundRequest.Status = RefundRequestStatus.Approved;

                if (refundRequest.User.Wallet == null)
                {
                    refundRequest.User.Wallet = new Wallet
                    {
                        UserId = refundRequest.User.Id,
                        Balance = 0
                    };

                    await _unitOfWork.WalletRepository.AddAsync(refundRequest.User.Wallet);
                    await _unitOfWork.SaveAsync();
                }

                refundRequest.User.Wallet.Balance += refundRequest.RefundAmount;

                var walletTransaction = new TransactionEntry
                {
                    Id = Guid.NewGuid(),
                    WalletId = refundRequest.User.Wallet.Id,
                    UserId = refundRequest.User.Id,
                    PaidAmount = refundRequest.RefundAmount,
                    Type = TransactionType.Refund,
                    TransactionDirection = TransactionDirection.Credit,
                    Status = TransactionStatus.Completed,
                    PaymentStatus = PaymentStatus.Success,
                    Description = $"Hoàn tiền cho booking {refundRequest.BookingId}",
                    Method = "Banking",
                    TransactionDateTime = DateTime.UtcNow,
                    Currency = "VND"
                };

                // var systemTransaction = new TransactionEntry
                // {
                //     Id = Guid.NewGuid(),
                //     UserId = null, 
                //     WalletId = null, 
                //     PaidAmount = refundRequest.RefundAmount,
                //     Type = TransactionType.Refund,
                //     TransactionDirection = TransactionDirection.Debit,
                //     Status = TransactionStatus.Completed,
                //     Description = $"Nguồn tiền hoàn cho booking {refundRequest.BookingId} xuất phát từ hệ thống",
                //     Method = "System"
                // };

                // await _unitOfWork.TransactionEntryRepository.AddAsync(systemTransaction);
                await _unitOfWork.TransactionEntryRepository.AddAsync(walletTransaction);
                _unitOfWork.RefundRequestRepository.Update(refundRequest);
                _unitOfWork.UserRepository.Update(refundRequest.User);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return new RefundRequestDto
                {
                    Id = refundRequest.Id,
                    BookingId = refundRequest.BookingId,
                    UserId = refundRequest.UserId,
                    UserName = refundRequest.User.FullName,
                    Reason = refundRequest.Reason,
                    RefundAmount = refundRequest.RefundAmount,
                    Status = refundRequest.Status,
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                    Note = refundRequest.Note,
                    CreatedTime = refundRequest.CreatedTime,
                    LastUpdatedTime = refundRequest.LastUpdatedTime,
                    RequestedAt = refundRequest.RequestedAt,
                    RespondedAt = refundRequest.RespondedAt,
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
    }

    public async Task<RefundRequestDto> RejectRefundRequestAsync(Guid refundRequestId, string rejectionReason)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentTime = _timeService.SystemTimeNow;
            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var refundRequest = await _unitOfWork.RefundRequestRepository
                .ActiveEntities
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == refundRequestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Refund Request");

            if (refundRequest.Status != RefundRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu không ở trạng thái Pending.");

            refundRequest.RespondedAt = currentTime;
            refundRequest.Status = RefundRequestStatus.Rejected;
            refundRequest.Note = rejectionReason;

            _unitOfWork.RefundRequestRepository.Update(refundRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new RefundRequestDto
            {
                Id = refundRequest.Id,
                BookingId = refundRequest.BookingId,
                UserId = refundRequest.UserId,
                Reason = refundRequest.Reason,
                UserName = refundRequest.User.FullName,
                RefundAmount = refundRequest.RefundAmount,
                Status = refundRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                Note = refundRequest.Note,
                CreatedTime = refundRequest.CreatedTime,
                LastUpdatedTime = refundRequest.LastUpdatedTime,
                RequestedAt = refundRequest.RequestedAt,
                RespondedAt = refundRequest.RespondedAt,
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

    public async Task<List<RefundRequestDto>> GetRefundRequestsForAdminAsync(RefundRequestAdminFilter filter)
    {
        try
        {
            var query = _unitOfWork.RefundRequestRepository.ActiveEntities
                .Include(r => r.User)
                .AsQueryable();


            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedTime >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedTime <= filter.ToDate.Value);

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status.Value);

            if (filter.UserId.HasValue)
                query = query.Where(r => r.UserId == filter.UserId.Value);

            var list = await query
                .OrderByDescending(r => r.CreatedTime)
                .Select(r => new RefundRequestDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    UserId = r.UserId,
                    UserName = r.User.FullName,
                    RefundAmount = r.RefundAmount,
                    Status = r.Status,
                    Reason = r.Reason,
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(r.Status),
                    Note = r.Note,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime,
                    RequestedAt = r.RequestedAt,
                    RespondedAt = r.RespondedAt,
                    BookingDataModel = new BusinessModels.BookingModels.BookingDataModel
                    {
                        Id = r.Booking.Id,
                        UserId = r.Booking.UserId,
                        TourId = r.Booking.TourId,
                        TripPlanId = r.Booking.TripPlanId,
                        WorkshopId = r.Booking.WorkshopId,
                        WorkshopScheduleId = r.Booking.WorkshopScheduleId,
                        TourGuideId = r.Booking.TourGuideId,
                        BookingType = r.Booking.BookingType,
                        BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(r.Booking.BookingType),
                        Status = r.Booking.Status,
                        StatusText = _enumService.GetEnumDisplayName<BookingStatus>(r.Booking.Status),
                        BookingDate = r.Booking.BookingDate,
                        StartDate = r.Booking.StartDate,
                        EndDate = r.Booking.EndDate,
                        OriginalPrice = r.Booking.OriginalPrice,
                        DiscountAmount = r.Booking.DiscountAmount,
                        FinalPrice = r.Booking.FinalPrice,
                        ContactName = r.Booking.ContactName,
                        ContactEmail = r.Booking.ContactEmail,
                        ContactPhone = r.Booking.ContactPhone,
                        ContactAddress = r.Booking.ContactAddress,
                    }
                })
                .ToListAsync();

            list.ForEach(r =>
            {
                if (r.BookingId != null)
                {
                    var query = _unitOfWork.BookingRepository
                    .ActiveEntities
                        .Where(b => b.Id == r.BookingId)
                        .Select(b => new
                        {
                            UserName = b.User != null ? b.User.FullName : string.Empty,
                            TourName = b.Tour != null ? b.Tour.Name : string.Empty,
                            DepartureDate = b.TourSchedule != null
                        ? (DateTime?)b.TourSchedule.DepartureDate
                        : null,
                            TourGuideName = b.TourGuide != null && b.TourGuide.User != null ? b.TourGuide.User.FullName : string.Empty,
                            TripPlanName = b.TripPlan != null ? b.TripPlan.Name : string.Empty,
                            WorkshopName = b.Workshop != null ? b.Workshop.Name : string.Empty
                        })
                        .AsQueryable();

                    r.BookingDataModel.UserName = query.Select(b => b.UserName).FirstOrDefault() ?? string.Empty;
                    r.BookingDataModel.TourName = query.Select(b => b.TourName).FirstOrDefault() ?? string.Empty;
                    r.BookingDataModel.DepartureDate = query.Select(b => b.DepartureDate).FirstOrDefault();
                    r.BookingDataModel.TourGuideName = query.Select(b => b.TourGuideName).FirstOrDefault() ?? string.Empty;
                    r.BookingDataModel.TripPlanName = query.Select(b => b.TripPlanName).FirstOrDefault() ?? string.Empty;
                    r.BookingDataModel.WorkshopName = query.Select(b => b.WorkshopName).FirstOrDefault() ?? string.Empty;
                }
            });

            return list;
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

    public async Task<List<RefundRequestDto>> GetRefundRequestsForUserAsync(RefundRequestUserFilter filter)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var query = _unitOfWork.RefundRequestRepository.ActiveEntities
                .Where(r => r.UserId == currentUserId)
                .AsQueryable();

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.CreatedTime >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(r => r.CreatedTime <= filter.ToDate.Value);

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status.Value);

            var list = await query
                .OrderByDescending(r => r.CreatedTime)
                .Select(r => new RefundRequestDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    UserId = r.UserId,
                    Reason = r.Reason,
                    UserName = r.User.FullName,
                    RefundAmount = r.RefundAmount,
                    Status = r.Status,
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(r.Status),
                    Note = r.Note,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime,
                    RequestedAt = r.RequestedAt,
                    RespondedAt = r.RespondedAt,
                })
                .ToListAsync();

            return list;
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

    public async Task<RefundRequestDto> GetRefundRequestDetailAsync(Guid refundRequestId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var isAdminOrMod = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);

            var query = _unitOfWork.RefundRequestRepository.ActiveEntities
                .Include(r => r.User)
                .Include(r => r.Booking)
                .AsQueryable();

            if (!isAdminOrMod)
            {
                query = query.Where(r => r.UserId == currentUserId);
            }

            var request = await query.FirstOrDefaultAsync(r => r.Id == refundRequestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Refund Request");

            var result = new RefundRequestDto
            {
                Id = request.Id,
                BookingId = request.BookingId,
                UserId = request.UserId,
                Reason = request.Reason,
                UserName = request.User.FullName,
                RefundAmount = request.RefundAmount,
                Status = request.Status,
                StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(request.Status),
                Note = request.Note,
                CreatedTime = request.CreatedTime,
                LastUpdatedTime = request.LastUpdatedTime,
                RequestedAt = request.RequestedAt,
                RespondedAt = request.RespondedAt,
                BookingDataModel = new BusinessModels.BookingModels.BookingDataModel
                {
                    Id = request.Booking.Id,
                    UserId = request.Booking.UserId,
                    TourId = request.Booking.TourId,
                    TripPlanId = request.Booking.TripPlanId,
                    WorkshopId = request.Booking.WorkshopId,
                    WorkshopScheduleId = request.Booking.WorkshopScheduleId,
                    TourGuideId = request.Booking.TourGuideId,
                    BookingType = request.Booking.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(request.Booking.BookingType),
                    Status = request.Booking.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(request.Booking.Status),
                    BookingDate = request.Booking.BookingDate,
                    StartDate = request.Booking.StartDate,
                    EndDate = request.Booking.EndDate,
                    OriginalPrice = request.Booking.OriginalPrice,
                    DiscountAmount = request.Booking.DiscountAmount,
                    FinalPrice = request.Booking.FinalPrice,
                    ContactName = request.Booking.ContactName,
                    ContactEmail = request.Booking.ContactEmail,
                    ContactPhone = request.Booking.ContactPhone,
                    ContactAddress = request.Booking.ContactAddress,
                }
            };

            if (result.BookingId != null)
            {
                var bookingQuery = _unitOfWork.BookingRepository
                .ActiveEntities
                    .Where(b => b.Id == result.BookingId)
                    .Select(b => new
                    {
                        UserName = b.User != null ? b.User.FullName : string.Empty,
                        TourName = b.Tour != null ? b.Tour.Name : string.Empty,
                        DepartureDate = b.TourSchedule != null
                    ? (DateTime?)b.TourSchedule.DepartureDate
                    : null,
                        TourGuideName = b.TourGuide != null && b.TourGuide.User != null ? b.TourGuide.User.FullName : string.Empty,
                        TripPlanName = b.TripPlan != null ? b.TripPlan.Name : string.Empty,
                        WorkshopName = b.Workshop != null ? b.Workshop.Name : string.Empty
                    })
                    .AsQueryable();

                result.BookingDataModel.UserName = bookingQuery.Select(b => b.UserName).FirstOrDefault() ?? string.Empty;
                result.BookingDataModel.TourName = bookingQuery.Select(b => b.TourName).FirstOrDefault() ?? string.Empty;
                result.BookingDataModel.DepartureDate = bookingQuery.Select(b => b.DepartureDate).FirstOrDefault();
                result.BookingDataModel.TourGuideName = bookingQuery.Select(b => b.TourGuideName).FirstOrDefault() ?? string.Empty;
                result.BookingDataModel.TripPlanName = bookingQuery.Select(b => b.TripPlanName).FirstOrDefault() ?? string.Empty;
                result.BookingDataModel.WorkshopName = bookingQuery.Select(b => b.WorkshopName).FirstOrDefault() ?? string.Empty;
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

    public async Task DeleteRefundRequestAsync(Guid refundRequestId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var refundRequest = await _unitOfWork.RefundRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == refundRequestId && r.UserId == currentUserId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Refund Request");

            if (refundRequest.Status != RefundRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Chỉ có thể xóa yêu cầu ở trạng thái Pending.");

            refundRequest.IsDeleted = true;
            _unitOfWork.RefundRequestRepository.Update(refundRequest);
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
    }
}