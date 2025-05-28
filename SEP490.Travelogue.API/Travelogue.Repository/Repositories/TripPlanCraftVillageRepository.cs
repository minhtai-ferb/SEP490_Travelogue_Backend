using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ITripPlanCraftVillageRepository : IGenericRepository<TripPlanCraftVillage>
{
}
public sealed class TripPlanCraftVillageRepository : GenericRepository<TripPlanCraftVillage>, ITripPlanCraftVillageRepository
{
    private readonly ApplicationDbContext _context;

    public TripPlanCraftVillageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
