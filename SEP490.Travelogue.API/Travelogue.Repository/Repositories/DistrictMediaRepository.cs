using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IDistrictMediaRepository : IGenericRepository<DistrictMedia>
{
    Task<DistrictMedia?> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<DistrictMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class DistrictMediaRepository : GenericRepository<DistrictMedia>, IDistrictMediaRepository
{
    private readonly ApplicationDbContext _context;

    public DistrictMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<DistrictMedia?> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            var events = _context.DistrictMedias.FirstOrDefaultAsync(a => a.FileName != null && a.FileName.Contains(title), cancellationToken);
            return events;
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

    public async Task<PagedResult<DistrictMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
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

        return new PagedResult<DistrictMedia>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
