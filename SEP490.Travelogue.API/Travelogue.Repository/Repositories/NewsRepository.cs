using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Repository.Repositories;

public interface INewsRepository : IGenericRepository<News>
{
    Task<News?> GetByNameAsync(string title, CancellationToken cancellationToken);

    Task<PagedResult<News>> GetPageWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<News>> GetPageWithSearchAsync(string? title, Guid? locationId, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<News>> GetPageWithSearchAsync(string? title, Guid? locationId, Boolean? isHighlighted, TypeExperience? typeExperience, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class NewsRepository : GenericRepository<News>, INewsRepository
{
    private readonly ApplicationDbContext _context;

    public NewsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<News?> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            var news = _context.News.FirstOrDefaultAsync(a => a.Title.Contains(title), cancellationToken);
            return news;
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

    public async Task<PagedResult<News>> GetPageWithSearchAsync(string? title, Guid? locationId, Boolean? isHighlighted, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(a => a.Title.Contains(title));
        }

        if (locationId.HasValue && locationId.Value != Guid.Empty)
        {
            query = query.Where(a => a.LocationId == locationId.Value);
        }

        if (isHighlighted.HasValue)
        {
            query = query.Where(a => a.IsHighlighted == isHighlighted.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<News>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<News>> GetPageWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(a => a.Title.Contains(title));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<News>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<News>> GetPageWithSearchAsync(string? title, Guid? locationId, Boolean? isHighlighted, TypeExperience? typeExperience, int pageNumber, int pageSize, CancellationToken cancellationToken = default) 
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(a => a.Title.Contains(title));
        }

        if (locationId.HasValue && locationId.Value != Guid.Empty)
        {
            query = query.Where(a => a.LocationId == locationId.Value);
        }

        if (isHighlighted.HasValue)
        {
            query = query.Where(a => a.IsHighlighted == isHighlighted.Value);
        }

        if (typeExperience.HasValue)
        {
            query = query.Where(a => a.TypeExperience == typeExperience.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<News>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
