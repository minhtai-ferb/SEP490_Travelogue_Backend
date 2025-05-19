using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;
public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<bool> AddRolesToUser(Guid userId, List<Guid> rolesToAdd);
    Task<bool> RoleExistsForUserAsync(Guid userId, Guid id);
    Task<List<UserRole>> GetByUserId(Guid userId);
    Task<bool> RemoveFromRolesAsync(Guid user, List<Guid> rolesToRemove);
}
public sealed class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    private readonly ApplicationDbContext _context;

    public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<bool> AddRolesToUser(Guid userId, List<Guid> rolesToAdd)
    {
        try
        {
            var userRoles = rolesToAdd.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId
            }).ToList();
            await _context.UserRoles.AddRangeAsync(userRoles);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> RoleExistsForUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<List<UserRole>> GetByUserId(Guid userId)
    {
        try
        {
            return await _context.UserRoles.Where(x => x.UserId == userId).ToListAsync();
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }

    public async Task<bool> RemoveFromRolesAsync(Guid userId, List<Guid> rolesToRemove)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Where(x => x.UserId == userId
                    && rolesToRemove.Contains(x.RoleId) && !x.IsDeleted && x.IsActive)
                .ToListAsync();
            if (userRoles.Any())
            {
                _context.UserRoles.RemoveRange(userRoles);

                //foreach (var userRole in userRoles)
                //{
                //    userRole.IsActive = false;
                //    userRole.IsDeleted = true;
                //    userRole.DeletedTime = DateTime.UtcNow;
                //}

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CustomExceptionFactory.CreateInternalServerError();
        }
    }
}