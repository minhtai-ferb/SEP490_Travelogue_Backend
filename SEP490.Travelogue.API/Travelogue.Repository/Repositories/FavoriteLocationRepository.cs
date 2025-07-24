using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IFavoriteLocationRepository : IGenericRepository<FavoriteLocation>
{
}
public sealed class FavoriteLocationRepository : GenericRepository<FavoriteLocation>, IFavoriteLocationRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
