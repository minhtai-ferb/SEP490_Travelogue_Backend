using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task UpdateAsync(Role Role);
    Task UpdateRoleAsync(Role Role);
    Task<List<Role>> GetAllRoleAsync(string? search = null);
    Task<PagedResult<Role>> GetPageRoleAsync(int pageNumber, int pageSize, string search, CancellationToken cancellationToken);
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetByNamesAsync(List<string> listName);
}
public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task UpdateAsync(Role Role)
    {
        _context.Roles.Update(Role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(Role Role)
    {
        _context.Roles.Update(Role);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetAllRoleAsync(string? search = null)
    {
        IQueryable<Role> query = _context.Roles;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return await query.ToListAsync();
    }

    public async Task<PagedResult<Role>> GetPageRoleAsync(int pageNumber, int pageSize, string search, CancellationToken cancellationToken)
    {
        IQueryable<Role> query = _context.Roles;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Name.ToLower().Contains(search.ToLower()));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Role>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        try
        {
            var query = await ActiveEntities.FirstOrDefaultAsync(u => u.Name == name);
            return query;
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

    public async Task<List<Role>> GetByNamesAsync(List<string> listName)
    {
        try
        {
            var result = await ActiveEntities.Where(u => listName.Contains(u.Name)).ToListAsync();
            return result;
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
}
