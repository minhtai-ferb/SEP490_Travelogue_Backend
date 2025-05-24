using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICraftVillageMediaRepository : IGenericRepository<CraftVillageMedia>
{
    Task<CraftVillageMedia> GetFirstByCraftVillageIdAsync(Guid id);
}
public sealed class CraftVillageMediaRepository : GenericRepository<CraftVillageMedia>, ICraftVillageMediaRepository
{
    private readonly ApplicationDbContext _context;

    public CraftVillageMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<CraftVillageMedia?> GetFirstByCraftVillageIdAsync(Guid eventId)
    {
        return await _context.Set<CraftVillageMedia>()
            .Where(em => em.CraftVillageId == eventId)
            .OrderBy(em => em.CreatedTime) // Lấy ảnh đầu tiên theo thời gian tạo
            .FirstOrDefaultAsync();
    }
}
