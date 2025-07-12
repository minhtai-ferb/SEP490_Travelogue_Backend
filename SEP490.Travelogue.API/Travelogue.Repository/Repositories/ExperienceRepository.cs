using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IExperienceRepository : IGenericRepository<Experience>
{
    Task<Experience?> GetByTitleAsync(string title, CancellationToken cancellationToken);
    Task<PagedResult<Experience>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default);
    Task<PagedResult<Experience>> GetPageWithSearchAsync(string? title, Guid? typeExperienceId, Guid? locationId, Guid? eventId, Guid? districtId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class ExperienceRepository : GenericRepository<Experience>, IExperienceRepository
{
    private readonly ApplicationDbContext _context;

    public ExperienceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<Experience?> GetByTitleAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            var experiences = _context.Experiences.FirstOrDefaultAsync(a => a.Title.Contains(title), cancellationToken);
            return experiences;
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

    public async Task<PagedResult<Experience>> GetPageWithSearchAsync(int pageNumber, int pageSize, string title, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var items = await ActiveEntities
            .Where(a => a.Title.Contains(title))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Experience>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Experience>> GetPageWithSearchAsync(string? title, Guid? typeExperienceId, Guid? locationId, Guid? eventId, Guid? districtId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(a => a.Title.Contains(title));
        }

        if (typeExperienceId.HasValue)
        {
            query = query.Where(a => a.TypeExperienceId == typeExperienceId);
        }

        if (locationId.HasValue)
        {
            query = query.Where(a => a.LocationId == locationId);
        }

        if (eventId.HasValue)
        {
            query = query.Where(a => a.EventId == eventId);
        }

        var items = await query
            .OrderBy(a => a.CreatedTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalItems = await query.CountAsync(cancellationToken);
        return new PagedResult<Experience>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
