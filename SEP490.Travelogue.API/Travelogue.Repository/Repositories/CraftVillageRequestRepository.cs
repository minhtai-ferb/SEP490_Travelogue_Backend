using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICraftVillageRequestRepository : IGenericRepository<CraftVillageRequest>
{
    Task<PagedResult<CraftVillageRequest>> GetPageWithSearchAsync(
        string? name,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class CraftVillageRequestRepository : GenericRepository<CraftVillageRequest>, ICraftVillageRequestRepository
{
    private readonly ApplicationDbContext _context;

    public CraftVillageRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<PagedResult<CraftVillageRequest>> GetPageWithSearchAsync(
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

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CraftVillageRequest>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
