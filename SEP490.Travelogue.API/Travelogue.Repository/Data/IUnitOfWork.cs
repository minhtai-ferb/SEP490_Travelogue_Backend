using Microsoft.EntityFrameworkCore.Storage;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Repositories;

namespace Travelogue.Repository.Data;

public interface IUnitOfWork //: IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }
    ILocationRepository LocationRepository { get; }
    IFavoriteLocationRepository FavoriteLocationRepository { get; }
    ICraftVillageRepository CraftVillageRepository { get; }
    ICuisineRepository CuisineRepository { get; }
    IDistrictRepository DistrictRepository { get; }
    IDistrictMediaRepository DistrictMediaRepository { get; }
    ILocationMediaRepository LocationMediaRepository { get; }
    INewsRepository NewsRepository { get; }
    INewsMediaRepository NewsMediaRepository { get; }
    ITripPlanRepository TripPlanRepository { get; }
    ITripPlanLocationRepository TripPlanLocationRepository { get; }
    ITourGuideRepository TourGuideRepository { get; }
    ITourRepository TourRepository { get; }
    ITourPlanLocationRepository TourPlanLocationRepository { get; }
    IBookingRepository BookingRepository { get; }
    IHistoricalLocationRepository HistoricalLocationRepository { get; }
    ITourGuideScheduleRepository TourGuideScheduleRepository { get; }
    ITourScheduleRepository TourScheduleRepository { get; }
    IWorkshopRepository WorkshopRepository { get; }
    IWorkshopActivityRepository WorkshopActivityRepository { get; }
    IWorkshopScheduleRepository WorkshopScheduleRepository { get; }
    ITourGuideRequestRepository TourGuideRequestRepository { get; }
    ICraftVillageRequestRepository CraftVillageRequestRepository { get; }
    IWorkshopMediaRepository WorkshopMediaRepository { get; }
    ITourMediaRepository TourMediaRepository { get; }
    ICertificationRepository CertificationRepository { get; }
    IPromotionRepository PromotionRepository { get; }
    ITransactionEntryRepository TransactionEntryRepository { get; }
    IBookingPriceRequestRepository BookingPriceRequestRepository { get; }
    IRejectionRequestRepository RejectionRequestRepository { get; }
    IReviewRepository ReviewRepository { get; }

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
