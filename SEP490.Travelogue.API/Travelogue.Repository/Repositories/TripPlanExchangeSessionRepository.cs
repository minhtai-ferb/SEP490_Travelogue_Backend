using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanExchangeSessionRepository : IGenericRepository<TripPlanExchangeSession>
{
}
public sealed class TripPlanExchangeSessionRepository : GenericRepository<TripPlanExchangeSession>, ITripPlanExchangeSessionRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanExchangeSessionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}