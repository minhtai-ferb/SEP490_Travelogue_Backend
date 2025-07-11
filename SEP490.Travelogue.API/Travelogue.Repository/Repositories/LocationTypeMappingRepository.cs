using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface ILocationTypeMappingRepository : IGenericRepository<LocationTypeMapping>
{

}

public sealed class LocationTypeMappingRepository : GenericRepository<LocationTypeMapping>, ILocationTypeMappingRepository
{
    private readonly ApplicationDbContext _context;

    public LocationTypeMappingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }


}
