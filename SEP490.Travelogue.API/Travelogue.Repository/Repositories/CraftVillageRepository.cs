using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICraftVillageRepository : IGenericRepository<CraftVillage>
{
    Task<List<CraftVillage?>> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<CraftVillage>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class CraftVillageRepository : GenericRepository<CraftVillage>, ICraftVillageRepository
{
    private readonly ApplicationDbContext _context;

    public CraftVillageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<CraftVillage?>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var query = ActiveEntities.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query
                    .Include(cv => cv.Location)
                    .Where(cv => cv.Location.Name.Contains(name));
            }

            var result = await query.ToListAsync(cancellationToken);
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

    public async Task<PagedResult<CraftVillage>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query
                .Include(cv => cv.Location)
                .Where(cv => cv.Location.Name.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CraftVillage>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
