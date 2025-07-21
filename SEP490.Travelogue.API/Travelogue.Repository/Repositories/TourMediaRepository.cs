using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourMediaRepository : IGenericRepository<TourMedia>
{
    Task<TourMedia?> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<TourMedia?> GetFirstByTourIdAsync(Guid id);
    Task<PagedResult<TourMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class TourMediaRepository : GenericRepository<TourMedia>, ITourMediaRepository
{
    private readonly ApplicationDbContext _context;

    public TourMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<TourMedia?> GetFirstByTourIdAsync(Guid newsId)
    {
        return await _context.Set<TourMedia>()
            .Where(em => em.TourId == newsId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }

    public Task<TourMedia?> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            if (title == null)
            {
                return Task.FromResult<TourMedia?>(null);
            }
            var newsMedias = _context.TourMedias.FirstOrDefaultAsync(a => a.FileName != null && a.FileName.Contains(title), cancellationToken);
            return newsMedias;
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

    public async Task<PagedResult<TourMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.FileName != null && a.FileName.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TourMedia>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
