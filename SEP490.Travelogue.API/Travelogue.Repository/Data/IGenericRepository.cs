using System.Linq.Expressions;
using Travelogue.Repository.Bases;

namespace Travelogue.Repository.Data;

public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves all active entities as an asynchronous operation.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of active entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paged result of entities.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paged result.</returns>
    /// <exception cref="ArgumentException">Thrown when pageNumber or pageSize is less than 1.</exception>
    Task<PagedResult<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queryable collection of active and non-deleted entities.
    /// </summary>
    IQueryable<TEntity> ActiveEntities { get; }

    /// <summary>
    /// Gets a queryable collection of all entities.
    /// </summary>
    IQueryable<TEntity> Entities { get; }

    /// <summary>
    /// Retrieves a queryable collection of all active entities.
    /// </summary>
    /// <returns>An IQueryable of active entities.</returns>
    IQueryable<TEntity> GetAllQueryable();

    /// <summary>
    /// Converts a queryable to a list asynchronously.
    /// </summary>
    /// <param name="query">The IQueryable to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of entities.</returns>
    Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity by its ID, regardless of its active or deleted state.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an active entity by its ID if it is not deleted.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity or null if not found or inactive.</returns>
    Task<TEntity?> GetActiveEntityByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Adds a range of entities to the database.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated list of active entities.</returns>
    /// <exception cref="ArgumentException">Thrown when entities is null or empty.</exception>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    void Remove(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);
    //Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);

    /// <summary>
    /// Retrieves an entity with related data based on a predicate and included navigation properties.
    /// </summary>
    /// <param name="predicate">The condition to filter the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includeProperties">The navigation properties to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity or null if not found.</returns>
    Task<TEntity?> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Retrieves an entity with related data based on a predicate and included navigation properties.
    /// </summary>
    /// <param name="predicate">The condition to filter the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includeProperties">The navigation properties to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity or null if not found.</returns>
    Task<TEntity?> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includeProperties);

    //Task<TEntity?> GetByIdAsync(object id);
    //Task<IEnumerable<TEntity>> GetPageAsync(int pageNumber, int pageSize);
    //Task<IQueryable<TEntity>> GetAllIQueryableAsync();
    //Task<TEntity?> GetByIdAllAsync(object id);
}
