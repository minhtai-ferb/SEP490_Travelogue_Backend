using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface INewsCategoryRepository : IGenericRepository<NewsCategory>
{
    Task<NewsCategory?> GetByNameAsync(string category, CancellationToken cancellationToken);
    Task<PagedResult<NewsCategory>> GetPageWithSearchAsync(string? category, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class NewsCategoryRepository : GenericRepository<NewsCategory>, INewsCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public NewsCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<NewsCategory?> GetByNameAsync(string category, CancellationToken cancellationToken)
    {
        try
        {
            var typeEvents = _context.NewsCategories.FirstOrDefaultAsync(a => a.Category.Contains(category), cancellationToken);
            return typeEvents;
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

    public async Task<PagedResult<NewsCategory>> GetPageWithSearchAsync(string? category, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(a => a.Category.Contains(category));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NewsCategory>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
