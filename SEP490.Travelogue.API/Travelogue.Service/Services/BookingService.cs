using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.BookingModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IBookingService
{
    // Task<BookingDataModel?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookingDataModel> AddBookingAsync(BookingCreateModel tripPlanCreateModel, CancellationToken cancellationToken);
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

            var newBooking = _mapper.Map<Booking>(bookingCreateModel);

            newBooking.CreatedBy = currentUserId;
            newBooking.LastUpdatedBy = currentUserId;
            newBooking.CreatedTime = currentTime;
            newBooking.LastUpdatedTime = currentTime;

            await _unitOfWork.BookingRepository.AddAsync(newBooking);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);

            var result = _unitOfWork.BookingRepository.ActiveEntities
                .FirstOrDefault(tp => tp.Id == newBooking.Id);

            return _mapper.Map<BookingDataModel>(result);
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
}