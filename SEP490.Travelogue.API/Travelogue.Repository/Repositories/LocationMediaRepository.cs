using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationMediaRepository : IGenericRepository<LocationMedia>
{
    Task<LocationMedia> GetFirstByLocationIdAsync(Guid id);
}
public sealed class LocationMediaRepository : GenericRepository<LocationMedia>, ILocationMediaRepository
{
    private readonly ApplicationDbContext _context;

    public LocationMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<LocationMedia?> GetFirstByLocationIdAsync(Guid locationId)
    {
        return await _context.Set<LocationMedia>()
            .Where(em => em.LocationId == locationId)
            .OrderBy(em => em.CreatedTime) // Lấy ảnh đầu tiên theo thời gian tạo
            .FirstOrDefaultAsync();
    }
}
