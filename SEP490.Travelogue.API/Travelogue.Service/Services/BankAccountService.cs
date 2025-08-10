using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.BankAccountModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface IBankAccountService
{
    Task<List<BankAccountDto>> GetUserBankAccountsAsync(Guid userId);
    Task<BankAccountDto> AddBankAccountAsync(BankAccountCreateDto dto);
    Task<BankAccountDto> UpdateBankAccountAsync(Guid bankAccountId, BankAccountUpdateDto dto);
    Task DeleteBankAccountAsync(Guid bankAccountId);
}

public class BankAccountService : IBankAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public BankAccountService(IUnitOfWork unitOfWork, IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<List<BankAccountDto>> GetUserBankAccountsAsync(Guid userId)
    {
        try
        {
            var accounts = await _unitOfWork.BankAccountRepository
                .ActiveEntities
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return accounts.Select(a => new BankAccountDto
            {
                Id = a.Id,
                BankName = a.BankName,
                BankAccountNumber = a.BankAccountNumber,
                BankOwnerName = a.BankOwnerName,
                IsDefault = a.IsDefault
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

    public async Task<BankAccountDto> AddBankAccountAsync(BankAccountCreateDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var entity = new BankAccount
            {
                UserId = currentUserId,
                BankName = dto.BankName,
                BankAccountNumber = dto.BankAccountNumber,
                BankOwnerName = dto.BankOwnerName,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BankAccountRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return new BankAccountDto
            {
                Id = entity.Id,
                BankName = entity.BankName,
                BankAccountNumber = entity.BankAccountNumber,
                BankOwnerName = entity.BankOwnerName,
                IsDefault = entity.IsDefault
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

    public async Task<BankAccountDto> UpdateBankAccountAsync(Guid bankAccountId, BankAccountUpdateDto dto)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var account = await _unitOfWork.BankAccountRepository
                .ActiveEntities
                .FirstOrDefaultAsync(b => b.Id == bankAccountId && b.UserId == currentUserId);

            if (account == null)
                throw CustomExceptionFactory.CreateNotFoundError("Bank account");

            account.BankName = dto.BankName;
            account.BankAccountNumber = dto.BankAccountNumber;
            account.BankOwnerName = dto.BankOwnerName;

            if (dto.IsDefault)
            {
                var allAccounts = await _unitOfWork.BankAccountRepository
                    .ActiveEntities
                    .Where(b => b.UserId == currentUserId)
                    .ToListAsync();

                foreach (var acc in allAccounts)
                    acc.IsDefault = acc.Id == bankAccountId;
            }
            else
            {
                account.IsDefault = dto.IsDefault;
            }

            await _unitOfWork.SaveAsync();

            return new BankAccountDto
            {
                Id = account.Id,
                BankName = account.BankName,
                BankAccountNumber = account.BankAccountNumber,
                BankOwnerName = account.BankOwnerName,
                IsDefault = account.IsDefault
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

    public async Task DeleteBankAccountAsync(Guid bankAccountId)
    {
        try
        {
            var currentUserId = Guid.Parse(_userContextService.GetCurrentUserId());
            var account = await _unitOfWork.BankAccountRepository
                .ActiveEntities
                .FirstOrDefaultAsync(b => b.Id == bankAccountId && b.UserId == currentUserId);

            if (account == null)
                throw CustomExceptionFactory.CreateNotFoundError("Bank account");

            var hasTransactions = await _unitOfWork.WithdrawalRequestRepository
                .ActiveEntities
                .AnyAsync(wr => wr.BankAccountId == bankAccountId);

            if (hasTransactions)
            {
                account.IsActive = false;
            }
            else
            {
                _unitOfWork.BankAccountRepository.Remove(account);
            }

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
