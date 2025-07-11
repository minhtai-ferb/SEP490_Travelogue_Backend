using Microsoft.EntityFrameworkCore.Storage;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Repositories;

namespace Travelogue.Repository.Data;

public interface IUnitOfWork //: IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    IRoleDistrictRepository RoleDistrictRepository { get; }
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }
    ILocationRepository LocationRepository { get; }
    IFavoriteLocationRepository FavoriteLocationRepository { get; }
    ILocationCraftVillageSuggestionRepository LocationCraftVillageSuggestionRepository { get; }
    ILocationCuisineSuggestionRepository LocationCuisineSuggestionRepository { get; }
    IExperienceRepository ExperienceRepository { get; }
    IEventRepository EventRepository { get; }
    ITypeEventRepository TypeEventRepository { get; }
    ITypeExperienceRepository TypeExperienceRepository { get; }
    ICraftVillageRepository CraftVillageRepository { get; }
    ICuisineRepository CuisineRepository { get; }
    IMediaRepository MediaRepository { get; }
    IDistrictRepository DistrictRepository { get; }
    IDistrictMediaRepository DistrictMediaRepository { get; }
    ILocationMediaRepository LocationMediaRepository { get; }
    IExperienceMediaRepository ExperienceMediaRepository { get; }
    IEventMediaRepository EventMediaRepository { get; }
    INewsRepository NewsRepository { get; }
    INewsMediaRepository NewsMediaRepository { get; }
    INewsCategoryRepository NewsCategoryRepository { get; }
    ITripPlanRepository TripPlanRepository { get; }
    ITripPlanVersionRepository TripPlanVersionRepository { get; }
    ITripPlanLocationRepository TripPlanLocationRepository { get; }
    ITourGuideRepository TourGuideRepository { get; }
    ITourRepository TourRepository { get; }
    ITourPlanLocationRepository TourPlanLocationRepository { get; }
    ITourTypeRepository TourTypeRepository { get; }
    IBookingRepository BookingRepository { get; }
    ITripPlanExchangeRepository TripPlanExchangeRepository { get; }
    ITripPlanExchangeSessionRepository TripPlanExchangeSessionRepository { get; }
    IHistoricalLocationRepository HistoricalLocationRepository { get; }
    ITourGuideScheduleRepository TourGuideScheduleRepository { get; }
    ITourScheduleRepository TourScheduleRepository { get; }
    IWorkshopRepository WorkshopRepository { get; }
    IWorkshopActivityRepository WorkshopActivityRepository { get; }
    IWorkshopScheduleRepository WorkshopScheduleRepository { get; }
    ITourGuideMappingRepository TourGuideMappingRepository { get; }

    IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity;
    void Save();
    Task SaveAsync();
    void BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task RollBackAsync();
    //ValueTask DisposeAsync();
    void CommitTransaction();
    void RollBack();
}
