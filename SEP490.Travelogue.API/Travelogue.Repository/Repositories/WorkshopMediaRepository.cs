using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IWorkshopMediaRepository : IGenericRepository<WorkshopMedia>
{
    Task<WorkshopMedia?> GetByNameAsync(string title, CancellationToken cancellationToken);
    Task<WorkshopMedia?> GetFirstByWorkshopIdAsync(Guid id);
    Task<PagedResult<WorkshopMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class WorkshopMediaRepository : GenericRepository<WorkshopMedia>, IWorkshopMediaRepository
{
    private readonly ApplicationDbContext _context;

    public WorkshopMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<WorkshopMedia?> GetFirstByWorkshopIdAsync(Guid newsId)
    {
        return await _context.Set<WorkshopMedia>()
            .Where(em => em.WorkshopId == newsId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }

    public Task<WorkshopMedia?> GetByNameAsync(string title, CancellationToken cancellationToken)
    {
        try
        {
            if (title == null)
            {
                return Task.FromResult<WorkshopMedia?>(null);
            }
            var newsMedias = _context.WorkshopMedias.FirstOrDefaultAsync(a => a.FileName != null && a.FileName.Contains(title), cancellationToken);
            return newsMedias;
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

    public async Task<PagedResult<WorkshopMedia>> GetPageWithSearchAsync(string? name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.FileName != null && a.FileName.Contains(name));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<WorkshopMedia>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
