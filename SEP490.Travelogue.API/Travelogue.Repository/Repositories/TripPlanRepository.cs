using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanRepository : IGenericRepository<TripPlan>
{
    Task<PagedResult<TripPlan>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    Task<PagedResult<TripPlan>> GetPageWithSearchAsync(
        string? title,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TripPlan?> GetWithIncludeAsync(Guid tripPlanId, Func<IQueryable<TripPlan>, IQueryable<TripPlan>> include);
}

public sealed class TripPlanRepository : GenericRepository<TripPlan>, ITripPlanRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<TripPlan?> GetWithIncludeAsync(Guid tripPlanId, Func<IQueryable<TripPlan>, IQueryable<TripPlan>> include)
    {
        if (tripPlanId == Guid.Empty)
        {
            throw new ArgumentException("Trip plan ID cannot be empty.", nameof(tripPlanId));
        }

        return await include(ActiveEntities)
            .FirstOrDefaultAsync(tp => tp.Id == tripPlanId);
    }

    // public async Task<TripPlan?> GetWithIncludeAsync(Guid tripPlanId, Func<IQueryable<TripPlan>, IQueryable<TripPlan>> include)
    // {
    //     IQueryable<TripPlan> query = _dbSet;
    //     query = include(query);
    //     return await query.FirstOrDefaultAsync(p => p.Id == tripPlanId);
    // }

    public async Task<PagedResult<TripPlan>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var items = await ActiveEntities
            .Where(a => a.Name.Contains(name))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TripPlan>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<TripPlan>> GetPageWithSearchAsync(
        string? title,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(a => a.Name.Contains(title));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TripPlan>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
