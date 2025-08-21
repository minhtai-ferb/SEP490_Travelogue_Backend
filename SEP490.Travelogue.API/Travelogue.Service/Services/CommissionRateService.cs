using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.CommissionSettingModels;

namespace Travelogue.Service.Services;

public interface ICommissionRateService
{
    Task<IEnumerable<CommissionRateDto>> GetAllAsync();
    Task<IEnumerable<CommissionRateGroupDto>> GetAllGroupAsync();
    Task<CommissionRateDto?> GetByIdAsync(Guid id);
    Task<CommissionRateDto?> GetCurrentAsync(CommissionType type, DateTime? onDate = null);
    Task<CommissionRateDto> CreateAsync(CommissionRateCreateDto dto);
    Task UpdateAsync(Guid id, decimal newRateValue);
    Task DeleteAsync(Guid id);
}

public class CommissionRateService : ICommissionRateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumService _enumService;

    public CommissionRateService(IUnitOfWork unitOfWork, IEnumService enumService)
    {
        _unitOfWork = unitOfWork;
        _enumService = enumService;
    }

    public async Task<IEnumerable<CommissionRateDto>> GetAllAsync()
    {
        try
        {
            var rates = await _unitOfWork.CommissionRateRepository
                .ActiveEntities
                .OrderByDescending(c => c.EffectiveDate)
                .ToListAsync();

            return rates.Select(r => new CommissionRateDto
            {
                Id = r.Id,
                Type = r.Type,
                CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(r.Type),
                RateValue = r.RateValue,
                EffectiveDate = r.EffectiveDate,
                ExpiryDate = r.ExpiryDate
            });
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

    public async Task<IEnumerable<CommissionRateGroupDto>> GetAllGroupAsync()
    {
        try
        {
            var rates = await _unitOfWork.CommissionRateRepository
                .ActiveEntities
                .OrderByDescending(c => c.EffectiveDate)
                .ToListAsync();

            var grouped = rates
                .GroupBy(r => r.Type)
                .Select(g => new CommissionRateGroupDto
                {
                    Type = g.Key,
                    CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(g.Key),
                    Rates = g.Select(r => new CommissionRateDto
                    {
                        Id = r.Id,
                        Type = r.Type,
                        CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(r.Type),
                        RateValue = r.RateValue,
                        EffectiveDate = r.EffectiveDate,
                        ExpiryDate = r.ExpiryDate
                    }).OrderByDescending(r => r.EffectiveDate).ToList()
                });

            return grouped;
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


    public async Task<CommissionRateDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var rate = await _unitOfWork.CommissionRateRepository
                .GetByIdAsync(id, new CancellationToken());

            if (rate == null)
                return null;

            return new CommissionRateDto
            {
                Id = rate.Id,
                Type = rate.Type,
                CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(rate.Type),
                RateValue = rate.RateValue,
                EffectiveDate = rate.EffectiveDate,
                ExpiryDate = rate.ExpiryDate
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

    public async Task<CommissionRateDto?> GetCurrentAsync(CommissionType type, DateTime? onDate = null)
    {
        try
        {
            var date = onDate ?? DateTime.UtcNow;

            var rate = await _unitOfWork.CommissionRateRepository
                .ActiveEntities
                .Where(r => r.Type == type
                         && r.EffectiveDate <= date
                         && (r.ExpiryDate == null || r.ExpiryDate >= date))
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync();

            if (rate == null)
                return null;

            return new CommissionRateDto
            {
                Id = rate.Id,
                Type = rate.Type,
                CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(rate.Type),
                RateValue = rate.RateValue,
                EffectiveDate = rate.EffectiveDate,
                ExpiryDate = rate.ExpiryDate
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

    public async Task<CommissionRateDto> CreateAsync(CommissionRateCreateDto dto)
    {
        try
        {
            var date = DateTime.UtcNow;

            var current = await _unitOfWork.CommissionRateRepository
                .ActiveEntities
                .Where(r => r.Type == dto.Type
                         && r.EffectiveDate <= date
                         && (r.ExpiryDate == null || r.ExpiryDate >= date))
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync();

            if (current != null)
            {
                current.ExpiryDate = dto.EffectiveDate.AddSeconds(-1);
                _unitOfWork.CommissionRateRepository.Update(current);
            }

            var entity = new CommissionRate
            {
                Type = dto.Type,
                RateValue = dto.RateValue,
                EffectiveDate = dto.EffectiveDate,
                ExpiryDate = null
            };

            await _unitOfWork.CommissionRateRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return new CommissionRateDto
            {
                Id = entity.Id,
                Type = entity.Type,
                CommissionTypeText = _enumService.GetEnumDisplayName<CommissionType>(entity.Type),
                RateValue = entity.RateValue,
                EffectiveDate = entity.EffectiveDate,
                ExpiryDate = entity.ExpiryDate
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

    public async Task UpdateAsync(Guid id, decimal newRateValue)
    {
        try
        {
            var entity = await _unitOfWork.CommissionRateRepository.GetByIdAsync(id, new CancellationToken());
            if (entity == null)
                throw CustomExceptionFactory.CreateNotFoundError("CommissionRate");

            entity.RateValue = newRateValue;
            _unitOfWork.CommissionRateRepository.Update(entity);
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

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _unitOfWork.CommissionRateRepository.GetByIdAsync(id, new CancellationToken());
            if (entity == null)
                throw CustomExceptionFactory.CreateNotFoundError("CommissionRate");

            entity.IsDeleted = true;
            _unitOfWork.CommissionRateRepository.Update(entity);
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
