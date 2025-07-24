using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourPlanLocationRepository : IGenericRepository<TourPlanLocation>
{
}

public sealed class TourPlanLocationRepository : GenericRepository<TourPlanLocation>, ITourPlanLocationRepository
{
    private readonly ApplicationDbContext _context;

    public TourPlanLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
