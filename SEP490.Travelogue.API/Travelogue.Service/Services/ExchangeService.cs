using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.ExchangeModels;
using Travelogue.Service.BusinessModels.ExchangeSessionModels;
using Travelogue.Service.Commons.Helpers;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IExchangeService
{
    // Task<ExchangeDataModel?> GetExchangeByIdAsync(Guid id, CancellationToken cancellationToken);
    // Task<List<ExchangeDataModel>> GetAllExchangesAsync(CancellationToken cancellationToken);
    Task AddExchangeAsync(ExchangeCreateModel exchangeCreateModel, CancellationToken cancellationToken);

    Task UpdateUserResponseAsync(UpdateUserResponseModel updateUserResponseModel, CancellationToken cancellationToken);

    // Task<List<ExchangeSessionDataModel>> GetSessionsByTripPlanAsync(Guid tripPlanId, CancellationToken cancellationToken);
    // Task UpdateExchangeAsync(Guid id, ExchangeUpdateModel ExchangeUpdateModel, CancellationToken cancellationToken);
    // Task DeleteExchangeAsync(Guid id, CancellationToken cancellationToken);
    // Task<PagedResult<ExchangeDataModel>> GetPagedExchangesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    // Task<PagedResult<ExchangeDataModel>> GetPagedExchangesWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public class ExchangeService : IExchangeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public ExchangeService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    // tour guide gửi lại trip plan version mới cho user
    public async Task AddExchangeAsync(ExchangeCreateModel exchangeCreateModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var newExchange = _mapper.Map<TripPlanExchange>(exchangeCreateModel);

            newExchange.RequestedAt = currentTime;
            newExchange.Status = ExchangeSessionStatus.Pending;

            newExchange.CreatedBy = currentUserId;
            newExchange.LastUpdatedBy = currentUserId;
            newExchange.CreatedTime = currentTime;
            newExchange.LastUpdatedTime = currentTime;

            _unitOfWork.BeginTransaction();
            await _unitOfWork.TripPlanExchangeRepository.AddAsync(newExchange);
            _unitOfWork.CommitTransaction();
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }

    // public async Task<List<ExchangeSessionDataModel>> GetSessionsByTripPlanAsync(Guid tripPlanId, CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var currentUserId = _userContextService.GetCurrentUserId();
    //         var currentTime = _timeService.SystemTimeNow;

    //         var existingExchangeSession = await _unitOfWork.TripPlanExchangeSessionRepository
    //             .ActiveEntities
    //             .Where(x => x.TripPlanId == tripPlanId && x.CreatedByUserId == Guid.Parse(currentUserId))
    //             .ToListAsync(cancellationToken)
    //                 ?? throw CustomExceptionFactory.CreateNotFoundError("Exchange not found");

    //         var exchangeSessionDataModels = _mapper.Map<List<ExchangeSessionDataModel>>(existingExchangeSession);

    //         return exchangeSessionDataModels;
    //     }
    //     catch (CustomException)
    //     {
    //         throw;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
    //     }
    //     finally
    //     {
    //         //  _unitOfWork.Dispose();
    //     }
    // }

    public async Task UpdateUserResponseAsync(UpdateUserResponseModel updateUserResponseModel, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var existingExchange = await _unitOfWork.TripPlanExchangeRepository.GetByIdAsync(updateUserResponseModel.ExchangeId, cancellationToken)
                ?? throw CustomExceptionFactory.CreateNotFoundError("Exchange not found");
            if (existingExchange.UserId != Guid.Parse(currentUserId))
            {
                throw CustomExceptionFactory.CreateForbiddenError();
            }

            existingExchange.Status = updateUserResponseModel.Status switch
            {
                UserExchangeResponseStatus.AcceptedByUser => ExchangeSessionStatus.AcceptedByUser,
                UserExchangeResponseStatus.RejectedByUser => ExchangeSessionStatus.RejectedByUser,
                _ => throw CustomExceptionFactory.CreateBadRequestError("Invalid user response status")
            };

            existingExchange.ResponseMessage = updateUserResponseModel.UserResponseMessage;
            existingExchange.RespondedAt = currentTime;

            _unitOfWork.BeginTransaction();
            _unitOfWork.TripPlanExchangeRepository.Update(existingExchange);
            _unitOfWork.CommitTransaction();

            var response = new UpdateUserResponseModel
            {
                ExchangeId = existingExchange.Id,
                Status = updateUserResponseModel.Status,
                UserResponseMessage = updateUserResponseModel.UserResponseMessage
            };
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            //  _unitOfWork.Dispose();
        }
    }
}