using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationRestaurantSuggestionRepository : IGenericRepository<LocationRestaurantSuggestion>
{

}
public sealed class LocationRestaurantSuggestionRepository : GenericRepository<LocationRestaurantSuggestion>, ILocationRestaurantSuggestionRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRestaurantSuggestionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

}
