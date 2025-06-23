using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<List<Hotel?>> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<Hotel>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Hotel?> GetByLocationId(Guid locationId, CancellationToken cancellationToken);
}
public sealed class HotelRepository : GenericRepository<Hotel>, IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<Hotel?>> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            var events = await _context.Hotels.Include(h => h.Location)
                .Where(a => a.Location.Name.Contains(title))
                .ToListAsync(cancellationToken);
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

    public async Task<Hotel?> GetByLocationId(Guid locationId, CancellationToken cancellationToken)
    {
        try
        {
            var hotel = await _context.Hotels.Include(h => h.Location)
                .FirstOrDefaultAsync(a => a.LocationId == locationId, cancellationToken);
            return hotel;
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

    public async Task<PagedResult<Hotel>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Include(q => q.Location).Where(a => a.Location.Name.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Hotel>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
