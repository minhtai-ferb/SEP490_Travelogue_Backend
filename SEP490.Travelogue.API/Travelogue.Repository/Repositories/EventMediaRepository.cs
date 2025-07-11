using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IEventMediaRepository : IGenericRepository<EventMedia>
{
    Task<EventMedia?> GetFirstByEventIdAsync(Guid id);
}
public sealed class EventMediaRepository : GenericRepository<EventMedia>, IEventMediaRepository
{
    private readonly ApplicationDbContext _context;

    public EventMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<EventMedia?> GetFirstByEventIdAsync(Guid eventId)
    {
        return await _context.Set<EventMedia>()
            .Where(em => em.EventId == eventId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }
}
