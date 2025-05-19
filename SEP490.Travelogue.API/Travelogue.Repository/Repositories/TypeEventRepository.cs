using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITypeEventRepository : IGenericRepository<TypeEvent>
{
    Task<TypeEvent?> GetByNameAsync(string typeName, CancellationToken cancellationToken);
    Task<PagedResult<TypeEvent>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<string?> GetTypeEventNameById(Guid? typeEventId);
}
public sealed class TypeEventRepository : GenericRepository<TypeEvent>, ITypeEventRepository
{
    private readonly ApplicationDbContext _context;

    public TypeEventRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TypeEvent?> GetByNameAsync(string typeName, CancellationToken cancellationToken)
    {
        try
        {
            var typeEvents = _context.TypeEvents.FirstOrDefaultAsync(a => a.TypeName.Contains(typeName), cancellationToken);
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

    public async Task<PagedResult<TypeEvent>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(typeName))
        {
            query = query.Where(a => a.TypeName.Contains(typeName));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TypeEvent>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<string?> GetTypeEventNameById(Guid? typeEventId)
    {
        try
        {
            var query = ActiveEntities.AsQueryable();
            var nameEvents = query.FirstOrDefaultAsync(x => x.Id == typeEventId);
            var result = nameEvents.Result?.TypeName;
            return result;
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
