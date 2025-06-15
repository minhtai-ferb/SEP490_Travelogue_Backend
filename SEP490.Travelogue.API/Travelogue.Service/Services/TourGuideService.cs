using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Const;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourGuideModels;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Services;

public interface ITourGuideService
{
    Task<TourGuideDetailResponse?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken);
    Task<TourGuideDataModel?> AssignToTourGuideAsync(List<string> emails, CancellationToken cancellationToken);
    // Task<TourGuideDataModel> AddTourGuideAsync(TourGuideCreateModel tourGuideCreateModel, CancellationToken cancellationToken);
    Task<TourGuideDataModel?> UpdateTourGuideAsync(Guid id, TourGuideUpdateModel tourGuideUpdateModel, CancellationToken cancellationToken);
    // Task DeleteTourGuideAsync(Guid id, CancellationToken cancellationToken);
    // Task<PagedResult<TourGuideDataModel>> GetPagedTourGuideWithSearchAsync(string? title, string? categoryName, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<List<TourGuideDataModel>> GetAllTourGuidesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // var existingTourGuide = await _unitOfWork.TourGuideRepository.GetAllAsync(cancellationToken);
            var existingTourGuide = await _unitOfWork.TourGuideRepository.ActiveEntities
                .Include(tg => tg.User)
                .ToListAsync(cancellationToken);
            if (existingTourGuide == null || existingTourGuide.Count() == 0)
            {
                return new List<TourGuideDataModel>();
            }

            var dataModel = _mapper.Map<List<TourGuideDataModel>>(existingTourGuide);

            // foreach (var item in dataModel)
            // {
            //     var districtMedia = _unitOfWork.TourGuideMediaRepository.ActiveEntities
            //         .Where(dm => dm.TourGuideId == item.Id)
            //         .OrderByDescending(dm => dm.CreatedTime)
            //         .ToList();

            //     foreach (var media in districtMedia)
            //     {
            //         item.Medias.Add(new MediaResponse
            //         {
            //             MediaUrl = media.MediaUrl,
            //             FileName = media.FileName ?? string.Empty,
            //             FileType = media.FileType,
            //             SizeInBytes = media.SizeInBytes,
            //             CreatedTime = media.CreatedTime,
            //         });
            //     }
            // }

            return dataModel;
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
            ////  _unitOfWork.Dispose();
        }
    }

    public async Task<TourGuideDetailResponse?> GetTourGuideByIdAsync(Guid id, CancellationToken cancellationToken)
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

            var tourGuideDetailResponse = _mapper.Map<TourGuideDetailResponse>(tourGuide);

            await transaction.CommitAsync(cancellationToken);
            return tourGuideDetailResponse;
        }
        catch (CustomException)
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
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
        catch (Exception)
        {
            await _unitOfWork.RollBackAsync();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }
}
