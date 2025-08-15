using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.RefundRequestModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IRefundRequestService
{
    Task<RefundRequestDto> CreateRefundRequestAsync(RefundRequestCreateDto dto);
    Task<RefundRequestDto> ApproveRefundRequestAsync(Guid refundRequestId);
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

            if (!(booking.Status == BookingStatus.Confirmed
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
                RefundAmount = dto.RefundAmount,
                Status = RefundRequestStatus.Pending
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
                UserId = refundRequest.UserId,
                UserName = username ?? string.Empty,
                RefundAmount = refundRequest.RefundAmount,
                Status = refundRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                RejectionReason = refundRequest.RejectionReason,
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

    public async Task<RefundRequestDto> ApproveRefundRequestAsync(Guid refundRequestId)
    {
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
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


                refundRequest.Status = RefundRequestStatus.Approved;

                refundRequest.User.Wallet.Balance += refundRequest.RefundAmount;

                var walletTransaction = new TransactionEntry
                {
                    Id = Guid.NewGuid(),
                    WalletId = refundRequest.User.Wallet.Id,
                    PaidAmount = refundRequest.RefundAmount,
                    Type = TransactionType.Refund,
                    Description = $"Hoàn tiền cho booking {refundRequest.BookingId}"
                };

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
                    RefundAmount = refundRequest.RefundAmount,
                    Status = refundRequest.Status,
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                    RejectionReason = refundRequest.RejectionReason,
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
    }

    public async Task<RefundRequestDto> RejectRefundRequestAsync(Guid refundRequestId, string rejectionReason)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var refundRequest = await _unitOfWork.RefundRequestRepository
                .ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == refundRequestId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Refund Request");

            if (refundRequest.Status != RefundRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Yêu cầu không ở trạng thái Pending.");

            refundRequest.Status = RefundRequestStatus.Rejected;
            refundRequest.RejectionReason = rejectionReason;

            _unitOfWork.RefundRequestRepository.Update(refundRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new RefundRequestDto
            {
                Id = refundRequest.Id,
                BookingId = refundRequest.BookingId,
                UserId = refundRequest.UserId,
                UserName = refundRequest.User.FullName,
                RefundAmount = refundRequest.RefundAmount,
                Status = refundRequest.Status,
                StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(refundRequest.Status),
                RejectionReason = refundRequest.RejectionReason,
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
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(r.Status),
                    RejectionReason = r.RejectionReason,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime,
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
                    UserName = r.User.FullName,
                    RefundAmount = r.RefundAmount,
                    Status = r.Status,
                    StatusText = _enumService.GetEnumDisplayName<RefundRequestStatus>(r.Status),
                    RejectionReason = r.RejectionReason,
                    CreatedTime = r.CreatedTime,
                    LastUpdatedTime = r.LastUpdatedTime,
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

            return new RefundRequestDto
            {
                Id = request.Id,
                BookingId = request.BookingId,
                UserId = request.UserId,
                UserName = request.User.FullName,
                RefundAmount = request.RefundAmount,
                Status = request.Status,
                RejectionReason = request.RejectionReason,
                CreatedTime = request.CreatedTime,
                LastUpdatedTime = request.LastUpdatedTime,
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