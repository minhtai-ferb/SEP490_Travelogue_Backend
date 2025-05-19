using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;
public interface IRoleDistrictRepository : IGenericRepository<RoleDistrict>
{
    Task UpdateAsync(RoleDistrict RoleDistrict);
    Task UpdateRoleDistrictAsync(RoleDistrict RoleDistrict);
    Task<List<RoleDistrict>> GetAllRoleDistrictAsync();
    Task<PagedResult<RoleDistrict>> GetPageRoleDistrictAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid districtId, Guid id);
    Task<Guid> GetDistrictIdByRoleId(string id);
    //Task<RoleDistrict> GetRoleDistrictByNameAsync();
}
public class RoleDistrictRepository : GenericRepository<RoleDistrict>, IRoleDistrictRepository
{
    private readonly ApplicationDbContext _context;

    public RoleDistrictRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task UpdateAsync(RoleDistrict RoleDistrict)
    {
        _context.RoleDistricts.Update(RoleDistrict);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoleDistrictAsync(RoleDistrict RoleDistrict)
    {
        _context.RoleDistricts.Update(RoleDistrict);
        await _context.SaveChangesAsync();
    }

    public Task<string> GetRoleDistrictEmailByIdAsync(string RoleDistrictId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<RoleDistrict>> GetAllRoleDistrictAsync()
    {
        IQueryable<RoleDistrict> query = _context.RoleDistricts;

        return await query.ToListAsync();
    }

    public async Task<PagedResult<RoleDistrict>> GetPageRoleDistrictAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<RoleDistrict> query = _context.RoleDistricts;

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<RoleDistrict>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> ExistsAsync(Guid districtId, Guid roleId)
    {
        return await _context.RoleDistricts.AnyAsync(ur => ur.DistrictId == districtId && ur.RoleId == roleId);
    }

    public async Task<Guid> GetDistrictIdByRoleId(string id)
    {
        return await _context.RoleDistricts.Where(x => x.RoleId == Guid.Parse(id)).Select(x => x.DistrictId).FirstOrDefaultAsync();
    }
}
