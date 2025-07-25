using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Repository.Utils;

namespace Travelogue.Repository.Repositories;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<PagedResult<Location>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Location>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, Guid typeId, CancellationToken cancellationToken = default);
    Task<PagedResult<Location>> GetPageWithSearchAsync(
        string? title,
        LocationType? typeId,
        Guid? districtId,
        HeritageRank? heritageRank,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Location>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default, LocationType? locationType = null);
    Task<List<string>> GetAllCategoriesAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<string> GetCategoryNameAsync(Guid locationId, CancellationToken cancellationToken = default);
}
public sealed class LocationRepository : GenericRepository<Location>, ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<string>> GetAllCategoriesAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location == null)
        {
            return new List<string>();
        }

        // Assuming LocationType is an enum or has a Type property that's an enum
        return location.LocationType != null
            ? new List<string> { location.LocationType.GetDisplayName() }
            : new List<string>();
    }

    public async Task<string> GetCategoryNameAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location == null)
        {
            return string.Empty;
        }

        return location.LocationType.GetDisplayName();
    }

    public async Task<PagedResult<Location>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Location>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default, LocationType? locationType = null)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities
            .Where(l => l.LocationType == locationType)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Location>> GetPageWithSearchAsync(int pageNumber, int pageSize, string name, Guid typeId, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        // if (typeId != Guid.Empty)
        // {
        //     query = query.Where(a => a.TypeLocationId == typeId);
        // }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Location>> GetPageWithSearchAsync(
        string? title,
        LocationType? type,
        Guid? districtId,
        HeritageRank? heritageRank,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
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

        if (type.HasValue)
        {
            query = query.Where(a => a.LocationType == type.Value);
        }

        if (districtId.HasValue)
        {
            query = query.Where(a => a.DistrictId == districtId.Value);
        }

        // if (heritageRank.HasValue)
        // {
        //     query = query.Where(a => a.HeritageRank == heritageRank.Value);
        // }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Location>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
