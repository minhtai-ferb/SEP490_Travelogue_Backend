using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourGuideRepository : IGenericRepository<TourGuide>
{
    Task<TourGuide?> GetByUserIdAsync(Guid userId);

    Task<PagedResult<TourGuide>> GetPageWithSearchAsync(
        string? name,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class TourGuideRepository : GenericRepository<TourGuide>, ITourGuideRepository
{
    private readonly ApplicationDbContext _context;

    public TourGuideRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TourGuide?> GetByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        return ActiveEntities.FirstOrDefaultAsync(tg => tg.UserId == userId);
    }

    public async Task<PagedResult<TourGuide>> GetPageWithSearchAsync(
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

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Include(tg => tg.User)
                         .Where(tg => tg.User != null && !string.IsNullOrEmpty(tg.User.FullName) && tg.User.FullName.ToLower().Contains(name.ToLower()));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourGuide>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
