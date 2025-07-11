using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICuisineRepository : IGenericRepository<Cuisine>
{
    Task<List<Cuisine>> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<PagedResult<Cuisine>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Cuisine?> GetByLocationId(Guid locationId, CancellationToken cancellationToken);
}

public sealed class CuisineRepository : GenericRepository<Cuisine>, ICuisineRepository
{
    private readonly ApplicationDbContext _context;

    public CuisineRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<Cuisine?> GetByLocationId(Guid locationId, CancellationToken cancellationToken)
    {
        try
        {
            var cuisine = await _context.Cuisines.Include(h => h.Location)
                .FirstOrDefaultAsync(a => a.LocationId == locationId, cancellationToken);
            return cuisine;
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

    public Task<List<Cuisine>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var cuisines = _context.Cuisines
                .Include(c => c.Location)
                .Where(c => c.Location.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync(cancellationToken);
            return cuisines;
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

    public async Task<PagedResult<Cuisine>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than zero.");
            }

            var query = ActiveEntities.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Include(q => q.Location).Where(a => a.Location.Name.ToLower().Contains(name.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Cuisine>
            {
                Items = items,
                TotalCount = totalItems,
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
}
