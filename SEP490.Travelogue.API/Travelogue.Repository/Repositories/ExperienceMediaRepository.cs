using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IExperienceMediaRepository : IGenericRepository<ExperienceMedia>
{
    Task<ExperienceMedia?> GetFirstByExperienceIdAsync(Guid id);
}
public sealed class ExperienceMediaRepository : GenericRepository<ExperienceMedia>, IExperienceMediaRepository
{
    private readonly ApplicationDbContext _context;

    public ExperienceMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<ExperienceMedia?> GetFirstByExperienceIdAsync(Guid experienceId)
    {
        return await _context.Set<ExperienceMedia>()
            .Where(em => em.ExperienceId == experienceId)
            .OrderBy(em => em.CreatedTime)
            .FirstOrDefaultAsync();
    }
}
