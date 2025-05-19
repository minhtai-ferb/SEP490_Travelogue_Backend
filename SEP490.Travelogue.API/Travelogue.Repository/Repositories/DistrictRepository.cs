using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IDistrictRepository : IGenericRepository<District>
{
    Task<District?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<string?> GetDistrictNameById(Guid? districtId);
    Task<string?> GetNameByIdAsync(Guid value, CancellationToken cancellationToken);
    Task<PagedResult<District>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class DistrictRepository : GenericRepository<District>, IDistrictRepository
{
    private readonly ApplicationDbContext _context;

    public DistrictRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<District?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var districts = _context.Districts.FirstOrDefaultAsync(a => a.Name.Contains(name), cancellationToken);
            return districts;
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

    public async Task<string?> GetDistrictNameById(Guid? districtId)
    {
        try
        {
            var districtName = await _context.Districts.FirstOrDefaultAsync(x => x.Id == districtId);

            var result = districtName?.Name;
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

    public Task<string?> GetNameByIdAsync(Guid value, CancellationToken cancellationToken)
    {
        var districtName = _context.Districts
            .Where(x => x.Id == value)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return districtName;
    }

    public async Task<PagedResult<District>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<District>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
