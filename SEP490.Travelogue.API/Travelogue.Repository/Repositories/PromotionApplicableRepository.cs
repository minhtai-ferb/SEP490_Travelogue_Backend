using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IPromotionApplicableRepository : IGenericRepository<PromotionApplicable>
{
    Task<PagedResult<PromotionApplicable>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    // Task<PagedResult<Order>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default);
    Task<PagedResult<PromotionApplicable>> GetPageWithSearchAsync(
        string? title,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class PromotionApplicableRepository : GenericRepository<PromotionApplicable>, IPromotionApplicableRepository
{
    private readonly ApplicationDbContext _context;

    public PromotionApplicableRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<PagedResult<PromotionApplicable>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default)
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

        return new PagedResult<PromotionApplicable>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PromotionApplicable>> GetPageWithSearchAsync(
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

        return new PagedResult<PromotionApplicable>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
