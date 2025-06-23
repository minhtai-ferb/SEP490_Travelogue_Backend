using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    Task<ExchangeSessionDataDetailModel> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<List<ExchangeSessionDataModel>> GetSessionsByTripPlanAsync(Guid tripPlanId, CancellationToken cancellationToken);
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

    public async Task<List<ExchangeSessionDataModel>> GetSessionsByTripPlanAsync(Guid tripPlanId, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingExchangeSessions = await _unitOfWork.TripPlanExchangeSessionRepository
                .ActiveEntities
                .Where(x => x.TripPlanId == tripPlanId && x.CreatedByUserId == Guid.Parse(currentUserId))
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ExchangeSessionDataModel>>(existingExchangeSessions);
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-----------------Error getting exchange sessions: {ex.Message}");
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<ExchangeSessionDataDetailModel> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingExchangeSession = await _unitOfWork.TripPlanExchangeSessionRepository
                .ActiveEntities
                .Include(x => x.Exchanges)                  // lấy tất cả Exchange liên quan
                .Include(x => x.TourGuide)                  // nếu bạn muốn lấy tên hướng dẫn viên
                .Include(x => x.TripPlan)                   // nếu muốn lấy tên TripPlan
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.CreatedByUserId == Guid.Parse(currentUserId), cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Exchange session");

            var response = _mapper.Map<ExchangeSessionDataDetailModel>(existingExchangeSession);

            return response;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-----------------Error getting exchange session by ID: {ex.Message}");
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }
}
