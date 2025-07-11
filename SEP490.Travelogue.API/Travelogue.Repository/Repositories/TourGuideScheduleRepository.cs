using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourGuideScheduleRepository : IGenericRepository<TourGuideSchedule>
{
    Task<TourGuideSchedule?> GetByUserIdAsync(Guid userId);

    Task<PagedResult<TourGuideSchedule>> GetPageWithSearchAsync(
        string? name,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed class TourGuideScheduleRepository : GenericRepository<TourGuideSchedule>, ITourGuideScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public TourGuideScheduleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TourGuideSchedule?> GetByUserIdAsync(Guid tourGuideId)
    {
        if (tourGuideId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(tourGuideId));
        }

        return ActiveEntities.FirstOrDefaultAsync(tg => tg.TourGuideId == tourGuideId);
    }

    public async Task<PagedResult<TourGuideSchedule>> GetPageWithSearchAsync(
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
            query = query.Include(tg => tg.TourGuide)
                .ThenInclude(tg => tg.User)
                .Where(tg => tg.TourGuide.User != null && !string.IsNullOrEmpty(tg.TourGuide.User.FullName) && tg.TourGuide.User.FullName.ToLower().Contains(name.ToLower()));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourGuideSchedule>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
