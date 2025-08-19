using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IUserAnnouncementRepository : IGenericRepository<UserAnnouncement> { }

public sealed class UserAnnouncementRepository : GenericRepository<UserAnnouncement>, IUserAnnouncementRepository {    
    
    private readonly ApplicationDbContext _context;

    public UserAnnouncementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}