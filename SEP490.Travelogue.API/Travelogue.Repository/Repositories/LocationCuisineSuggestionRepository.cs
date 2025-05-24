using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationCuisineSuggestionRepository : IGenericRepository<LocationCuisineSuggestion>
{

}
public sealed class LocationCuisineSuggestionRepository : GenericRepository<LocationCuisineSuggestion>, ILocationCuisineSuggestionRepository
{
    private readonly ApplicationDbContext _context;

    public LocationCuisineSuggestionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

}
