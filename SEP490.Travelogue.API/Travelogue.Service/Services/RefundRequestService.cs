using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.RefundRequestModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IRefundRequestService
{
}

public class RefundRequestService : IRefundRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public RefundRequestService(IUnitOfWork unitOfWork, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task CreateRefundRequestAsync(RefundRequestCreateDto dto)
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

            // Rule: Không cho tạo request nếu đã có pending hoặc approved
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