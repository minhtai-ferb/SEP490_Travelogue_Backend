using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IRestaurantRepository : IGenericRepository<Restaurant>
{
    Task<Restaurant?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<PagedResult<Restaurant>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<Restaurant?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var restaurants = _context.Restaurants.FirstOrDefaultAsync(a => a.Name.Contains(name), cancellationToken);
            return restaurants;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<PagedResult<Restaurant>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Restaurant>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
