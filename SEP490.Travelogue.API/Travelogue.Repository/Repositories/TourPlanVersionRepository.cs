using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITourPlanVersionRepository : IGenericRepository<TourPlanVersion>
{

}

public sealed class TourPlanVersionRepository : GenericRepository<TourPlanVersion>, ITourPlanVersionRepository
{
    private readonly ApplicationDbContext _context;

    public TourPlanVersionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

}