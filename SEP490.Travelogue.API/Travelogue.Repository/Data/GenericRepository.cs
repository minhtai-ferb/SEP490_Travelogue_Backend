using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.BaseEntities;

namespace Travelogue.Repository.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> Entities => _dbSet;
    public IQueryable<T> ActiveEntities => _dbSet.Where(e => e.IsActive && !e.IsDeleted);

    public IQueryable<T> GetAllQueryable() => ActiveEntities;

    public async Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await ActiveEntities.ToListAsync(cancellationToken);

    //public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    //{
    //    var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    //    return entity != null && entity.IsActive && !entity.IsDeleted ? entity : null;
    //}

    public async Task<T?> GetActiveEntityByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity != null && entity.IsActive && !entity.IsDeleted ? entity : null;
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
     .AsNoTracking()
     .FirstOrDefaultAsync(e => e.Id == (Guid)id, cancellationToken);
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentException("The entities list cannot be null or empty.", nameof(entities));
        }

        await _dbSet.AddRangeAsync(entities);
        // await _context.SaveChangesAsync();

        return await ActiveEntities.ToListAsync();
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentException("The entities list cannot be null or empty.", nameof(entities));
        }

        await _dbSet.AddRangeAsync(entities, cancellationToken);
        // await _context.SaveChangesAsync(cancellationToken);

        return await ActiveEntities.ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            // await _context.SaveChangesAsync();

            return entity;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"-----------------Error creating exchange session: {ex.Message}");

            throw new Exception("An error occurred while adding the entity.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"-----------------Error creating exchange session: {ex.Message}");

            throw new Exception("An unexpected error occurred while adding the entity.", ex);
        }

    }

    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Remove(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        _dbSet.RemoveRange(entities);
    }

    public async Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var query = includeProperties.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<T>, IQueryable<T>>[] includeProperties)
    {
        var query = includeProperties.Aggregate(_dbSet.AsQueryable(), (current, include) => include(current));
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetWithIncludeAsync(Guid id, Func<IQueryable<T>, IQueryable<T>> include)
    {
        IQueryable<T> query = _dbSet;
        query = include(query);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<PagedResult<T>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var totalItems = await ActiveEntities.CountAsync(cancellationToken);
        var items = await ActiveEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
