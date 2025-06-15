using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IBookingService
{
    // Task<BookingDataModel?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookingDataModel> AddBookingAsync(BookingCreateModel tripPlanCreateModel, CancellationToken cancellationToken);
    Task<TourGuideBookingRequestDataModel?> CreateRequestAsync(TourGuideBookingRequestCreateModel requestCreateModel, CancellationToken cancellationToken);
    Task<TourGuideBookingRequestDataModel?> RejectByGuideAsync(Guid id, CancellationToken cancellationToken);
    Task<object> SuggestNewVersionAsync(Guid bookingRequestId, Guid versionId, string? guideNote);
    Task<object?> CreateBookingFromRequestAsync(Guid bookingRequestId, CancellationToken cancellationToken);
}

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task<BookingDataModel> AddBookingAsync(BookingCreateModel bookingCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == bookingCreateModel.TourGuideId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("tour guide");
            var tour = await _unitOfWork.TourRepository.ActiveEntities
                .FirstOrDefaultAsync(t => t.Id == bookingCreateModel.TourId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("tour");
            var version = await _unitOfWork.TripPlanVersionRepository.ActiveEntities
                .FirstOrDefaultAsync(v => v.Id == bookingCreateModel.VersionId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("tour version");

            var newBooking = _mapper.Map<Order>(bookingCreateModel);

            newBooking.CreatedBy = currentUserId;
            newBooking.LastUpdatedBy = currentUserId;
            newBooking.CreatedTime = currentTime;
            newBooking.LastUpdatedTime = currentTime;

            await _unitOfWork.OrderRepository.AddAsync(newBooking);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.OrderRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == newBooking.Id);

            return _mapper.Map<BookingDataModel>(result);
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<TourGuideBookingRequestDataModel?> CreateRequestAsync(TourGuideBookingRequestCreateModel requestCreateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == requestCreateModel.TourGuideId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("tour guide");

            var tripPlan = await _unitOfWork.TripPlanRepository.ActiveEntities
                .FirstOrDefaultAsync(tp => tp.Id == requestCreateModel.TripPlanId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("trip plan");

            var tripPlanVersion = await _unitOfWork.TripPlanVersionRepository.ActiveEntities
                .FirstOrDefaultAsync(v => v.Id == requestCreateModel.TripPlanVersionId, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("trip plan version");
            if (tripPlanVersion.TripPlanId != tripPlan.Id)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Trip plan version does not belong to the specified trip plan.");
            }
            var tourGuideBookingRequest = _mapper.Map<TripPlanExchange>(requestCreateModel);

            tourGuideBookingRequest.UserId = Guid.Parse(currentUserId);
            tourGuideBookingRequest.Status = ExchangeSessionStatus.Pending;
            tourGuideBookingRequest.RequestedAt = currentTime;

            tourGuideBookingRequest.CreatedBy = currentUserId;
            tourGuideBookingRequest.LastUpdatedBy = currentUserId;
            tourGuideBookingRequest.CreatedTime = currentTime;
            tourGuideBookingRequest.LastUpdatedTime = currentTime;

            await _unitOfWork.TourGuideBookingRequestRepository.AddAsync(tourGuideBookingRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.TourGuideBookingRequestRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == tourGuideBookingRequest.Id);

            return _mapper.Map<TourGuideBookingRequestDataModel>(result);
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<TourGuideBookingRequestDataModel?> RejectByGuideAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var tourGuideBookingRequest = await _unitOfWork.TourGuideBookingRequestRepository.ActiveEntities
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken) ?? throw CustomExceptionFactory.CreateNotFoundError("tour guide booking request");

            tourGuideBookingRequest.Status = ExchangeSessionStatus.Cancelled;

            tourGuideBookingRequest.CreatedBy = currentUserId;
            tourGuideBookingRequest.LastUpdatedBy = currentUserId;
            tourGuideBookingRequest.CreatedTime = currentTime;
            tourGuideBookingRequest.LastUpdatedTime = currentTime;

            await _unitOfWork.TourGuideBookingRequestRepository.AddAsync(tourGuideBookingRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.TourGuideBookingRequestRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == tourGuideBookingRequest.Id);

            return _mapper.Map<TourGuideBookingRequestDataModel>(result);
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    /// <summary>
    /// Tour guide gửi lại version mới cho người dùng đã gửi booking request
    /// </summary>
    /// <param name="bookingRequestId"></param>
    /// <param name="version"></param>
    /// <param name="guideNote"></param>
    /// <returns></returns>
    public async Task<object> SuggestNewVersionAsync(Guid bookingRequestId, Guid versionId, string? guideNote)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var bookingRequest = await _unitOfWork.TourGuideBookingRequestRepository
                .GetWithIncludeAsync(bookingRequestId, include => include.Include(br => br.TripPlan))
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking request");

            var version = await _unitOfWork.TripPlanVersionRepository
                .GetWithIncludeAsync(versionId, include => include.Include(v => v.TripPlan))
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");

            bookingRequest.SuggestedTripPlanVersionId = version.Id;

            await _unitOfWork.TourGuideBookingRequestRepository.AddAsync(bookingRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new
            {
                BookingRequestId = bookingRequest.Id,
                SuggestedTripPlanVersionId = version.Id,
                VersionNumber = version.VersionNumber,
                Notes = guideNote
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<object> RequestNewVersion(Guid bookingRequestId, Guid versionId, string? guideNote)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var bookingRequest = await _unitOfWork.TourGuideBookingRequestRepository
                .GetWithIncludeAsync(bookingRequestId, include => include.Include(br => br.TripPlan))
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking request");

            var version = await _unitOfWork.TripPlanVersionRepository
                .GetWithIncludeAsync(versionId, include => include.Include(v => v.TripPlan))
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");

            bookingRequest.SuggestedTripPlanVersionId = version.Id;

            await _unitOfWork.TourGuideBookingRequestRepository.AddAsync(bookingRequest);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new
            {
                BookingRequestId = bookingRequest.Id,
                SuggestedTripPlanVersionId = version.Id,
                VersionNumber = version.VersionNumber,
                Notes = guideNote
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    /// <summary>
    /// Tạo một đơn đặt tour (booking) từ yêu cầu booking đã được người dùng chấp nhận.
    ///
    /// <para><b>Luồng xử lý:</b></para>
    /// <list type="number">
    /// <item>1. Lấy thông tin yêu cầu booking theo ID (kèm TripPlan liên quan).</item>
    /// <item>2. Đánh dấu yêu cầu booking đã được người dùng chấp nhận.</item>
    /// <item>3. Tạo booking mới dựa trên thông tin từ booking request.</item>
    /// <item>4. Thiết lập các thông tin bổ sung như: VersionId, thời gian đặt, người tạo, thời gian dự kiến,...</item>
    /// <item>5. Thêm booking vào database.</item>
    /// <item>6. Lưu lại trạng thái mới của booking request và booking vừa tạo.</item>
    /// <item>7. Trả về thông tin của booking dưới dạng object ẩn danh.</item>
    /// </list>
    ///
    /// <para>Giao dịch (transaction) được sử dụng để đảm bảo tính toàn vẹn nếu có lỗi xảy ra trong quá trình.</para>
    /// </summary>
    /// <param name="bookingRequestId">ID của yêu cầu booking</param>
    /// <param name="cancellationToken">Token hủy thao tác</param>
    /// <returns>Thông tin booking vừa được tạo</returns>
    public async Task<object?> CreateBookingFromRequestAsync(Guid bookingRequestId, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var bookingRequest = await _unitOfWork.TourGuideBookingRequestRepository
                .GetWithIncludeAsync(bookingRequestId, include => include.Include(br => br.TripPlan))
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking request");

            // Kiểm tra có phải là người dùng đã gửi yêu cầu booking này không
            if (bookingRequest.UserId != Guid.Parse(currentUserId))
            {
                throw CustomExceptionFactory.CreateBadRequestError("You are not the user who sent this booking request.");
            }

            bookingRequest.Status = ExchangeSessionStatus.AcceptedByUser;

            var newBooking = _mapper.Map<Order>(bookingRequest);
            newBooking.VersionId = bookingRequest.SuggestedTripPlanVersionId ?? bookingRequest.TripPlanVersionId;
            newBooking.OrderDate = currentTime;
            newBooking.CreatedBy = currentUserId;
            newBooking.LastUpdatedBy = currentUserId;
            newBooking.ScheduledStartDate = bookingRequest.StartDate;
            newBooking.ScheduledEndDate = bookingRequest.EndDate;

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new
            {
                BookingId = newBooking.Id,
                UserId = newBooking.UserId,
                TourId = newBooking.TourId,
                TourGuideId = newBooking.TourGuideId,
                VersionId = newBooking.VersionId,
                OrderDate = newBooking.OrderDate,
                ScheduledStartDate = newBooking.ScheduledStartDate,
                ScheduledEndDate = newBooking.ScheduledEndDate,
                Status = newBooking.Status,
                TotalPaid = newBooking.TotalPaid
            };
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }
}
