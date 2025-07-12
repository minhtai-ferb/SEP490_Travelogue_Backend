using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourGuideService
{
    Task<TourGuideDataModel?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken);
    Task<TourGuideDataModel?> AssignToTourGuideAsync(List<string> emails, CancellationToken cancellationToken);
    // Task<TourGuideDataModel> AddTourGuideAsync(TourGuideCreateModel tourGuideCreateModel, CancellationToken cancellationToken);
    Task<TourGuideDataModel?> UpdateTourGuideAsync(Guid id, TourGuideUpdateModel tourGuideUpdateModel, CancellationToken cancellationToken);
    // Task DeleteTourGuideAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TourGuideDataModel>> GetPagedTourGuideWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<TourGuideDataModel>> GetTourGuidesByFilterAsync(TourGuideFilterRequest request, CancellationToken cancellationToken);
}

public class TourGuideService : ITourGuideService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly ITimeService _timeService;

    public TourGuideService(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService, ITimeService timeService)
    {
        _unitOfWork = unitOfWork;
        // ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper;
        _userContextService = userContextService;
        _timeService = timeService;
    }

    public async Task<TourGuideDataModel?> AssignToTourGuideAsync(List<string> emails, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var currentUserId = _userContextService.GetCurrentUserId();
            var currentTime = _timeService.SystemTimeNow;

            var users = await _unitOfWork.UserRepository.ActiveEntities
                .Where(u => emails.Contains(u.Email))
                .ToListAsync(cancellationToken);

            if (users == null || users.Count == 0)
            {
                throw CustomExceptionFactory.CreateNotFoundError("No users found with the provided emails.");
            }

            var roleTourGuide = await _unitOfWork.RoleRepository.GetByNameAsync(AppRole.TOUR_GUIDE);

            if (roleTourGuide == null)
            {

                roleTourGuide = new Role(AppRole.TOUR_GUIDE, true);
                // Thêm mới role tour guide
                roleTourGuide.Description = "Tour Guide Role";
                roleTourGuide.CreatedBy = currentUserId;
                roleTourGuide.LastUpdatedBy = currentUserId;
                roleTourGuide.CreatedTime = currentTime;
                roleTourGuide.LastUpdatedTime = currentTime;

                await _unitOfWork.RoleRepository.AddAsync(roleTourGuide);
                await _unitOfWork.SaveAsync();
            }

            var tourGuides = new List<TourGuide>();
            foreach (var user in users)
            {
                var existingTourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(user.Id);
                if (existingTourGuide != null)
                {
                    continue; // User is already a tour guide
                }

                var newTourGuide = new TourGuide
                {
                    UserId = user.Id,
                    User = user
                };
                tourGuides.Add(newTourGuide);

                // Check if the user already has the tour guide role
                // If not, create a new UserRole for the tour guide
                var existingUserRole = await _unitOfWork.UserRoleRepository.ActiveEntities
                    .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == roleTourGuide.Id, cancellationToken);

                if (existingUserRole == null)
                {
                    var newUserRoles = new UserRole
                    {
                        RoleId = roleTourGuide.Id,
                        UserId = user.Id,
                        CreatedBy = currentUserId,
                        LastUpdatedBy = currentUserId,
                        CreatedTime = currentTime,
                        LastUpdatedTime = currentTime
                    };
                    await _unitOfWork.UserRoleRepository.AddAsync(newUserRoles);
                }
            }

            if (tourGuides.Count > 0)
            {
                await _unitOfWork.TourGuideRepository.AddRangeAsync(tourGuides);
                await _unitOfWork.SaveAsync();
            }

            await transaction.CommitAsync(cancellationToken);
            return _mapper.Map<TourGuideDataModel>(tourGuides.FirstOrDefault());
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

    public async Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var existingTourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .ToListAsync(cancellationToken);
            if (existingTourGuide == null || existingTourGuide.Count == 0)
            {
                return new List<TourGuideDataModel>();
            }

            var dataModel = _mapper.Map<List<TourGuideDataModel>>(existingTourGuide);

            return dataModel;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<List<TourGuideDataModel>> GetTourGuidesByFilterAsync(TourGuideFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var existingTourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .Include(tg => tg.TourGuideSchedules)
                .ToListAsync(cancellationToken);
            if (existingTourGuide == null || existingTourGuide.Count() == 0)
            {
                return new List<TourGuideDataModel>();
            }

            existingTourGuide = existingTourGuide
                .Where(tg => string.IsNullOrEmpty(request.FullName) || tg.User.FullName.ToLower().Contains(request.FullName.ToLower()))
                .Where(tg => !request.MinRating.HasValue || tg.Rating >= request.MinRating.Value)
                .Where(tg => !request.MaxRating.HasValue || tg.Rating <= request.MaxRating.Value)
                .Where(tg => !request.MinPrice.HasValue || tg.Price >= request.MinPrice.Value)
                .Where(tg => !request.MaxPrice.HasValue || tg.Price <= request.MaxPrice.Value)
                .Where(tg => !request.Gender.HasValue || tg.User.Sex == request.Gender.Value)
                .Where(tg => !request.StartDate.HasValue || !request.EndDate.HasValue ||
                     !tg.TourGuideSchedules.Any(s => s.Date >= request.StartDate.Value && s.Date <= request.EndDate.Value))
                .ToList();

            var result = _mapper.Map<List<TourGuideDataModel>>(existingTourGuide);

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
        finally
        {
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<PagedResult<TourGuideDataModel>> GetPagedTourGuideWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var tourGuide = _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .Where(tg => string.IsNullOrEmpty(name) || tg.User.FullName.ToLower().Contains(name.ToLower()));

            if (tourGuide == null)
            {
                return new PagedResult<TourGuideDataModel>
                {
                    Items = new List<TourGuideDataModel>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var tourGuideDetailResponse = _mapper.Map<TourGuideDataModel>(tourGuide);

            var totalCount = await tourGuide.CountAsync(cancellationToken);

            return new PagedResult<TourGuideDataModel>
            {
                Items = await tourGuide
                    .OrderBy(tg => tg.User.FullName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(tg => _mapper.Map<TourGuideDataModel>(tg))
                    .ToListAsync(cancellationToken),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
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

    public async Task<TourGuideDataModel?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var tourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .FirstOrDefaultAsync(tg => tg.Id == id, cancellationToken);

            if (tourGuide == null)
            {
                return null; // Hoặc ném một ngoại lệ nếu cần
            }

            var tourGuideDetailResponse = _mapper.Map<TourGuideDataModel>(tourGuide);

            await transaction.CommitAsync(cancellationToken);
            return tourGuideDetailResponse;
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

    public async Task<TourGuideDataModel?> UpdateTourGuideAsync(Guid userId, TourGuideUpdateModel tourGuideUpdateModel, CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken) ?? throw new Exception("User not found");

            // Cập nhật thông tin user
            user.Email = tourGuideUpdateModel.Email;
            user.PhoneNumber = tourGuideUpdateModel.PhoneNumber;
            user.FullName = tourGuideUpdateModel.FullName;
            user.Sex = tourGuideUpdateModel.Sex;
            user.Address = tourGuideUpdateModel.Address;

            // Kiểm tra xem có phải tour guide không
            var tourGuide = await _unitOfWork.TourGuideRepository.GetByUserIdAsync(userId) ?? throw new Exception("This user is not a tour guide");

            // Cập nhật thông tin tour guide
            tourGuide.Introduction = tourGuideUpdateModel.Introduction;

            // Lưu thay đổi
            await _unitOfWork.UserRepository.UpdateAsync(user);
            _unitOfWork.TourGuideRepository.Update(tourGuide);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync(cancellationToken);
            var updatedTourGuide = _mapper.Map<TourGuideDataModel>(tourGuide);
            return updatedTourGuide;
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
