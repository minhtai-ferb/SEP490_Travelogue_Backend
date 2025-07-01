using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITypeHistoricalLocationRepository : IGenericRepository<TypeHistoricalLocation>
{
    Task<TypeHistoricalLocation?> GetByNameAsync(string typeName, CancellationToken cancellationToken);
    Task<PagedResult<TypeHistoricalLocation>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class TypeHistoricalLocationRepository : GenericRepository<TypeHistoricalLocation>, ITypeHistoricalLocationRepository
{
    private readonly ApplicationDbContext _context;

    public TypeHistoricalLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TypeHistoricalLocation?> GetByNameAsync(string typeName, CancellationToken cancellationToken)
    {
        try
        {
            var typeEvents = _context.TypeHistoricalLocations.FirstOrDefaultAsync(a => a.Name.Contains(typeName), cancellationToken);
            return typeEvents;
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

    public async Task<PagedResult<TypeHistoricalLocation>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(typeName))
        {
            query = query.Where(a => a.Name.Contains(typeName));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TypeHistoricalLocation>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
