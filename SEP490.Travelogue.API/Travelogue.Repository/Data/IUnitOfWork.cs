using Microsoft.EntityFrameworkCore.Storage;
using Travelogue.Repository.Bases.BaseEntitys;
using Travelogue.Repository.Repositories;

namespace Travelogue.Repository.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    IRoleDistrictRepository RoleDistrictRepository { get; }
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }
    ILocationRepository LocationRepository { get; }
    IFavoriteLocationRepository FavoriteLocationRepository { get; }
    ILocationHotelSuggestionRepository LocationHotelSuggestionRepository { get; }
    ILocationRestaurantSuggestionRepository LocationRestaurantSuggestionRepository { get; }
    ITypeLocationRepository TypeLocationRepository { get; }
    IExperienceRepository ExperienceRepository { get; }
    IEventRepository EventRepository { get; }
    ITypeEventRepository TypeEventRepository { get; }
    ITypeExperienceRepository TypeExperienceRepository { get; }
    IHotelRepository HotelRepository { get; }
    IHotelMediaRepository HotelMediaRepository { get; }
    IRestaurantRepository RestaurantRepository { get; }
    IRestaurantMediaRepository RestaurantMediaRepository { get; }
    IMediaRepository MediaRepository { get; }
    IDistrictRepository DistrictRepository { get; }
    IDistrictMediaRepository DistrictMediaRepository { get; }
    ILocationMediaRepository LocationMediaRepository { get; }
    IExperienceMediaRepository ExperienceMediaRepository { get; }
    IEventMediaRepository EventMediaRepository { get; }
    INewsRepository NewsRepository { get; }
    INewsMediaRepository NewsMediaRepository { get; }
    INewsCategoryRepository NewsCategoryRepository { get; }

    IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity;
    void Save();
    Task SaveAsync();
    void BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task RollBackAsync();
    ValueTask DisposeAsync();
    void CommitTransaction();
    void RollBack();
}
