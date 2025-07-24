using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IHistoricalLocationRepository : IGenericRepository<HistoricalLocation>
{
    Task<HistoricalLocation?> GetByLocationId(Guid locationId, CancellationToken cancellationToken);
}

public sealed class HistoricalLocationRepository : GenericRepository<HistoricalLocation>, IHistoricalLocationRepository
{
    private readonly ApplicationDbContext _context;

    public HistoricalLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<HistoricalLocation?> GetByLocationId(Guid locationId, CancellationToken cancellationToken)
    {
        try
        {
            var historicalLocation = await _context.HistoricalLocations.Include(h => h.Location)
                .FirstOrDefaultAsync(a => a.LocationId == locationId, cancellationToken);
            return historicalLocation;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }
}
