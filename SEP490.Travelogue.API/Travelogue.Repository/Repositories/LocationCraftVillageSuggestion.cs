using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationCraftVillageSuggestionRepository : IGenericRepository<LocationCraftVillageSuggestion>
{
}
public sealed class LocationCraftVillageSuggestionRepository : GenericRepository<LocationCraftVillageSuggestion>, ILocationCraftVillageSuggestionRepository
{
    private readonly ApplicationDbContext _context;

    public LocationCraftVillageSuggestionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
