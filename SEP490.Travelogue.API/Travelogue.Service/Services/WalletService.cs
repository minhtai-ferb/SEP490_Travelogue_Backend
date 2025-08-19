using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.BankAccountModels;
using Travelogue.Service.BusinessModels.TransactionModels;
using Travelogue.Service.BusinessModels.WithdrawalRequestModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IWalletService
{
    Task<decimal> GetBalanceAsync();
    Task<List<TransactionDto>> GetTransactionsAsync();
    Task RequestWithdrawalAsync(WithdrawalRequestCreateDto request);
    Task<List<WithdrawalRequestDto>> GetWithdrawalRequestAsync(WithdrawalRequestFilterDto filterDto);
    Task<List<WithdrawalRequestDto>> GetMyWithdrawalRequestAsync(MyWithdrawalRequestFilterDto filterDto);
    Task ApproveAsync(Guid requestId, string proofImageUrl, string? adminNote);
    Task RejectAsync(Guid requestId, string reason);
}

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IEnumService _enumService;
    private readonly ITimeService _timeService;

    public WalletService(IUnitOfWork unitOfWork, IUserContextService userContextService, IEnumService enumService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _enumService = enumService;
        _timeService = timeService;
    }

    public async Task<decimal> GetBalanceAsync()
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var wallet = await _unitOfWork.WalletRepository
                .ActiveEntities
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);
            return wallet?.Balance ?? 0m;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync()
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var wallet = await _unitOfWork.WalletRepository
                .ActiveEntities
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);

            if (wallet == null)
                return new List<TransactionDto>();

            var transactions = await _unitOfWork.TransactionEntryRepository
                .ActiveEntities
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                UserId = t.UserId,
                AccountNumber = t.AccountNumber,
                PaidAmount = t.PaidAmount,
                PaymentReference = t.PaymentReference,
                TransactionDateTime = t.TransactionDateTime,
                CounterAccountBankId = t.CounterAccountBankId,
                CounterAccountName = t.CounterAccountName,
                CounterAccountNumber = t.CounterAccountNumber,
                Currency = t.Currency,
                PaymentLinkId = t.PaymentLinkId,
                PaymentStatus = t.PaymentStatus,
                PaymentStatusText = _enumService.GetEnumDisplayName<PaymentStatus>(t.PaymentStatus),
                Status = t.Status,
                StatusText = _enumService.GetEnumDisplayName<TransactionStatus>(t.Status),
                Type = t.Type,
                TypeText = _enumService.GetEnumDisplayName<TransactionType>(t.Type),
            }).ToList();
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

    public async Task RequestWithdrawalAsync(WithdrawalRequestCreateDto request)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var currentTime = _timeService.SystemTimeNow;

            var hasPendingRequest = await _unitOfWork.WithdrawalRequestRepository
                .ActiveEntities
                .AnyAsync(wr => wr.UserId == currentUserId && wr.Status == WithdrawalRequestStatus.Pending);

            if (hasPendingRequest)
                throw CustomExceptionFactory.CreateBadRequestError("Bạn đang có một yêu cầu rút tiền đang chờ xử lý. Vui lòng đợi admin duyệt trước khi tạo yêu cầu mới.");


            var wallet = await _unitOfWork.WalletRepository
                .ActiveEntities
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);

            if (wallet == null)
                throw CustomExceptionFactory.CreateNotFoundError("Wallet");

            if (request.Amount < 10000)
                throw CustomExceptionFactory.CreateBadRequestError("Số tiền muốn rút ít nhất là 10.000");

            if (wallet.Balance < request.Amount)
                throw CustomExceptionFactory.CreateBadRequestError("Không đủ số dư");

            await _unitOfWork.WithdrawalRequestRepository.AddAsync(new WithdrawalRequest
            {
                WalletId = wallet.Id,
                UserId = currentUserId,
                Amount = request.Amount,
                BankAccountId = request.BankAccountId,
                Note = request.Note,
                RequestTime = currentTime.DateTime,
                Status = WithdrawalRequestStatus.Pending,
            });

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

    public async Task<List<WithdrawalRequestDto>> GetWithdrawalRequestAsync(WithdrawalRequestFilterDto filterDto)
    {
        try
        {
            var query = _unitOfWork.WithdrawalRequestRepository.ActiveEntities
                .Include(wr => wr.User)
                .Include(wr => wr.Wallet)
                .Include(wr => wr.BankAccount)
                .AsQueryable();

            if (filterDto.UserId.HasValue && filterDto.UserId.Value != Guid.Empty)
            {
                query = query.Where(wr => wr.UserId == filterDto.UserId.Value);
            }

            if (filterDto.Status.HasValue)
            {
                query = query.Where(wr => wr.Status == filterDto.Status.Value);
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(wr => wr.CreatedTime >= filterDto.FromDate.Value);
            }
            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(wr => wr.CreatedTime <= filterDto.ToDate.Value);
            }

            var entities = await query
                .OrderByDescending(wr => wr.CreatedTime)
                .ToListAsync();

            var result = entities.Select(wr => new WithdrawalRequestDto
            {
                Id = wr.Id,
                WalletId = wr.WalletId,
                WalletBalance = wr.Wallet?.Balance ?? 0.0m,
                UserId = wr.UserId,
                UserName = wr.User.FullName,
                Amount = wr.Amount,
                Status = wr.Status,
                StatusText = _enumService.GetEnumDisplayName<WithdrawalRequestStatus>(wr.Status),
                BankAccountId = wr.BankAccountId,
                RequestTime = wr.RequestTime,
                BankAccount = wr.BankAccount == null ? null : new BankAccountDto
                {
                    Id = wr.BankAccount.Id,
                    UserId = wr.BankAccount.UserId,
                    BankName = wr.BankAccount.BankName,
                    BankAccountNumber = wr.BankAccount.BankAccountNumber,
                    BankOwnerName = wr.BankAccount.BankOwnerName
                }
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

    public async Task<List<WithdrawalRequestDto>> GetMyWithdrawalRequestAsync(MyWithdrawalRequestFilterDto filterDto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());

            var query = _unitOfWork.WithdrawalRequestRepository.ActiveEntities
                .Where(w => w.UserId == currentUserId)
                .Include(wr => wr.User)
                .Include(wr => wr.BankAccount)
                .AsQueryable();

            if (filterDto.Status.HasValue)
            {
                query = query.Where(wr => wr.Status == filterDto.Status.Value);
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(wr => wr.CreatedTime >= filterDto.FromDate.Value);
            }
            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(wr => wr.CreatedTime <= filterDto.ToDate.Value);
            }

            var entities = await query
                .OrderByDescending(wr => wr.CreatedTime)
                .ToListAsync();

            var result = entities.Select(wr => new WithdrawalRequestDto
            {
                Id = wr.Id,
                WalletId = wr.WalletId,
                Amount = wr.Amount,
                UserId = wr.UserId,
                UserName = wr.User.FullName,
                Status = wr.Status,
                StatusText = _enumService.GetEnumDisplayName<WithdrawalRequestStatus>(wr.Status),
                BankAccountId = wr.BankAccountId,
                RequestTime = wr.RequestTime,
                BankAccount = wr.BankAccount == null ? null : new BankAccountDto
                {
                    Id = wr.BankAccount.Id,
                    UserId = wr.BankAccount.UserId,
                    BankName = wr.BankAccount.BankName,
                    BankAccountNumber = wr.BankAccount.BankAccountNumber,
                    BankOwnerName = wr.BankAccount.BankOwnerName
                }
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

    public async Task ApproveAsync(Guid requestId, string proofImageUrl, string? adminNote)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.WithdrawalRequestRepository
                .ActiveEntities
                .Include(r => r.Wallet)
                .Where(r => r.Id == requestId)
                .FirstOrDefaultAsync();
            if (request == null || request.Status != WithdrawalRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Yeeu cầu không hợp lệ");

            request.Status = WithdrawalRequestStatus.Approved;
            request.ProofImageUrl = proofImageUrl;
            request.Note = adminNote;
            request.LastUpdatedTime = currentTime;

            request.Wallet.Balance -= request.Amount;

            _unitOfWork.WalletRepository.Update(request.Wallet);
            _unitOfWork.WithdrawalRequestRepository.Update(request);
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

    public async Task RejectAsync(Guid requestId, string reason)
    {
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var hasPermission = _userContextService.HasAnyRole(AppRole.ADMIN, AppRole.MODERATOR);
            if (!hasPermission)
                throw CustomExceptionFactory.CreateForbiddenError();

            var currentTime = _timeService.SystemTimeNow;

            var request = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(requestId, new CancellationToken());
            if (request == null || request.Status != WithdrawalRequestStatus.Pending)
                throw CustomExceptionFactory.CreateBadRequestError("Yeeu cầu không hợp lệ");

            request.Status = WithdrawalRequestStatus.Rejected;
            request.Note = reason;
            request.LastUpdatedTime = currentTime;

            _unitOfWork.WithdrawalRequestRepository.Update(request);
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
