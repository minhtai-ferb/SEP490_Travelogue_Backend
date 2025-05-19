using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<PagedResult<Event>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default);
    Task<PagedResult<Event>> GetPageWithFilterAsync(
        int pageNumber,
        int pageSize,
        string? name,
        Guid? typeId,
        Guid? locationId,
        int? month,
        int? year,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Event>> GetPageWithFilterAsync(
        string? name,
        Guid? typeId,
        Guid? locationId,
        Guid? districtId,
        int? month,
        int? year,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<List<Event>> GetHighlightedEvent(CancellationToken cancellationToken);
}
public sealed class EventRepository : GenericRepository<Event>, IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<Event?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var events = _context.Events.FirstOrDefaultAsync(a => a.Name.Contains(name), cancellationToken);
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

    public async Task<PagedResult<Event>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var items = await ActiveEntities
            .Where(a => a.Name.Contains(name))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Event>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Event>> GetPageWithFilterAsync(
        int pageNumber,
        int pageSize,
        string? name,
        Guid? typeId,
        Guid? locationId,
        int? month,
        int? year,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        if (typeId.HasValue)
        {
            query = query.Where(a => a.TypeEventId == typeId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(a => a.LocationId == locationId.Value);
        }

        if (year.HasValue && month.HasValue)
        {
            query = query.Where(a => a.StartDate.HasValue && a.EndDate.HasValue &&
                                     a.StartDate.Value.Year == year.Value &&
                                     (a.StartDate.Value.Month == month.Value || a.EndDate.Value.Month == month.Value));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Event>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Event>> GetPageWithFilterAsync(
        string? name,
        Guid? typeId,
        Guid? locationId,
        Guid? districtId,
        int? month,
        int? year,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        if (typeId.HasValue)
        {
            query = query.Where(a => a.TypeEventId == typeId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(a => a.LocationId == locationId.Value);
        }

        if (districtId.HasValue)
        {
            query = query.Where(a => a.DistrictId == districtId.Value);
        }

        if (year.HasValue && month.HasValue && year.Value > 0 && month.Value > 0)
        {
            query = query.Where(a => a.StartDate.HasValue && a.EndDate.HasValue &&
                                     a.StartDate.Value.Year == year.Value &&
                                     (a.StartDate.Value.Month == month.Value || a.EndDate.Value.Month == month.Value));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.StartDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.EventMedias)
            .ToListAsync(cancellationToken);

        return new PagedResult<Event>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public Task<List<Event>> GetHighlightedEvent(CancellationToken cancellationToken)
    {
        try
        {
            var highlightedEvent = _context.Events
                .Where(a => a.IsHighlighted == true)
                .Where(a => a.IsActive)
                .Where(a => !a.IsDeleted)
                .ToListAsync(cancellationToken);

            return highlightedEvent;
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
