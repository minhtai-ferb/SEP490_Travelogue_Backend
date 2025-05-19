using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface INewsRepository : IGenericRepository<News>
{
    Task<News?> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<News>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default);
    Task<PagedResult<News>> GetPageWithSearchAsync(string? title, string? categoryName, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
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
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<PagedResult<News>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var items = await ActiveEntities
            .Where(a => a.Title.Contains(title))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalItems = await ActiveEntities.CountAsync(cancellationToken);

        return new PagedResult<News>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<News>> GetPageWithSearchAsync(string? title, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
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

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.NewsCategoryId == categoryId.Value);
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

    public async Task<PagedResult<News>> GetPageWithSearchAsync(
        string? title,
        string? categoryName,
        Guid? categoryId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities
            .Include(n => n.NewsCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(n => n.Title.Contains(title));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(n => n.NewsCategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(n => n.NewsCategory != null &&
                                     n.NewsCategory.Category.Contains(categoryName));
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
