using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IMediaRepository : IGenericRepository<EventMedia>
{
    Task<List<EventMedia>> GetEventMediaAsync(Guid eventId, CancellationToken cancellationToken);
}
public sealed class MediaRepository : GenericRepository<EventMedia>, IMediaRepository
{
    private readonly ApplicationDbContext _context;

    public MediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public Task<List<EventMedia>> GetEventMediaAsync(Guid eventId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
