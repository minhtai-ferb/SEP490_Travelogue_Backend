using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CommissionSettingModels;

namespace Travelogue.Service.Services;

public interface ICommissionSettingService
{
    Task<IEnumerable<CommissionSettingDto>> GetAllAsync();
    Task<CommissionSettingDto?> GetCurrentAsync();
    Task<CommissionSettingDto?> GetByDateAsync(DateTime date);
    Task<CommissionSettingDto> UpdateAsync(Guid id, CreateCommissionSettingRequest request);
    Task<CommissionSettingDto> CreateAsync(CreateCommissionSettingRequest request);
}

public sealed class CommissionSettingService : ICommissionSettingService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommissionSettingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CommissionSettingDto>> GetAllAsync()
    {
        try
        {
            var settings = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .ToListAsync();

            return settings.Select(s => new CommissionSettingDto
            {
                Id = s.Id,
                TourGuideCommissionRate = s.TourGuideCommissionRate,
                TourGuideEffectiveDate = s.TourGuideEffectiveDate,
                TourGuideEndDate = s.TourGuideEndDate,
                CraftVillageCommissionRate = s.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = s.CraftVillageEffectiveDate,
                CraftVillageEndDate = s.CraftVillageEndDate,
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
    public async Task<CommissionSettingDto?> GetCurrentAsync()
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CommissionSettingDto
            {
                Id = setting.Id,

                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                TourGuideEffectiveDate = setting.TourGuideEffectiveDate,
                TourGuideEndDate = setting.TourGuideEndDate,

                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = setting.CraftVillageEffectiveDate,
                CraftVillageEndDate = setting.CraftVillageEndDate,
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

    public async Task<CommissionSettingDto?> GetByDateAsync(DateTime date)
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .Where(c =>
                    (c.TourGuideEffectiveDate <= date && (!c.TourGuideEndDate.HasValue || date < c.TourGuideEndDate)) ||
                    (c.CraftVillageEffectiveDate <= date && (!c.CraftVillageEndDate.HasValue || date < c.CraftVillageEndDate))
                )
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CommissionSettingDto
            {
                Id = setting.Id,

                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                TourGuideEffectiveDate = setting.TourGuideEffectiveDate,
                TourGuideEndDate = setting.TourGuideEndDate,

                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = setting.CraftVillageEffectiveDate,
                CraftVillageEndDate = setting.CraftVillageEndDate,
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
    public async Task<CommissionSettingDto> UpdateAsync(Guid id, CreateCommissionSettingRequest request)
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .FirstOrDefaultAsync(x => x.Id == id);
            if (setting == null)
                throw CustomExceptionFactory.CreateNotFoundError($"Commission setting");

            setting.TourGuideCommissionRate = request.TourGuideCommissionRate;
            setting.TourGuideEffectiveDate = request.TourGuideEffectiveDate;
            setting.TourGuideEndDate = request.TourGuideEndDate;

            setting.CraftVillageCommissionRate = request.CraftVillageCommissionRate;
            setting.CraftVillageEffectiveDate = request.CraftVillageEffectiveDate;
            setting.CraftVillageEndDate = request.CraftVillageEndDate;

            _unitOfWork.CommissionSettingsRepository.Update(setting);
            await _unitOfWork.SaveAsync();

            return new CommissionSettingDto
            {
                Id = setting.Id,

                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                TourGuideEffectiveDate = setting.TourGuideEffectiveDate,
                TourGuideEndDate = setting.TourGuideEndDate,

                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = setting.CraftVillageEffectiveDate,
                CraftVillageEndDate = setting.CraftVillageEndDate,
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

    public async Task<CommissionSettingDto> CreateAsync(CreateCommissionSettingRequest request)
    {
        try
        {
            var newSetting = new CommissionSettings
            {
                Id = Guid.NewGuid(),

                // TourGuide
                TourGuideCommissionRate = request.TourGuideCommissionRate,
                TourGuideEffectiveDate = request.TourGuideEffectiveDate,
                TourGuideEndDate = request.TourGuideEndDate,

                // CraftVillage
                CraftVillageCommissionRate = request.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = request.CraftVillageEffectiveDate,
                CraftVillageEndDate = request.CraftVillageEndDate,
            };

            await _unitOfWork.CommissionSettingsRepository.AddAsync(newSetting);
            await _unitOfWork.SaveAsync();

            return new CommissionSettingDto
            {
                Id = newSetting.Id,

                TourGuideCommissionRate = newSetting.TourGuideCommissionRate,
                TourGuideEffectiveDate = newSetting.TourGuideEffectiveDate,
                TourGuideEndDate = newSetting.TourGuideEndDate,

                CraftVillageCommissionRate = newSetting.CraftVillageCommissionRate,
                CraftVillageEffectiveDate = newSetting.CraftVillageEffectiveDate,
                CraftVillageEndDate = newSetting.CraftVillageEndDate,
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

    public async Task<IEnumerable<TourGuideCommissionDto>> GetAllTourGuideAsync()
    {
        try
        {
            var settings = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .ToListAsync();

            return settings.Select(s => new TourGuideCommissionDto
            {
                Id = s.Id,
                TourGuideCommissionRate = s.TourGuideCommissionRate,
                EffectiveDate = s.TourGuideEffectiveDate,
                EndDate = s.TourGuideEndDate
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

    public async Task<IEnumerable<CraftVillageCommissionDto>> GetAllCraftVillageAsync()
    {
        try
        {
            var settings = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.CraftVillageEffectiveDate)
                .ToListAsync();

            return settings.Select(s => new CraftVillageCommissionDto
            {
                Id = s.Id,
                CraftVillageCommissionRate = s.CraftVillageCommissionRate,
                EffectiveDate = s.CraftVillageEffectiveDate,
                EndDate = s.CraftVillageEndDate
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
    public async Task<TourGuideCommissionDto?> GetCurrentTourGuideAsync()
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new TourGuideCommissionDto
            {
                Id = setting.Id,
                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                EffectiveDate = setting.TourGuideEffectiveDate,
                EndDate = setting.TourGuideEndDate
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

    public async Task<CraftVillageCommissionDto?> GetCurrentCraftVillageAsync()
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .OrderByDescending(c => c.CraftVillageEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CraftVillageCommissionDto
            {
                Id = setting.Id,
                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                EffectiveDate = setting.CraftVillageEffectiveDate,
                EndDate = setting.CraftVillageEndDate
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

    public async Task<TourGuideCommissionDto?> GetTourGuideByDateAsync(DateTime date)
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .Where(c => c.TourGuideEffectiveDate <= date && (c.TourGuideEndDate == null || c.TourGuideEndDate > date))
                .OrderByDescending(c => c.TourGuideEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new TourGuideCommissionDto
            {
                Id = setting.Id,
                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                EffectiveDate = setting.TourGuideEffectiveDate,
                EndDate = setting.TourGuideEndDate
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

    public async Task<CraftVillageCommissionDto?> GetCraftVillageByDateAsync(DateTime date)
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .Where(c => c.CraftVillageEffectiveDate <= date && (c.CraftVillageEndDate == null || c.CraftVillageEndDate > date))
                .OrderByDescending(c => c.CraftVillageEffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CraftVillageCommissionDto
            {
                Id = setting.Id,
                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                EffectiveDate = setting.CraftVillageEffectiveDate,
                EndDate = setting.CraftVillageEndDate
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
}
