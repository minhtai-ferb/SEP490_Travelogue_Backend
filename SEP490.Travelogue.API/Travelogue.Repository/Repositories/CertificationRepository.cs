using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICertificationRepository : IGenericRepository<Certification>
{
    Task<Certification?> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<Certification>> GetPageWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class CertificationRepository : GenericRepository<Certification>, ICertificationRepository
{
    private readonly ApplicationDbContext _context;

    public CertificationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<Certification?> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            var news = _context.Certifications.FirstOrDefaultAsync(a => a.Name.Contains(title), cancellationToken);
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

    public async Task<PagedResult<Certification>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var items = await ActiveEntities
            .Where(a => a.Name.Contains(title))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalItems = await ActiveEntities.CountAsync(cancellationToken);

        return new PagedResult<Certification>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Certification>> GetPageWithSearchAsync(string? title, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(a => a.Name.Contains(title));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Certification>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
