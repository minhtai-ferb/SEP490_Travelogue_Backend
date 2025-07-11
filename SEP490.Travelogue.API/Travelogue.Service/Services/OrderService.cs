using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IOrderService
{
    Task<CreatePaymentResult> CreatePaymentLink(CreateBookingRequest request, CancellationToken cancellationToken = default);
}

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;
    private readonly PayOS _payOS;

    public OrderService(IUnitOfWork unitOfWork, PayOS payOS, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _payOS = payOS;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task<CreatePaymentResult> CreatePaymentLink(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            if (!Guid.TryParse(currentUserId, out Guid currentUserIdGuid))
                throw CustomExceptionFactory.CreateBadRequestError("Invalid user ID format.");

            // Validate tour plan version
            var tour = await _unitOfWork.TourRepository.GetAsync(
                tp => tp.Id == request.TourId,
                q => q.Include(tp => tp.TourSchedules),
                cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour plan version not found.");

            // Validate tour guide
            var tourGuide = await _unitOfWork.TourGuideRepository.GetAsync(
                tg => tg.Id == request.TourGuideId,
                q => q.Include(tg => tg.User).Include(tg => tg.TourGuideSchedules),
                cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Tour guide not found.");

            TourSchedule? tourSchedule = null;
            DateTimeOffset? startDate;
            DateTimeOffset? endDate;
            decimal totalAmount;
            int totalDays;

            if (request.TourScheduleId.HasValue)
            {
                // Predefined tour: Validate tour schedule and guide assignment
                tourSchedule = await _unitOfWork.TourScheduleRepository.GetAsync(
                    ts => ts.Id == request.TourScheduleId.Value && ts.TourId == request.TourId,
                    q => q.Include(ts => ts.TourScheduleGuides),
                    cancellationToken)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Tour schedule not found or not associated with the tour plan version.");

                if (!tourSchedule.TourScheduleGuides.Any(tsg => tsg.TourGuideId == request.TourGuideId))
                    throw CustomExceptionFactory.CreateBadRequestError("Tour guide is not assigned to this tour schedule.");

                // Use tour schedule dates
                startDate = tourSchedule.DepartureDate;
                endDate = tourSchedule.DepartureDate.AddDays(tourSchedule.TotalDays - 1);
                totalDays = tourSchedule.TotalDays;

                // Check participant capacity
                int totalParticipants = request.AdultCount + request.ChildCount;
                if (totalParticipants <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError("At least one participant is required.");
                if (tourSchedule.CurrentBooked + totalParticipants > tourSchedule.MaxParticipant)
                    throw CustomExceptionFactory.CreateBadRequestError("Tour schedule has reached maximum participant capacity.");

                // Use tour schedule pricing
                totalAmount = (request.AdultCount * tourSchedule.AdultPrice) + (request.ChildCount * tourSchedule.ChildrenPrice);
            }
            else
            {
                // Custom trip plan: Validate trip plan version
                var bookingRequest = await _unitOfWork.TripPlanExchangeRepository
                    .ActiveEntities
                    .Include(b => b.Session)
                    .FirstOrDefaultAsync(
                        b => b.SuggestedTripPlanVersionId == request.TripPlanVersionId
                        && b.TourGuideId == request.TourGuideId,
                        cancellationToken)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Booking request not found for the specified trip plan version and tour guide.");

                // Check if the user sent this booking request
                if (bookingRequest.UserId != Guid.Parse(currentUserId))
                {
                    throw CustomExceptionFactory.CreateBadRequestError("You are not the user who sent this booking request.");
                }

                if (bookingRequest.Session.FinalStatus != ExchangeSessionStatus.AcceptedByUser)
                    throw CustomExceptionFactory.CreateBadRequestError("This booking request has not been accepted by the user yet.");

                var tripPlanVersion = await _unitOfWork.TripPlanVersionRepository.GetAsync(
                    tp => tp.Id == request.TripPlanVersionId,
                    q => q.Include(tp => tp.TripPlan),
                    cancellationToken)
                    ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version not found.");

                if (tripPlanVersion.TripPlan == null)
                    throw CustomExceptionFactory.CreateNotFoundError("Trip plan not found for the specified trip plan version.");

                int tripTotalDays = (int)(tripPlanVersion.TripPlan.EndDate.Date - tripPlanVersion.TripPlan.StartDate.Date).TotalDays + 1;

                if (tripTotalDays <= 0)
                    throw CustomExceptionFactory.CreateBadRequestError("Total days must be greater than zero for custom trip plans.");

                startDate = request.StartDate;
                endDate = request.EndDate;
                totalDays = tripTotalDays;

                if (endDate < startDate)
                    throw CustomExceptionFactory.CreateBadRequestError("End date cannot be earlier than start date.");
                if (((endDate - startDate)?.Days ?? 0) + 1 != totalDays)
                    throw CustomExceptionFactory.CreateBadRequestError("Date range must match trip plan total days.");

                // Use tour guide price
                totalAmount = tourGuide.Price * totalDays;
            }

            // Check tour guide availability
            var conflictingSchedules = await _unitOfWork.TourGuideScheduleRepository.FindAsync(
                s => s.TourGuideId == request.TourGuideId &&
                     s.Date >= startDate && s.Date <= endDate,
                cancellationToken);
            if (conflictingSchedules.Any())
                throw CustomExceptionFactory.CreateBadRequestError("Tour guide is not available for the requested dates.");

            // Calculate total price
            if (totalAmount <= 0)
                throw CustomExceptionFactory.CreateBadRequestError("Total amount must be greater than zero.");

            // Create booking
            var booking = new Booking
            {
                TourId = request.TourId,
                TourScheduleId = request.TourScheduleId,
                TourGuideId = request.TourGuideId,
                UserId = currentUserIdGuid,
                OriginalPrice = totalAmount,
                DiscountAmount = totalAmount,
                FinalPrice = totalAmount,
                Status = BookingStatus.Pending,
                Participants = new List<BookingParticipant>
                {
                    new BookingParticipant
                    {
                        Type = ParticipantType.Adult,
                        Quantity = request.AdultCount,
                        PricePerParticipant = request.TourScheduleId.HasValue ? tourSchedule!.AdultPrice : tourGuide.Price
                    },
                    new BookingParticipant
                    {
                        Type = ParticipantType.Child,
                        Quantity = request.ChildCount,
                        PricePerParticipant = request.TourScheduleId.HasValue ? tourSchedule!.ChildrenPrice : tourGuide.Price
                    }
                }
            };

            // Update CurrentBooked for predefined tours
            if (tourSchedule != null)
            {
                tourSchedule.CurrentBooked += request.AdultCount + request.ChildCount;
                _unitOfWork.TourScheduleRepository.Update(tourSchedule);
            }

            // Generate order code
            int orderCode = int.Parse(currentTime.ToString("yyyyMMddHHmmss"));

            // Prepare PayOS payment data
            string tourDescription = request.TourScheduleId.HasValue
                ? $"Tour: {tour.Name ?? "Custom Tour"} (Run: {startDate:yyyy-MM-dd}, Adults: {request.AdultCount}, Children: {request.ChildCount})"
                : $"Custom Trip Plan with {tourGuide.User.FullName} (Adults: {request.AdultCount}, Children: {request.ChildCount})";

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
                description: $"Payment for {tourDescription} from {startDate:yyyy-MM-dd}",
                items: items,
                cancelUrl: "http://yourapp.com/cancel",
                returnUrl: "http://yourapp.com/success",
                expiredAt: Convert.ToInt64(currentTime.AddMinutes(5))
            );

            // Create PayOS payment link
            var paymentResult = await _payOS.createPaymentLink(paymentData);

            // Link payment to booking
            booking.PaymentLinkId = paymentResult.paymentLinkId;

            // Add booking and schedule entries
            await _unitOfWork.BookingRepository.AddAsync(booking);

            // Add schedule entries for the tour guide
            for (var date = startDate; date <= endDate; date = date.Value.AddDays(1))
            {
                await _unitOfWork.TourGuideScheduleRepository.AddAsync(new TourGuideSchedule
                {
                    TourGuideId = request.TourGuideId,
                    Date = date ?? throw CustomExceptionFactory.CreateBadRequestError("Invalid date for tour guide schedule."),
                    BookingId = booking.Id
                });
            }

            await _unitOfWork.SaveAsync();
            return paymentResult;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError($"Failed to create payment link: {ex.Message}");
        }
    }
}