using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
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

    public RefundRequestService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    // public async Task CreateRefundRequestAsync(RefundRequestCreateDto dto)
    // {
    //     var userId = Guid.Parse(_userContextService.GetCurrentUserId());

    //     var booking = await _unitOfWork.BookingRepository
    //         .ActiveEntities
    //         .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.UserId == userId)
    //         ?? throw CustomExceptionFactory.CreateNotFoundError("Booking");

    //     // Rule: Không cho tạo request nếu đã có pending hoặc approved
    //     var existingRequest = await _unitOfWork.RefundRequestRepository
    //         .ActiveEntities
    //         .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId &&
    //             (r.Status == RefundRequestStatus.Pending || r.Status == RefundRequestStatus.Approved));

    //     if (existingRequest != null)
    //         throw CustomExceptionFactory.CreateBadRequestError("Đã tồn tại yêu cầu hoàn tiền cho booking này.");

    //     if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.CancelledByProvider)
    //         throw CustomExceptionFactory.CreateBadRequestError("Trạng thái booking không cho phép hoàn tiền.");

    //     if (dto.RefundAmount > booking.FinalPrice)
    //         throw CustomExceptionFactory.CreateBadRequestError("Số tiền hoàn vượt quá số tiền đã thanh toán.");

    //     var refundRequest = new RefundRequest
    //     {
    //         Id = Guid.NewGuid(),
    //         UserId = userId,
    //         BookingId = dto.BookingId,
    //         RefundAmount = dto.RefundAmount,
    //         Status = RefundRequestStatus.Pending
    //     };

    //     await _unitOfWork.RefundRequestRepository.AddAsync(refundRequest);
    //     await _unitOfWork.SaveAsync();
    // }

}