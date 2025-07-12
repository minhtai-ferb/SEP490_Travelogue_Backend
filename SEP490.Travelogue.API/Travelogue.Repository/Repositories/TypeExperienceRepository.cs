using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITypeExperienceRepository : IGenericRepository<TypeExperience>
{
    Task<TypeExperience?> GetByNameAsync(string typeName, CancellationToken cancellationToken);
    Task<PagedResult<TypeExperience>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
public sealed class TypeExperienceRepository : GenericRepository<TypeExperience>, ITypeExperienceRepository
{
    private readonly ApplicationDbContext _context;

    public TypeExperienceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<TypeExperience?> GetByNameAsync(string typeName, CancellationToken cancellationToken)
    {
        try
        {
            var typeExperiences = _context.TypeExperiences.FirstOrDefaultAsync(a => a.TypeName.Contains(typeName), cancellationToken);
            return typeExperiences;
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

    public async Task<PagedResult<TypeExperience>> GetPageWithSearchAsync(string? typeName, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than zero.");
        }

        var query = ActiveEntities.AsQueryable();
        if (!string.IsNullOrEmpty(typeName))
        {
            query = query.Where(a => a.TypeName.Contains(typeName));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TypeExperience>
        {
            Items = items,
            TotalCount = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
