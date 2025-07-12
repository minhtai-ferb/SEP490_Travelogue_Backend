using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourTypeRepository : IGenericRepository<TourType>
{
    Task<TourType?> GetByNameAsync(string typeName, CancellationToken cancellationToken);
    Task<PagedResult<TourType>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<string?> GetTourTypeNameById(Guid? typeEventId);
}
public sealed class TourTypeRepository : GenericRepository<TourType>, ITourTypeRepository
{
    private readonly ApplicationDbContext _context;

    public TourTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TourType?> GetByNameAsync(string typeName, CancellationToken cancellationToken)
    {
        try
        {
            var typeEvents = _context.TourTypes.FirstOrDefaultAsync(a => a.Name.Contains(typeName), cancellationToken);
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

    public async Task<PagedResult<TourType>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
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

        return new PagedResult<TourType>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<string?> GetTourTypeNameById(Guid? typeEventId)
    {
        try
        {
            var query = ActiveEntities.AsQueryable();
            var nameEvents = await query.FirstOrDefaultAsync(x => x.Id == typeEventId);
            var result = nameEvents?.Name;
            return result;
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
}
