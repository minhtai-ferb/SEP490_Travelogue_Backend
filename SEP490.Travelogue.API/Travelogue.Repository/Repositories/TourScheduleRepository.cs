using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourScheduleRepository : IGenericRepository<TourSchedule>
{
    Task<PagedResult<TourSchedule>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    // Task<PagedResult<TourSchedule>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    Task<PagedResult<TourSchedule>> GetPageWithSearchAsync(
        string? title,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class TourScheduleRepository : GenericRepository<TourSchedule>, ITourScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public TourScheduleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<PagedResult<TourSchedule>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var items = await ActiveEntities
            //.Where(a => a.Name.Contains(name))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourSchedule>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<TourSchedule>> GetPageWithSearchAsync(
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

        // if (!string.IsNullOrWhiteSpace(title))
        // {
        //     query = query.Where(a => a.Name.Contains(title));
        // }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourSchedule>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
