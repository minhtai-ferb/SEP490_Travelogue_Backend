using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Service.BusinessModels.CommissionSettingModels;

namespace Travelogue.Service.Services;

public interface ICommissionSettingService
{
    Task<IEnumerable<CommissionSettingDto>> GetAllAsync();
    Task<CommissionSettingDto?> GetCurrentAsync();
    Task<CommissionSettingDto?> GetByDateAsync(DateTime date);
    Task<CommissionSettingDto> UpdateAsync(Guid id, UpdateCommissionSettingRequest request);
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
                .OrderByDescending(c => c.EffectiveDate)
                .ToListAsync();

            return settings.Select(s => new CommissionSettingDto
            {
                Id = s.Id,
                TourGuideCommissionRate = s.TourGuideCommissionRate,
                CraftVillageCommissionRate = s.CraftVillageCommissionRate,
                EffectiveDate = s.EffectiveDate
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
                .OrderByDescending(c => c.EffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CommissionSettingDto
            {
                Id = setting.Id,
                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                EffectiveDate = setting.EffectiveDate
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
                .Where(c => c.EffectiveDate <= date)
                .OrderByDescending(c => c.EffectiveDate)
                .FirstOrDefaultAsync();

            return setting == null ? null : new CommissionSettingDto
            {
                Id = setting.Id,
                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                EffectiveDate = setting.EffectiveDate
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

    public async Task<CommissionSettingDto> UpdateAsync(Guid id, UpdateCommissionSettingRequest request)
    {
        try
        {
            var setting = await _unitOfWork.CommissionSettingsRepository
                .ActiveEntities
                .FirstOrDefaultAsync(x => x.Id == id);
            if (setting == null)
                throw CustomExceptionFactory.CreateNotFoundError($"Commission setting");

            setting.TourGuideCommissionRate = request.TourGuideCommissionRate;
            setting.CraftVillageCommissionRate = request.CraftVillageCommissionRate;
            setting.EffectiveDate = request.EffectiveDate;

            _unitOfWork.CommissionSettingsRepository.Update(setting);
            await _unitOfWork.SaveAsync();

            return new CommissionSettingDto
            {
                Id = setting.Id,
                TourGuideCommissionRate = setting.TourGuideCommissionRate,
                CraftVillageCommissionRate = setting.CraftVillageCommissionRate,
                EffectiveDate = setting.EffectiveDate
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
