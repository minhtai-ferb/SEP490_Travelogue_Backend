using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IHistoricalLocationRepository : IGenericRepository<HistoricalLocation>
{
}

public sealed class HistoricalLocationRepository : GenericRepository<HistoricalLocation>, IHistoricalLocationRepository
{
    private readonly ApplicationDbContext _context;

    public HistoricalLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}
