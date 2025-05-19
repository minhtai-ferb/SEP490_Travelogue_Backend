using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IHotelMediaRepository : IGenericRepository<HotelMedia>
{
    Task<HotelMedia> GetFirstByHotelIdAsync(Guid id);
}
public sealed class HotelMediaRepository : GenericRepository<HotelMedia>, IHotelMediaRepository
{
    private readonly ApplicationDbContext _context;

    public HotelMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<HotelMedia?> GetFirstByHotelIdAsync(Guid eventId)
    {
        return await _context.Set<HotelMedia>()
            .Where(em => em.HotelId == eventId)
            .OrderBy(em => em.CreatedTime) // Lấy ảnh đầu tiên theo thời gian tạo
            .FirstOrDefaultAsync();
    }
}
