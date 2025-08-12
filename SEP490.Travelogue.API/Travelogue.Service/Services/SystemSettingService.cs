using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.SystemSettingModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ISystemSettingService
{
    Task<SystemSettingDto> UpdateAsync(SystemSettingUpdateDto dto);
    Task<List<SystemSettingDto>> GetAllAsync();
}

public class SystemSettingService : ISystemSettingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumService _enumService;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public SystemSettingService(IUnitOfWork unitOfWork, IEnumService enumService, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        _enumService = enumService;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task<SystemSettingDto> UpdateAsync(SystemSettingUpdateDto dto)
    {
        try
        {
            var isAllowed = _userContextService.HasAnyRole(AppRole.ADMIN);
            if (!isAllowed)
                throw CustomExceptionFactory.CreateForbiddenError();

            var setting = await _unitOfWork.SystemSettingRepository
                .ActiveEntities
                .FirstOrDefaultAsync(s => s.Key == dto.Key)
                ?? throw CustomExceptionFactory.CreateNotFoundError("SystemSetting");

            setting.Value = dto.Value;
            // setting.Unit = dto.Unit;
            setting.LastUpdatedTime = DateTimeOffset.UtcNow;

            _unitOfWork.SystemSettingRepository.Update(setting);
            await _unitOfWork.SaveAsync();

            return new SystemSettingDto
            {
                Key = setting.Key,
                KeyText = _enumService.GetEnumDisplayName<SystemSettingKey>(setting.Key) ?? string.Empty,
                Value = setting.Value,
                Unit = setting.Unit,
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

    public async Task<List<SystemSettingDto>> GetAllAsync()
    {
        try
        {
            var isAllowed = _userContextService.HasAnyRole(AppRole.ADMIN);
            if (!isAllowed)
                throw CustomExceptionFactory.CreateForbiddenError();

            var entities = await _unitOfWork.SystemSettingRepository
                .ActiveEntities
                .Select(s => new
                {
                    s.Key,
                    s.Value,
                    s.Unit,
                })
                .ToListAsync();

            var settings = entities
                .Select(s => new SystemSettingDto
                {
                    Key = s.Key,
                    KeyText = _enumService.GetEnumDisplayName<SystemSettingKey>(s.Key) ?? string.Empty,
                    Value = s.Value,
                    Unit = s.Unit,
                })
                .ToList();

            return settings;
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
