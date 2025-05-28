using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanCuisineRepository : IGenericRepository<TripPlanCuisine>
{
}
public sealed class TripPlanCuisineRepository : GenericRepository<TripPlanCuisine>, ITripPlanCuisineRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanCuisineRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
