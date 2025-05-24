using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ICuisineMediaRepository : IGenericRepository<CuisineMedia>
{
    Task<CuisineMedia> GetFirstByCuisineIdAsync(Guid cuisineId);
}
public sealed class CuisineMediaRepository : GenericRepository<CuisineMedia>, ICuisineMediaRepository
{
    private readonly ApplicationDbContext _context;

    public CuisineMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<CuisineMedia?> GetFirstByCuisineIdAsync(Guid cuisineId)
    {
        return await _context.Set<CuisineMedia>()
            .Where(em => em.CuisineId == cuisineId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }
}
