using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationHotelSuggestionRepository : IGenericRepository<LocationHotelSuggestion>
{
}
public sealed class LocationHotelSuggestionRepository : GenericRepository<LocationHotelSuggestion>, ILocationHotelSuggestionRepository
{
    private readonly ApplicationDbContext _context;

    public LocationHotelSuggestionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
