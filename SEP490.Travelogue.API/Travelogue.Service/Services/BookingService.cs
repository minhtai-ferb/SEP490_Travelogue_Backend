using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IBookingService
{
    // Task<BookingDataModel?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookingDataModel> CreateTourBookingAsync(CreateBookingTourDto dto, CancellationToken cancellationToken);
    Task<BookingDataModel> CreateTourGuideBookingAsync(CreateBookingTourGuideDto dto, CancellationToken cancellationToken);
    Task<BookingDataModel> CreateWorkshopBookingAsync(CreateBookingWorkshopDto dto, CancellationToken cancellationToken);
    Task<List<Booking>> GetUserBookingsAsync(
        BookingType? bookingType = null,
        DateTimeOffset? bookingDate = null,
        BookingStatus? bookingStatus = null,
        CancellationToken cancellationToken = default);
    Task<List<BookingDataModel>> GetUserBookingsAsync(BookingFilterDto filter);
    Task<List<BookingDataModel>> GetAllBookingsAsync(BookingFilterDto filter);
    Task<PagedResult<BookingDataModel>> GetPagedBookingsAsync(BookingFilterDto filter, int pageNumber = 1, int pageSize = 10);
    Task<BookingDataModel> GetBookingByIdAsync(Guid bookingId);
    Task<CreatePaymentResult> CreatePaymentLink(Guid bookingId, CancellationToken cancellationToken = default);
    Task<bool> ProcessPaymentResponseAsync(PaymentResponse paymentResponse);
    Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId);
    Task<bool> CancelBookingAsync(Guid bookingId);
}

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly IEnumService _enumService;
    private readonly PayOS _payOS;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService, IEnumService enumService, PayOS payOS)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
        _enumService = enumService;
        _payOS = payOS;
    }

    public async Task<BookingDataModel> CreateTourBookingAsync(CreateBookingTourDto dto, CancellationToken cancellationToken)
    {
        if (dto.AdultCount <= 0)
        {
            throw CustomExceptionFactory.CreateBadRequestError(
                dto.ChildrenCount > 0
                    ? "Phải có ít nhất một người lớn đi kèm nếu có trẻ em."
                    : "Cần có ít nhất một người lớn tham gia."
            );
        }
        var currentUserId = _userContextService.GetCurrentUserId();
        var currentTime = _timeService.SystemTimeNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var tourSchedule = await _unitOfWork.TourScheduleRepository
                .ActiveEntities
                .Include(ts => ts.Tour)
                .FirstOrDefaultAsync(ts => ts.Id == dto.ScheduledId && ts.TourId == dto.TourId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("tour hoặc tour schedule");

            // còn chỗ kh
            int totalParticipants = dto.AdultCount + dto.ChildrenCount;
            if (tourSchedule.CurrentBooked + totalParticipants > tourSchedule.MaxParticipant)
                throw CustomExceptionFactory.CreateBadRequestError("Tour không còn đủ số lượng chỗ bạn yêu cầu");

            decimal adultPrice = tourSchedule.AdultPrice;
            decimal childrenPrice = tourSchedule.ChildrenPrice;
            decimal originalPrice = (adultPrice * dto.AdultCount) + (childrenPrice * dto.ChildrenCount);

            // mã giảm giá
            var (discountAmount, promotion) = await ValidateAndCalculateDiscountAsync(dto.PromotionCode, originalPrice, dto.TourId);

            decimal finalPrice = Math.Max(0, originalPrice - discountAmount);

            var booking = new Booking
            {
                UserId = Guid.Parse(currentUserId),
                TourId = dto.TourId,
                TourScheduleId = dto.ScheduledId,
                BookingType = BookingType.Tour,
                Status = BookingStatus.Pending,
                BookingDate = DateTimeOffset.UtcNow,
                StartDate = tourSchedule.DepartureDate,
                EndDate = tourSchedule.DepartureDate.AddDays(tourSchedule.Tour.TotalDays - 1),
                PromotionId = promotion?.Id,
                OriginalPrice = originalPrice,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice
            };

            // Add participants
            if (dto.AdultCount > 0)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Type = ParticipantType.Adult,
                    Quantity = dto.AdultCount,
                    PricePerParticipant = adultPrice
                });
            }

            if (dto.ChildrenCount > 0)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Type = ParticipantType.Child,
                    Quantity = dto.ChildrenCount,
                    PricePerParticipant = childrenPrice
                });
            }

            await _unitOfWork.BookingRepository.AddAsync(booking);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            var response = new BookingDataModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TourId = booking.TourId,
                TourScheduleId = booking.TourScheduleId,
                TourGuideId = booking.TourGuideId,
                WorkshopId = booking.WorkshopId,
                PaymentLinkId = booking.PaymentLinkId,
                Status = booking.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(booking.Status),
                BookingType = booking.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(booking.BookingType),
                BookingDate = booking.BookingDate,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                CancelledAt = booking.CancelledAt,
                PromotionId = booking.PromotionId,
                OriginalPrice = booking.OriginalPrice,
                DiscountAmount = booking.DiscountAmount,
                FinalPrice = booking.FinalPrice
            };

            return response;
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

    public async Task<BookingDataModel> CreateWorkshopBookingAsync(CreateBookingWorkshopDto dto, CancellationToken cancellationToken)
    {
        if (dto.AdultCount <= 0)
        {
            throw CustomExceptionFactory.CreateBadRequestError(
                dto.ChildrenCount > 0
                    ? "Phải có ít nhất một người lớn đi kèm nếu có trẻ em."
                    : "Cần có ít nhất một người lớn tham gia."
            );
        }

        var currentUserId = _userContextService.GetCurrentUserId();
        var currentTime = _timeService.SystemTimeNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var workshopSchedule = await _unitOfWork.WorkshopScheduleRepository
                .ActiveEntities
                .Include(ws => ws.Workshop)
                .FirstOrDefaultAsync(ws => ws.Id == dto.WorkshopScheduleId && ws.WorkshopId == dto.WorkshopId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("workshop hoặc workshop schedule");

            // Check available slots
            int totalParticipants = dto.AdultCount + dto.ChildrenCount;
            if (workshopSchedule.CurrentBooked + totalParticipants > workshopSchedule.MaxParticipant)
                throw CustomExceptionFactory.CreateBadRequestError("Workshop không còn đủ chỗ cho số lượng bạn yêu cầu.");

            decimal adultPrice = workshopSchedule.AdultPrice;
            decimal childrenPrice = workshopSchedule.ChildrenPrice;
            decimal originalPrice = (adultPrice * dto.AdultCount) + (childrenPrice * dto.ChildrenCount);

            var (discountAmount, promotion) = await ValidateAndCalculateDiscountAsync(dto.PromotionCode, originalPrice, dto.WorkshopId);

            decimal finalPrice = Math.Max(0, originalPrice - discountAmount);

            var booking = new Booking
            {
                UserId = Guid.Parse(currentUserId),
                WorkshopId = dto.WorkshopId,
                WorkshopScheduleId = dto.WorkshopScheduleId,
                BookingType = BookingType.Workshop,
                Status = BookingStatus.Pending,
                BookingDate = DateTimeOffset.UtcNow,
                StartDate = workshopSchedule.StartTime,
                EndDate = workshopSchedule.EndTime,
                PromotionId = promotion?.Id,
                OriginalPrice = originalPrice,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice
            };

            // Add participants
            if (dto.AdultCount > 0)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Type = ParticipantType.Adult,
                    Quantity = dto.AdultCount,
                    PricePerParticipant = adultPrice
                });
            }

            if (dto.ChildrenCount > 0)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Type = ParticipantType.Child,
                    Quantity = dto.ChildrenCount,
                    PricePerParticipant = childrenPrice
                });
            }

            // workshopSchedule.CurrentBooked += totalParticipants;

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            var response = new BookingDataModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TourId = booking.TourId,
                TourScheduleId = booking.TourScheduleId,
                TourGuideId = booking.TourGuideId,
                WorkshopId = booking.WorkshopId,
                WorkshopScheduleId = booking.WorkshopScheduleId,
                PaymentLinkId = booking.PaymentLinkId,
                Status = booking.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(booking.Status),
                BookingType = booking.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(booking.BookingType),
                BookingDate = booking.BookingDate,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                CancelledAt = booking.CancelledAt,
                PromotionId = booking.PromotionId,
                OriginalPrice = booking.OriginalPrice,
                DiscountAmount = booking.DiscountAmount,
                FinalPrice = booking.FinalPrice
            };

            return response;
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

    public async Task<BookingDataModel> CreateTourGuideBookingAsync(CreateBookingTourGuideDto dto, CancellationToken cancellationToken)
    {
        if (dto.AdultCount <= 0)
        {
            throw CustomExceptionFactory.CreateBadRequestError(
                dto.ChildrenCount > 0
                    ? "Phải có ít nhất một người lớn đi kèm nếu có trẻ em."
                    : "Cần có ít nhất một người lớn tham gia."
            );
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.");
        }

        var currentUserId = _userContextService.GetCurrentUserId();
        var currentTime = _timeService.SystemTimeNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            TripPlan? tripPlan = null;
            TourGuide? tourGuide = null;

            if (dto.TripPlanId.HasValue)
            {
                tripPlan = await _unitOfWork.TripPlanRepository
                    .ActiveEntities
                    .FirstOrDefaultAsync(t => t.Id == dto.TripPlanId.Value)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("trip plan");

                if (tripPlan.Status != TripPlanStatus.Sketch)
                    throw CustomExceptionFactory.CreateBadRequestError("Trip plan đang không ở trạng thái phù hợp");
            }

            tourGuide = await _unitOfWork.TourGuideRepository
                .ActiveEntities
                .Include(g => g.TourGuideSchedules)
                .FirstOrDefaultAsync(g => g.Id == dto.TourGuideId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("hướng dẫn viên");

            var numberOfDays = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;

            // tour guide có rảnh vào ngày đó kh
            var dateRange = Enumerable.Range(0, (dto.EndDate.Date - dto.StartDate.Date).Days + 1)
                .Select(d => dto.StartDate.Date.AddDays(d))
                .ToList();

            var isBusy = tourGuide.TourGuideSchedules != null && tourGuide.TourGuideSchedules.Any(s =>
                dateRange.Contains(s.Date.Date) && s.BookingId != null);

            if (isBusy)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Hướng dẫn viên đã có lịch vào một hoặc nhiều ngày trong khoảng thời gian được chọn.");
            }

            // Tính giá
            decimal tourGuideDailyPrice = tourGuide?.Price ?? 0;
            decimal originalPrice = tourGuideDailyPrice * numberOfDays;
            decimal finalPrice = originalPrice;

            // var (discountAmount, promotion) = await ValidateAndCalculateDiscountAsync(dto.PromotionCode, originalPrice, tourGuide?.Id);

            // decimal finalPrice = Math.Max(0, originalPrice - discountAmount);

            var booking = new Booking
            {
                UserId = Guid.Parse(currentUserId),
                TourGuideId = dto.TourGuideId,
                TripPlanId = dto.TripPlanId,
                BookingType = BookingType.TourGuide,
                Status = BookingStatus.Pending,
                BookingDate = currentTime,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                // PromotionId = promotion?.Id,
                OriginalPrice = originalPrice,
                DiscountAmount = 0,
                FinalPrice = originalPrice
            };

            // Thêm participant
            booking.Participants.Add(new BookingParticipant
            {
                Type = ParticipantType.Adult,
                Quantity = dto.AdultCount,
                PricePerParticipant = 0
            });

            if (dto.ChildrenCount > 0)
            {
                booking.Participants.Add(new BookingParticipant
                {
                    Type = ParticipantType.Child,
                    Quantity = dto.ChildrenCount,
                    PricePerParticipant = 0
                });
            }

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return new BookingDataModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TripPlanId = booking.TourId,
                TourGuideId = booking.TourGuideId,
                Status = booking.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(booking.Status),
                BookingType = booking.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(booking.BookingType),
                BookingDate = booking.BookingDate,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                CancelledAt = booking.CancelledAt,
                PromotionId = booking.PromotionId,
                OriginalPrice = booking.OriginalPrice,
                DiscountAmount = booking.DiscountAmount,
                FinalPrice = booking.FinalPrice
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

    public async Task<CreatePaymentResult> CreatePaymentLink(Guid bookingId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            if (!Guid.TryParse(currentUserId, out Guid currentUserIdGuid))
                throw CustomExceptionFactory.CreateBadRequestError("Invalid user ID format.");

            var booking = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Include(b => b.Tour)
                .Include(b => b.TourSchedule)
                .Include(b => b.TourGuide)
                .ThenInclude(tg => tg!.User)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == currentUserIdGuid, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Booking not found or user not authorized.");

            if (booking.Status != BookingStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Booking is not in a pending state.");

            if (booking.StartDate < currentTime)
                throw CustomExceptionFactory.CreateBadRequestError("Cannot create payment link for past bookings.");

            decimal totalAmount = booking.FinalPrice;
            if (totalAmount <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Total amount must be greater than zero.");

            // Calculate participant counts
            int adultCount = booking.Participants
                .Where(p => p.Type == ParticipantType.Adult)
                .Sum(p => p.Quantity);
            int childCount = booking.Participants
                .Where(p => p.Type == ParticipantType.Child)
                .Sum(p => p.Quantity);

            // Generate order code
            long orderCode = long.Parse(currentTime.ToString("yyyyMMddHHmmss"));

            // Prepare payment description
            string tourDescription = booking.TourScheduleId.HasValue
                ? $"Tour: {booking.Tour?.Name ?? "Custom Tour"} (Run: {booking.TourSchedule?.DepartureDate:yyyy-MM-dd}, Adults: {adultCount}, Children: {childCount})"
                : $"Custom Tour with {booking.TourGuide?.User.FullName ?? "Unknown Guide"} (Adults: {adultCount}, Children: {childCount})";

            var items = new List<ItemData>
            {
                new ItemData(
                    name: tourDescription,
                    quantity: 1,
                    price: (int)totalAmount
                )
            };

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)totalAmount,
                description: $"Payment for Travelogue",
                items: items,
                cancelUrl: "http://yourapp.com/cancel",
                returnUrl: "http://yourapp.com/success",
                expiredAt: (int)currentTime.AddMinutes(5).ToUnixTimeSeconds()
            );

            var paymentResult = await _payOS.createPaymentLink(paymentData);

            booking.PaymentLinkId = paymentResult.paymentLinkId;

            _unitOfWork.BookingRepository.Update(booking);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return paymentResult;
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

    public async Task<List<Booking>> GetUserBookingsAsync(
        BookingType? bookingType = null,
        DateTimeOffset? bookingDate = null,
        BookingStatus? bookingStatus = null,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = _userContextService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserId, out Guid currentUserIdGuid))
            throw CustomExceptionFactory.CreateBadRequestError("User id không hợp lệ");

        var query = _unitOfWork.BookingRepository
            .ActiveEntities
            .Include(b => b.Tour)
            .Include(b => b.TourSchedule)
            .Include(b => b.TourGuide)
            .Include(b => b.Workshop)
            .Include(b => b.WorkshopSchedule)
            .Include(b => b.Promotion)
            .Include(b => b.Participants)
            .Where(b => b.UserId == currentUserIdGuid);

        // Apply filters
        if (bookingType.HasValue)
            query = query.Where(b => b.BookingType == bookingType.Value);

        if (bookingDate.HasValue)
            query = query.Where(b => b.BookingDate.Date == bookingDate.Value.Date);

        if (bookingStatus.HasValue)
            query = query.Where(b => b.Status == bookingStatus.Value);

        // Sort by BookingDate descending
        query = query.OrderByDescending(b => b.BookingDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ProcessPaymentResponseAsync(PaymentResponse paymentResponse)
    {
        try
        {
            var currentUserId = "1";
            var currentTime = _timeService.SystemTimeNow;

            // In các giá trị của PaymentResponse để kiểm tra
            if (paymentResponse != null)
            {
                Console.WriteLine("Parsed paymentResponse successfully:");
                Console.WriteLine($"Code: {paymentResponse.Code}");
                Console.WriteLine($"Desc: {paymentResponse.Desc}");
                Console.WriteLine($"Success: {paymentResponse.Success}");
                Console.WriteLine($"Signature: {paymentResponse.Signature}");

                if (paymentResponse.Data != null)
                {
                    Console.WriteLine("Data:");
                    Console.WriteLine($"  AccountNumber: {paymentResponse.Data.AccountNumber}");
                    Console.WriteLine($"  Amount: {paymentResponse.Data.Amount}");
                    Console.WriteLine($"  Description: {paymentResponse.Data.Description}");
                    Console.WriteLine($"  Reference: {paymentResponse.Data.Reference}");
                    Console.WriteLine($"  TransactionDateTime: {paymentResponse.Data.TransactionDateTime}");
                    Console.WriteLine($"  CounterAccountBankId: {paymentResponse.Data.CounterAccountBankId}");
                    Console.WriteLine($"  CounterAccountBankName: {paymentResponse.Data.CounterAccountBankName}");
                    Console.WriteLine($"  CounterAccountName: {paymentResponse.Data.CounterAccountName}");
                    Console.WriteLine($"  CounterAccountNumber: {paymentResponse.Data.CounterAccountNumber}");
                    Console.WriteLine($"  Currency: {paymentResponse.Data.Currency}");
                    Console.WriteLine($"  OrderCode: {paymentResponse.Data.OrderCode}");
                    Console.WriteLine($"  PaymentLinkId: {paymentResponse.Data.PaymentLinkId}");
                }
                else
                {
                    Console.WriteLine("Data is null.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("PaymentResponse is null.");
                return false;
            }

            // Kiểm tra điều kiện trong paymentResponse
            if (paymentResponse.Code != "00" || !paymentResponse.Success)
            {
                Console.WriteLine("Validation failed for paymentResponse.");
                return false;
            }

            // Xử lý TransactionDateTime
            if (!DateTime.TryParse(paymentResponse.Data.TransactionDateTime, out DateTime transactionDateTime))
            {
                Console.WriteLine("TransactionDateTime is invalid.");
                return false;
            }

            var existingBooking = await _unitOfWork.BookingRepository
                .ActiveEntities
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.PaymentLinkId == paymentResponse.Data.PaymentLinkId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("booking");

            var totalParticipants = existingBooking.Participants.Sum(p => p.Quantity);
            if (totalParticipants <= 0)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Khong có người tham gia nào trong booking.");
            }

            switch (existingBooking.BookingType)
            {
                case BookingType.Tour:
                    if (existingBooking.TourScheduleId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("TourScheduleId không tồn tại trong booking tour.");

                    var tourSchedule = await _unitOfWork.TourScheduleRepository
                        .ActiveEntities
                        .FirstOrDefaultAsync(ts => ts.Id == existingBooking.TourScheduleId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("tour schedule");

                    tourSchedule.CurrentBooked += totalParticipants;
                    _unitOfWork.TourScheduleRepository.Update(tourSchedule);
                    break;

                case BookingType.Workshop:
                    if (existingBooking.WorkshopScheduleId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("WorkshopScheduleId không tồn tại trong booking workshop.");

                    var workshopSchedule = await _unitOfWork.WorkshopScheduleRepository
                        .ActiveEntities
                        .FirstOrDefaultAsync(ws => ws.Id == existingBooking.WorkshopScheduleId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("workshop schedule");

                    var workshop = await _unitOfWork.WorkshopRepository
                        .ActiveEntities
                        .Include(w => w.CraftVillage)
                        .FirstOrDefaultAsync(w => w.Id == existingBooking.WorkshopId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("workshop");

                    var ownerId = workshop.CraftVillage?.OwnerId;
                    if (ownerId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("Workshop không có chủ sở hữu hợp lệ.");

                    var owner = await _unitOfWork.UserRepository
                        .ActiveEntities
                        .Include(u => u.Wallet)
                        .FirstOrDefaultAsync(u => u.Id == ownerId.Value)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("user");

                    if (owner.Wallet == null)
                        throw CustomExceptionFactory.CreateBadRequestError("Owner chưa có ví.");

                    owner.Wallet.Balance += existingBooking.FinalPrice;
                    _unitOfWork.UserRepository.Update(owner);

                    workshopSchedule.CurrentBooked += totalParticipants;
                    _unitOfWork.WorkshopScheduleRepository.Update(workshopSchedule);

                    break;

                case BookingType.TourGuide:
                    if (existingBooking.TourGuideId == null)
                        throw CustomExceptionFactory.CreateBadRequestError("TourGuideId không tồn tại trong booking tour guide.");
                    var tourGuide = await _unitOfWork.TourGuideRepository
                        .ActiveEntities
                        .Include(tg => tg.TourGuideSchedules)
                        .FirstOrDefaultAsync(tg => tg.Id == existingBooking.TourGuideId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("tour guide");
                    var guideUser = await _unitOfWork.UserRepository
                        .ActiveEntities
                        .Include(u => u.Wallet)
                        .FirstOrDefaultAsync(u => u.Id == tourGuide.UserId)
                        ?? throw CustomExceptionFactory.CreateNotFoundError("user");

                    guideUser.Wallet.Balance += existingBooking.FinalPrice;
                    _unitOfWork.UserRepository.Update(guideUser);

                    var dateRange = existingBooking.StartDate.Date <= existingBooking.EndDate.Date
                        ? Enumerable.Range(0, (existingBooking.EndDate.Date - existingBooking.StartDate.Date).Days + 1)
                            .Select(d => existingBooking.StartDate.Date.AddDays(d))
                            .ToList()
                        : new List<DateTime>();

                    foreach (var date in dateRange)
                    {
                        var schedule = new TourGuideSchedule
                        {
                            TourGuideId = tourGuide.Id,
                            Date = date,
                            Note = "Đặt tour thành công",
                            BookingId = existingBooking.Id
                        };
                        await _unitOfWork.TourGuideScheduleRepository.AddAsync(schedule);
                    }

                    // var dateRange = Enumerable.Range(0, (existingBooking.EndDate.Date - existingBooking.StartDate.Date).Days + 1)
                    // .Select(d => existingBooking.StartDate.Date.AddDays(d))
                    // .ToList();

                    // foreach (var date in dateRange)
                    // {
                    //     var schedule = new TourGuideSchedule
                    //     {
                    //         TourGuideId = tourGuide.Id,
                    //         Date = date,
                    //         Note = "Đặt tour thành công",
                    //         BookingId = existingBooking.Id
                    //     };
                    //     await _unitOfWork.TourGuideScheduleRepository.AddAsync(schedule);
                    // }
                    break;

                default:
                    throw CustomExceptionFactory.CreateBadRequestError("Loại booking không được hỗ trợ.");
            }

            var order = new TransactionEntry
            {
                // BookingId = existingBooking.Id,
                AccountNumber = paymentResponse.Data.AccountNumber,
                PaidAmount = paymentResponse.Data.Amount,
                PaymentReference = paymentResponse.Data.Reference,
                TransactionDateTime = transactionDateTime,
                CounterAccountBankId = paymentResponse.Data.CounterAccountBankId,
                CounterAccountName = paymentResponse.Data.CounterAccountName,
                CounterAccountNumber = paymentResponse.Data.CounterAccountNumber,
                Currency = paymentResponse.Data.Currency,
                PaymentLinkId = paymentResponse.Data.PaymentLinkId,
                PaymentStatus = PaymentStatus.Success,
                CreatedBy = currentUserId,
                CreatedTime = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime
            };

            _unitOfWork.BeginTransaction();

            await _unitOfWork.TransactionEntryRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            var bookingsToUpdate = await _unitOfWork.BookingRepository.Entities
                .Where(b => b.PaymentLinkId == paymentResponse.Data.PaymentLinkId)
                .ToListAsync();

            if (bookingsToUpdate.Any())
            {
                foreach (var booking in bookingsToUpdate)
                {
                    booking.Status = BookingStatus.Confirmed;
                }

                _unitOfWork.BookingRepository.UpdateRange(bookingsToUpdate);
                await _unitOfWork.SaveAsync();
            }

            _unitOfWork.CommitTransaction();
            return true;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollBack();
            Console.WriteLine("Error occurred:");
            Console.WriteLine(ex.Message);
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            // _unitOfWork.Dispose();
        }
    }

    public async Task<List<BookingDataModel>> GetUserBookingsAsync(BookingFilterDto filter)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isUser = _userContextService.HasRole(AppRole.USER);
            if (!isUser)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            IQueryable<Booking> query = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.UserId == currentUserId)
                .Include(b => b.TourGuide)
                    .ThenInclude(tg => tg != null ? tg.User : null)
                .Include(b => b.Tour)
                .Include(b => b.TourSchedule)
                .Include(b => b.TripPlan)
                .Include(b => b.Workshop)
                .Include(b => b.WorkshopSchedule)
                .Include(b => b.Promotion);

            // status
            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            // bookng type
            if (filter.BookingType.HasValue)
            {
                query = query.Where(b => b.BookingType == filter.BookingType.Value);
            }

            // ngày tháng
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                if (filter.StartDate > filter.EndDate)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                }
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value && b.BookingDate <= filter.EndDate.Value);
            }
            else if (filter.StartDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value);
            }
            else if (filter.EndDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= filter.EndDate.Value);
            }

            var bookings = await query
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var result = bookings.Select(b => new BookingDataModel
            {
                Id = b.Id,
                UserId = b.UserId,
                TourId = b.TourId,
                TourScheduleId = b.TourScheduleId,
                TourGuideId = b.TourGuideId,
                TripPlanId = b.TripPlanId,
                WorkshopId = b.WorkshopId,
                WorkshopScheduleId = b.WorkshopScheduleId,
                PaymentLinkId = b.PaymentLinkId,
                Status = b.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                BookingType = b.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                BookingDate = b.BookingDate,
                CancelledAt = b.CancelledAt,
                PromotionId = b.PromotionId,
                OriginalPrice = b.OriginalPrice,
                DiscountAmount = b.DiscountAmount,
                FinalPrice = b.FinalPrice
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

    public async Task<bool> CancelBookingAsync(Guid bookingId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var user = await _unitOfWork.UserRepository
                .ActiveEntities
                .FirstOrDefaultAsync(u => u.Id == currentUserId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("user");

            var booking = await _unitOfWork.BookingRepository
                .ActiveEntities
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == currentUserId)
                ?? throw CustomExceptionFactory.CreateNotFoundError("tour");

            if (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Expired)
                throw CustomExceptionFactory.CreateBadRequestError("Không thể hủy đơn đã hoàn tất hoặc đã hết hạn.");

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = currentTime;
            booking.LastUpdatedTime = currentTime;

            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveAsync();

            return true;
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

    public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId)
    {
        try
        {
            // if (string.IsNullOrWhiteSpace(orderId))
            // {
            //     throw CustomExceptionFactory.CreateBadRequestError("Order ID không được để trống.");
            // }

            // if (!long.TryParse(orderId, out long parsedOrderId))
            // {
            //     throw CustomExceptionFactory.CreateBadRequestError("Order ID không hợp lệ.");
            // }
            return await _payOS.getPaymentLinkInformation(orderId);
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

    public async Task<List<BookingDataModel>> GetAllBookingsAsync(BookingFilterDto filter)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isAllowed = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            if (!isAllowed)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            IQueryable<Booking> query = _unitOfWork.BookingRepository
                .ActiveEntities
                .Include(b => b.TourGuide)
                    .ThenInclude(tg => tg != null ? tg.User : null)
                .Include(b => b.Tour)
                .Include(b => b.TourSchedule)
                .Include(b => b.TripPlan)
                .Include(b => b.Workshop)
                .Include(b => b.WorkshopSchedule)
                .Include(b => b.Promotion);

            // status
            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            // ngày tháng
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                if (filter.StartDate > filter.EndDate)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                }
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value && b.BookingDate <= filter.EndDate.Value);
            }
            else if (filter.StartDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value);
            }
            else if (filter.EndDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= filter.EndDate.Value);
            }

            var bookings = await query
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            var result = bookings.Select(b => new BookingDataModel
            {
                Id = b.Id,
                UserId = b.UserId,
                TourId = b.TourId,
                TourScheduleId = b.TourScheduleId,
                TourGuideId = b.TourGuideId,
                TripPlanId = b.TripPlanId,
                WorkshopId = b.WorkshopId,
                WorkshopScheduleId = b.WorkshopScheduleId,
                PaymentLinkId = b.PaymentLinkId,
                Status = b.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                BookingType = b.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                BookingDate = b.BookingDate,
                CancelledAt = b.CancelledAt,
                PromotionId = b.PromotionId,
                OriginalPrice = b.OriginalPrice,
                DiscountAmount = b.DiscountAmount,
                FinalPrice = b.FinalPrice
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

    public async Task<PagedResult<BookingDataModel>> GetPagedBookingsAsync(BookingFilterDto filter, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isAllowed = _userContextService.HasAnyRole(AppRole.MODERATOR, AppRole.ADMIN);
            if (!isAllowed)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            IQueryable<Booking> query = _unitOfWork.BookingRepository
                .ActiveEntities
                .Include(b => b.TourGuide)
                    .ThenInclude(tg => tg != null ? tg.User : null)
                .Include(b => b.Tour)
                .Include(b => b.TourSchedule)
                .Include(b => b.TripPlan)
                .Include(b => b.Workshop)
                .Include(b => b.WorkshopSchedule)
                .Include(b => b.Promotion);

            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            // Lọc theo ngày
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                if (filter.StartDate > filter.EndDate)
                {
                    throw CustomExceptionFactory.CreateBadRequestError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                }
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value && b.BookingDate <= filter.EndDate.Value);
            }
            else if (filter.StartDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= filter.StartDate.Value);
            }
            else if (filter.EndDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= filter.EndDate.Value);
            }

            // Tổng số bản ghi trước khi phân trang
            var totalRecords = await query.CountAsync();

            // Phân trang
            var bookings = await query
                .OrderBy(b => b.BookingDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<BookingDataModel>();

            foreach (var b in bookings)
            {
                var userName = _unitOfWork.UserRepository
                    .ActiveEntities
                    .Where(u => u.Id == b.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault();

                var tourName = _unitOfWork.TourRepository
                    .ActiveEntities
                    .Where(u => u.Id == b.TourId)
                    .Select(u => u.Name)
                    .FirstOrDefault();

                string? tourGuideName = null;
                if (b.TourGuideId.HasValue)
                {
                    var tourGuide = _unitOfWork.TourGuideRepository
                        .ActiveEntities
                        // .Include(tg => tg.User)
                        .Where(u => u.Id == b.TourGuideId)
                        .Select(u => u.User.FullName)
                        .FirstOrDefault();
                    tourGuideName = tourGuide;
                }

                var bookingModel = new BookingDataModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = userName,
                    TourId = b.TourId,
                    TourName = tourName,
                    TourScheduleId = b.TourScheduleId,
                    TourGuideId = b.TourGuideId,
                    TourGuideName = tourGuideName,
                    TripPlanId = b.TripPlanId,
                    WorkshopId = b.WorkshopId,
                    WorkshopScheduleId = b.WorkshopScheduleId,
                    PaymentLinkId = b.PaymentLinkId,
                    Status = b.Status,
                    StatusText = _enumService.GetEnumDisplayName<BookingStatus>(b.Status),
                    BookingType = b.BookingType,
                    BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(b.BookingType),
                    BookingDate = b.BookingDate,
                    CancelledAt = b.CancelledAt,
                    PromotionId = b.PromotionId,
                    OriginalPrice = b.OriginalPrice,
                    DiscountAmount = b.DiscountAmount,
                    FinalPrice = b.FinalPrice
                };

                result.Add(bookingModel);
            }

            return new PagedResult<BookingDataModel>
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


    public async Task<BookingDataModel> GetBookingByIdAsync(Guid bookingId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var isUser = _userContextService.HasRole(AppRole.USER);
            var isAdminOrModerator = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);

            if (!isUser && !isAdminOrModerator)
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            var query = _unitOfWork.BookingRepository
                .ActiveEntities
                .Where(b => b.Id == bookingId);

            if (!isAdminOrModerator)
            {
                query = query.Where(b => b.UserId == currentUserId);
            }

            var booking = await query
                .Include(b => b.TourGuide)
                    .ThenInclude(tg => tg != null ? tg.User : null)
                .Include(b => b.Tour)
                .Include(b => b.TourSchedule)
                .Include(b => b.TripPlan)
                .Include(b => b.Workshop)
                .Include(b => b.WorkshopSchedule)
                .Include(b => b.Promotion)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Booking không tồn tại hoặc không thuộc về bạn.");
            }

            var result = new BookingDataModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TourId = booking.TourId,
                TourScheduleId = booking.TourScheduleId,
                TourGuideId = booking.TourGuideId,
                TripPlanId = booking.TripPlanId,
                WorkshopId = booking.WorkshopId,
                WorkshopScheduleId = booking.WorkshopScheduleId,
                PaymentLinkId = booking.PaymentLinkId,
                Status = booking.Status,
                StatusText = _enumService.GetEnumDisplayName<BookingStatus>(booking.Status),
                BookingType = booking.BookingType,
                BookingTypeText = _enumService.GetEnumDisplayName<BookingType>(booking.BookingType),
                BookingDate = booking.BookingDate,
                CancelledAt = booking.CancelledAt,
                PromotionId = booking.PromotionId,
                OriginalPrice = booking.OriginalPrice,
                DiscountAmount = booking.DiscountAmount,
                FinalPrice = booking.FinalPrice
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

    private async Task<(decimal DiscountAmount, Promotion? Promotion)> ValidateAndCalculateDiscountAsync(string? promotionCode, decimal basePrice, Guid tourId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(promotionCode))
                return (0, null);

            var promotion = await _unitOfWork.PromotionRepository
                .ActiveEntities
                .Include(p => p.PromotionApplicables)
                .FirstOrDefaultAsync(p => p.PromotionCode == promotionCode);

            if (promotion == null)
                throw CustomExceptionFactory.CreateBadRequestError("Không tim thấy mã giảm giá");

            // kieem tra ngày
            var currentDate = DateTimeOffset.UtcNow;
            if (promotion.StartDate > currentDate || promotion.EndDate < currentDate)
                throw CustomExceptionFactory.CreateBadRequestError("Promotion is not active or has expired.");

            // đối tượng áp dụng
            if (promotion.ApplicableType != ServiceOption.Tour || promotion.ApplicableType != ServiceOption.Both)
                throw CustomExceptionFactory.CreateBadRequestError("Mã giảm giá không áp dụng cho tour.");

            bool isApplicable = promotion.PromotionApplicables.Any(pa => pa.TourId == tourId);
            if (!isApplicable)
                throw CustomExceptionFactory.CreateBadRequestError("Mã giảm giá không áp dụng với tour này");

            decimal discountAmount = promotion.DiscountType switch
            {
                DiscountType.Fixed => promotion.DiscountValue,
                DiscountType.Percentage => basePrice * (promotion.DiscountValue / 100),
                _ => throw CustomExceptionFactory.CreateBadRequestError("Loại giảm giá không phù hợp")
            };

            return (discountAmount, promotion);
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }
}