using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IRestaurantMediaRepository : IGenericRepository<RestaurantMedia>
{
    Task<RestaurantMedia> GetFirstByRestaurantIdAsync(Guid restaurantId);
}
public sealed class RestaurantMediaRepository : GenericRepository<RestaurantMedia>, IRestaurantMediaRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<RestaurantMedia?> GetFirstByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Set<RestaurantMedia>()
            .Where(em => em.RestaurantId == restaurantId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }
}
