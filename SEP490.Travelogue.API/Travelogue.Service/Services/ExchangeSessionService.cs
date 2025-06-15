using AutoMapper;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ExchangeSessionModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IExchangeSessionService
{
    Task CreateExchangeSessionAsync(CreateExchangeSessionRequest request, CancellationToken cancellationToken);
}

public class ExchangeSessionService : IExchangeSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public ExchangeSessionService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task CreateExchangeSessionAsync(CreateExchangeSessionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIdAsync(request.TourGuideId, cancellationToken);
            if (tourGuide == null)
            {
                throw CustomExceptionFactory.CreateNotFoundError("Tour guide");
            }

            var tripPlanVersion = await _unitOfWork.TripPlanVersionRepository.GetByIdAsync(request.TripPlanVersionId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan version");

            var tripPlan = await _unitOfWork.TripPlanRepository.GetByIdAsync(tripPlanVersion.TripPlanId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Trip plan");

            var newSession = new TripPlanExchangeSession
            {
                TourGuideId = tourGuide.Id,
                TripPlanId = tripPlan.Id,
                CreatedByUserId = Guid.Parse(currentUserId),
                FinalStatus = ExchangeSessionStatus.Pending,
                CreatedBy = currentUserId,
                CreatedAt = currentTime,
                LastUpdatedBy = currentUserId,
                LastUpdatedTime = currentTime
            };

            await _unitOfWork.TripPlanExchangeSessionRepository.AddAsync(newSession);
            await _unitOfWork.SaveAsync();

        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-----------------Error creating exchange session: {ex.Message}");
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

}
