using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITypeLocationRepository : IGenericRepository<TypeLocation>
{
    Task<PagedResult<TypeLocation>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<string?> GetTypeLocationNameById(Guid? typeLocationId);
}
public sealed class TypeLocationRepository : GenericRepository<TypeLocation>, ITypeLocationRepository
{
    private readonly ApplicationDbContext _context;

    public TypeLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<PagedResult<TypeLocation>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
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

        return new PagedResult<TypeLocation>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public Task<string?> GetTypeLocationNameById(Guid? typeLocationId)
    {
        try
        {
            var query = ActiveEntities.AsQueryable();
            var typeLocationName = query.FirstOrDefaultAsync(x => x.Id == typeLocationId);
            var result = typeLocationName.Result?.Name;
            return Task.FromResult(result);
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
}
