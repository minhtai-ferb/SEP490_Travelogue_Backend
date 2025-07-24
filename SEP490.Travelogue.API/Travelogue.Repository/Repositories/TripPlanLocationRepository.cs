using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanLocationRepository : IGenericRepository<TripPlanLocation>
{
}
public sealed class TripPlanLocationRepository : GenericRepository<TripPlanLocation>, ITripPlanLocationRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
