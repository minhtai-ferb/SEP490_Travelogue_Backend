using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourGuideMappingRepository : IGenericRepository<TourGuideMapping>
{

    Task<PagedResult<TourGuideMapping>> GetPageWithSearchAsync(
        string? name,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class TourGuideMappingRepository : GenericRepository<TourGuideMapping>, ITourGuideMappingRepository
{
    private readonly ApplicationDbContext _context;

    public TourGuideMappingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<PagedResult<TourGuideMapping>> GetPageWithSearchAsync(
        string? name,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        // if (!string.IsNullOrWhiteSpace(name))
        // {
        //     query = query.Include(tg => tg.User)
        //                  .Where(tg => tg.User != null && !string.IsNullOrEmpty(tg.User.FullName) && tg.User.FullName.ToLower().Contains(name.ToLower()));
        // }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourGuideMapping>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
