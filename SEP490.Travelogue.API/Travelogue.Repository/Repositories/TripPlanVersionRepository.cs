using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanVersionRepository : IGenericRepository<TripPlanVersion>
{
    // Task<PagedResult<TripPlanVersion>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    // Task<PagedResult<TripPlanVersion>> GetPageWithSearchAsync(
    //     string? title,
    //     int pageNumber,
    //     int pageSize,
    //     CancellationToken cancellationToken = default);
}

public sealed class TripPlanVersionRepository : GenericRepository<TripPlanVersion>, ITripPlanVersionRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanVersionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    // public async Task<PagedResult<TrTripPlanVersionipPlan>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default)
    // {
    //     if (pageNumber < 1 || pageSize < 1)
    //     {
    //         throw new ArgumentException("Page number and page size must be greater than zero.");
    //     }

    //     var totalItems = await ActiveEntities.CountAsync(cancellationToken);
    //     var items = await ActiveEntities
    //         .Where(a => a.Name.Contains(name))
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync(cancellationToken);

    //     return new PagedResult<TripPlan>
    //     {
    //         Items = items,
    //         TotalCount = totalItems,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize
    //     };
    // }

    // public async Task<PagedResult<TripPlanVersion>> GetPageWithSearchAsync(
    //     string? title,
    //     int pageNumber,
    //     int pageSize,
    //     CancellationToken cancellationToken = default)
    // {
    //     if (pageNumber < 1 || pageSize < 1)
    //     {
    //         throw new ArgumentException("Page number and page size must be greater than zero.");
    //     }

    //     var query = ActiveEntities.AsQueryable();

    //     if (!string.IsNullOrWhiteSpace(title))
    //     {
    //         query = query.Where(a => a.Name.Contains(title));
    //     }

    //     var totalItems = await query.CountAsync(cancellationToken);

    //     var items = await query
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync(cancellationToken);

    //     return new PagedResult<TripPlan>
    //     {
    //         Items = items,
    //         TotalCount = totalItems,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize
    //     };
    // }
}
