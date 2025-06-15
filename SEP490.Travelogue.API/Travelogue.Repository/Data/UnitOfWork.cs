using Microsoft.EntityFrameworkCore.Storage;
using Travelogue.Repository.Bases.BaseEntities;
using Travelogue.Repository.Repositories;

namespace Travelogue.Repository.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext = null!;
    private IDbContextTransaction? _transaction = null!;
    private bool _disposed = false;

    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public IUserRoleRepository UserRoleRepository { get; }
    public IRoleDistrictRepository RoleDistrictRepository { get; }
    public IPasswordResetTokenRepository PasswordResetTokenRepository { get; }
    public ILocationRepository LocationRepository { get; }
    public IFavoriteLocationRepository FavoriteLocationRepository { get; }
    public ILocationCuisineSuggestionRepository LocationCuisineSuggestionRepository { get; }
    public ILocationCraftVillageSuggestionRepository LocationCraftVillageSuggestionRepository { get; }
    public ITypeLocationRepository TypeLocationRepository { get; }
    public IExperienceRepository ExperienceRepository { get; }
    public IEventRepository EventRepository { get; }
    public ITypeEventRepository TypeEventRepository { get; }
    public ITypeExperienceRepository TypeExperienceRepository { get; }
    public IHotelRepository HotelRepository { get; }
    public IHotelMediaRepository HotelMediaRepository { get; }
    public ILocationHotelSuggestionRepository LocationHotelSuggestionRepository { get; }
    public ICraftVillageRepository CraftVillageRepository { get; }
    public ICraftVillageMediaRepository CraftVillageMediaRepository { get; }
    public ICuisineRepository CuisineRepository { get; }
    public ICuisineMediaRepository CuisineMediaRepository { get; }
    public IMediaRepository MediaRepository { get; }
    public IDistrictRepository DistrictRepository { get; }
    public INewsRepository NewsRepository { get; }
    public INewsMediaRepository NewsMediaRepository { get; }
    public IDistrictMediaRepository DistrictMediaRepository { get; }
    public IEventMediaRepository EventMediaRepository { get; }
    public IExperienceMediaRepository ExperienceMediaRepository { get; }
    public ILocationMediaRepository LocationMediaRepository { get; }
    public INewsCategoryRepository NewsCategoryRepository { get; }
    public ITripPlanRepository TripPlanRepository { get; }
    public ITripPlanVersionRepository TripPlanVersionRepository { get; }
    public ITripPlanLocationRepository TripPlanLocationRepository { get; }
    public ITripPlanCraftVillageRepository TripPlanCraftVillageRepository { get; }
    public ITripPlanCuisineRepository TripPlanCuisineRepository { get; }
    public ITourGuideRepository TourGuideRepository { get; }
    public ITourRepository TourRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public ITourGuideBookingRequestRepository TourGuideBookingRequestRepository { get; }
    public ITripPlanExchangeSessionRepository TripPlanExchangeSessionRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        UserRepository = new UserRepository(_dbContext);
        RoleRepository = new RoleRepository(_dbContext);
        UserRoleRepository = new UserRoleRepository(_dbContext);
        RoleDistrictRepository = new RoleDistrictRepository(_dbContext);
        PasswordResetTokenRepository = new PasswordResetTokenRepository(_dbContext);
        LocationRepository = new LocationRepository(_dbContext);
        FavoriteLocationRepository = new FavoriteLocationRepository(_dbContext);
        LocationCuisineSuggestionRepository = new LocationCuisineSuggestionRepository(_dbContext);
        LocationCraftVillageSuggestionRepository = new LocationCraftVillageSuggestionRepository(_dbContext);
        TypeLocationRepository = new TypeLocationRepository(_dbContext);
        ExperienceRepository = new ExperienceRepository(_dbContext);
        EventRepository = new EventRepository(_dbContext);
        TypeEventRepository = new TypeEventRepository(_dbContext);
        TypeExperienceRepository = new TypeExperienceRepository(_dbContext);
        HotelRepository = new HotelRepository(_dbContext);
        HotelMediaRepository = new HotelMediaRepository(_dbContext);
        LocationHotelSuggestionRepository = new LocationHotelSuggestionRepository(_dbContext);
        CraftVillageRepository = new CraftVillageRepository(_dbContext);
        CraftVillageMediaRepository = new CraftVillageMediaRepository(_dbContext);
        CuisineRepository = new CuisineRepository(_dbContext);
        CuisineMediaRepository = new CuisineMediaRepository(_dbContext);
        MediaRepository = new MediaRepository(_dbContext);
        DistrictRepository = new DistrictRepository(_dbContext);
        NewsRepository = new NewsRepository(_dbContext);
        NewsMediaRepository = new NewsMediaRepository(_dbContext);
        DistrictMediaRepository = new DistrictMediaRepository(_dbContext);
        EventMediaRepository = new EventMediaRepository(_dbContext);
        ExperienceMediaRepository = new ExperienceMediaRepository(_dbContext);
        LocationMediaRepository = new LocationMediaRepository(_dbContext);
        NewsCategoryRepository = new NewsCategoryRepository(_dbContext);
        TripPlanRepository = new TripPlanRepository(_dbContext);
        TripPlanVersionRepository = new TripPlanVersionRepository(_dbContext);
        TripPlanLocationRepository = new TripPlanLocationRepository(_dbContext);
        TripPlanCraftVillageRepository = new TripPlanCraftVillageRepository(_dbContext);
        TripPlanCuisineRepository = new TripPlanCuisineRepository(_dbContext);
        TourGuideRepository = new TourGuideRepository(_dbContext);
        TourRepository = new TourRepository(_dbContext);
        OrderRepository = new OrderRepository(_dbContext);
        TourGuideBookingRequestRepository = new TourGuideBookingRequestRepository(_dbContext);
        TripPlanExchangeSessionRepository = new TripPlanExchangeSessionRepository(_dbContext);
    }

    public IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity
    {
        return new GenericRepository<T>(_dbContext);
    }
    public void Save()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            throw new InvalidOperationException("Failed to save changes to the database.", ex);
        }
    }

    public void BeginTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        try
        {
            _dbContext.SaveChanges();
            _transaction?.Commit();
        }
        catch
        {
            RollBack();
            throw;
        }
    }

    public void RollBack()
    {
        _transaction?.Rollback();
    }

    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!_disposed)
    //    {
    //        if (disposing)
    //        {
    //            _dbContext.Dispose();
    //            _transaction?.Dispose();
    //        }
    //    }
    //    _disposed = true;
    //}

    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        try
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
            return _transaction;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error beginning transaction: {ex.Message}");
            throw new InvalidOperationException("Failed to begin database transaction.", ex);
        }
    }


    public async Task CommitTransactionAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollBackAsync();
            throw;
        }
    }

    public async Task RollBackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            //await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #region Repositories
    // private IUserRepository _userRepository = null!;
    // private IRoleRepository _roleRepository = null!;
    // private IUserRoleRepository _userRoleRepository = null!;
    // private IRoleDistrictRepository _roleDistrictRepository = null!;
    // private IPasswordResetTokenRepository _passwordResetTokenRepository = null!;
    // private ILocationRepository _locationRepository = null!;
    // private IFavoriteLocationRepository _favoriteLocationRepository = null!;
    // private ILocationCuisineSuggestionRepository _locationCuisineSuggestionRepository = null!;
    // private ILocationCraftVillageSuggestionRepository _locationCraftVillageSuggestionRepository = null!;
    // private ITypeLocationRepository _typeLocationRepository = null!;
    // private IExperienceRepository _experienceRepository = null!;
    // private IEventRepository _activityRepository = null!;
    // private ITypeEventRepository _typeEventRepository = null!;
    // private ITypeExperienceRepository _typeExperienceRepository = null!;
    // private IHotelRepository _hotelRepository = null!;
    // private IHotelMediaRepository _hotelMediaRepository = null!;
    // private ILocationHotelSuggestionRepository _locationHotelSuggestionRepository = null!;
    // private ICraftVillageRepository _craftVillageRepository = null!;
    // private ICraftVillageMediaRepository _craftVillageMediaRepository = null!;
    // private ICuisineRepository _cuisineRepository = null!;
    // private ICuisineMediaRepository _cuisineMediaRepository = null!;
    // private IMediaRepository _mediaRepository = null!;
    // private IDistrictRepository _districtRepository = null!;
    // private INewsRepository _newsRepository = null!;
    // private INewsMediaRepository _newsMediaRepository = null!;
    // private IDistrictMediaRepository _districtMediaRepository = null!;
    // private IEventMediaRepository _eventMediaRepository = null!;
    // private IExperienceMediaRepository _experienceMediaRepository = null!;
    // private ILocationMediaRepository _locationMediaRepository = null!;
    // private INewsCategoryRepository _newsCategoryRepository = null!;
    // private ITripPlanRepository _tripPlanRepository = null!;
    // private ITripPlanVersionRepository _tripPlanVersionRepository = null!;
    // private ITripPlanLocationRepository _tripPlanLocationRepository = null!;
    // private ITripPlanCraftVillageRepository _tripPlanCraftVillageRepository = null!;
    // private ITripPlanCuisineRepository _tripPlanCuisineRepository = null!;
    // private ITourGuideRepository _tourGuideRepository = null!;
    // private ITourRepository _tourRepository = null!;
    // private IOrderRepository _orderRepository = null!;
    // private ITourGuideBookingRequestRepository _tourGuideBookingRequestRepository = null!;
    // private ITripPlanExchangeSessionRepository _tripPlanExchangeSessionRepository = null!;

    // public ITripPlanExchangeSessionRepository TripPlanExchangeSessionRepository
    // {
    //     get { return _tripPlanExchangeSessionRepository ??= new TripPlanExchangeSessionRepository(_dbContext); }
    // }

    // public ITourGuideBookingRequestRepository TourGuideBookingRequestRepository
    // {
    //     get { return _tourGuideBookingRequestRepository ??= new TourGuideBookingRequestRepository(_dbContext); }
    // }

    // public IOrderRepository OrderRepository
    // {
    //     get { return _orderRepository ??= new OrderRepository(_dbContext); }
    // }

    // public ITourRepository TourRepository
    // {
    //     get { return _tourRepository ??= new TourRepository(_dbContext); }
    // }

    // public ITourGuideRepository TourGuideRepository
    // {
    //     get { return _tourGuideRepository ??= new TourGuideRepository(_dbContext); }
    // }

    // public ITripPlanVersionRepository TripPlanVersionRepository
    // {
    //     get { return _tripPlanVersionRepository ??= new TripPlanVersionRepository(_dbContext); }
    // }

    // public ITripPlanCraftVillageRepository TripPlanCraftVillageRepository
    // {
    //     get { return _tripPlanCraftVillageRepository ??= new TripPlanCraftVillageRepository(_dbContext); }
    // }

    // public ITripPlanCuisineRepository TripPlanCuisineRepository
    // {
    //     get { return _tripPlanCuisineRepository ??= new TripPlanCuisineRepository(_dbContext); }
    // }

    // public ITripPlanLocationRepository TripPlanLocationRepository
    // {
    //     get { return _tripPlanLocationRepository ??= new TripPlanLocationRepository(_dbContext); }
    // }

    // public ITripPlanRepository TripPlanRepository
    // {
    //     get { return _tripPlanRepository ??= new TripPlanRepository(_dbContext); }
    // }

    // public INewsCategoryRepository NewsCategoryRepository
    // {
    //     get { return _newsCategoryRepository ??= new NewsCategoryRepository(_dbContext); }
    // }

    // public INewsMediaRepository NewsMediaRepository
    // {
    //     get { return _newsMediaRepository ??= new NewsMediaRepository(_dbContext); }
    // }

    // public IFavoriteLocationRepository FavoriteLocationRepository
    // {
    //     get { return _favoriteLocationRepository ??= new FavoriteLocationRepository(_dbContext); }
    // }
    // public IHotelMediaRepository HotelMediaRepository
    // {
    //     get { return _hotelMediaRepository ??= new HotelMediaRepository(_dbContext); }
    // }

    // public ICraftVillageMediaRepository CraftVillageMediaRepository
    // {
    //     get { return _craftVillageMediaRepository ??= new CraftVillageMediaRepository(_dbContext); }
    // }

    // public ICuisineMediaRepository CuisineMediaRepository
    // {
    //     get { return _cuisineMediaRepository ??= new CuisineMediaRepository(_dbContext); }
    // }

    // public IDistrictMediaRepository DistrictMediaRepository
    // {
    //     get { return _districtMediaRepository ??= new DistrictMediaRepository(_dbContext); }
    // }

    // public IExperienceRepository ExperienceRepository
    // {
    //     get { return _experienceRepository ??= new ExperienceRepository(_dbContext); }
    // }

    // public IEventRepository EventRepository
    // {
    //     get { return _activityRepository ??= new EventRepository(_dbContext); }
    // }

    // public ITypeEventRepository TypeEventRepository
    // {
    //     get { return _typeEventRepository ??= new TypeEventRepository(_dbContext); }
    // }
    // public ITypeExperienceRepository TypeExperienceRepository
    // {
    //     get { return _typeExperienceRepository ??= new TypeExperienceRepository(_dbContext); }
    // }
    // public IHotelRepository HotelRepository
    // {
    //     get { return _hotelRepository ??= new HotelRepository(_dbContext); }
    // }
    // public ICraftVillageRepository CraftVillageRepository
    // {
    //     get { return _craftVillageRepository ??= new CraftVillageRepository(_dbContext); }
    // }
    // public ICuisineRepository CuisineRepository
    // {
    //     get { return _cuisineRepository ??= new CuisineRepository(_dbContext); }
    // }
    // public IMediaRepository MediaRepository
    // {
    //     get { return _mediaRepository ??= new MediaRepository(_dbContext); }
    // }

    // public IDistrictRepository DistrictRepository
    // {
    //     get { return _districtRepository ??= new DistrictRepository(_dbContext); }
    // }

    // public INewsRepository NewsRepository
    // {
    //     get { return _newsRepository ??= new NewsRepository(_dbContext); }
    // }

    // public ITypeLocationRepository TypeLocationRepository
    // {
    //     get { return _typeLocationRepository ??= new TypeLocationRepository(_dbContext); }
    // }

    // public ILocationRepository LocationRepository
    // {
    //     get { return _locationRepository ??= new LocationRepository(_dbContext); }
    // }

    // public ILocationHotelSuggestionRepository LocationHotelSuggestionRepository
    // {
    //     get { return _locationHotelSuggestionRepository ??= new LocationHotelSuggestionRepository(_dbContext); }
    // }

    // public ILocationCraftVillageSuggestionRepository LocationCraftVillageSuggestionRepository
    // {
    //     get { return _locationCraftVillageSuggestionRepository ??= new LocationCraftVillageSuggestionRepository(_dbContext); }
    // }

    // public ILocationCuisineSuggestionRepository LocationCuisineSuggestionRepository
    // {
    //     get { return _locationCuisineSuggestionRepository ??= new LocationCuisineSuggestionRepository(_dbContext); }
    // }

    // public IPasswordResetTokenRepository PasswordResetTokenRepository
    // {
    //     get { return _passwordResetTokenRepository ??= new PasswordResetTokenRepository(_dbContext); }
    // }

    // public IRoleDistrictRepository RoleDistrictRepository
    // {
    //     get { return _roleDistrictRepository ??= new RoleDistrictRepository(_dbContext); }
    // }

    // public IUserRoleRepository UserRoleRepository
    // {
    //     get { return _userRoleRepository ??= new UserRoleRepository(_dbContext); }
    // }

    // public IRoleRepository RoleRepository
    // {
    //     get { return _roleRepository ??= new RoleRepository(_dbContext); }
    // }

    // public IUserRepository UserRepository
    // {
    //     get { return _userRepository ??= new UserRepository(_dbContext); }
    // }

    // public ILocationMediaRepository LocationMediaRepository
    // {
    //     get { return _locationMediaRepository ??= new LocationMediaRepository(_dbContext); }
    // }

    // public IExperienceMediaRepository ExperienceMediaRepository
    // {
    //     get { return _experienceMediaRepository ??= new ExperienceMediaRepository(_dbContext); }
    // }

    // public IEventMediaRepository EventMediaRepository
    // {
    //     get { return _eventMediaRepository ??= new EventMediaRepository(_dbContext); }
    // }

    #endregion Repositories



    //public async ValueTask DisposeAsync()
    //{
    //    if (!_disposed)
    //    {
    //        if (_transaction != null)
    //        {
    //            await _transaction.DisposeAsync();
    //        }
    //        await _dbContext.DisposeAsync();
    //        _disposed = true;
    //    }
    //}
}
