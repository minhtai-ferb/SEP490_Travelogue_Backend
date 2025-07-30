using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Bases;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;

namespace Travelogue.Repository.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<List<User>> GetUsersByRoleAsync(string roleName);
    Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName);
    Task<string> GetUserEmailByIdAsync(string userId);
    Task UpdateAsync(User user);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task UpdateUserAsync(User user);
    Task<string?> GetUserNameByIdAsync(object id);
    Task<List<User>> GetAllUserAsync(string? search = null);
    Task<PagedResult<User>> GetPageUserAsync(int pageNumber, int pageSize, string? search = null);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> CreateUser(User user, string password);
    Task<bool> CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt);
    Task<bool> AddToRoleAsync(User user, Guid roleId);
    Task<bool> AddToRoleAsync(User user, string roleName);
    Task<List<Role>?> GetRolesAsync(User user);
    Task<List<Role>> GetRolesByUserIdAsync(Guid userId);
    Task<User?> GetUserByIdAsync(Guid id);
}
public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName)
    {
        var searchTerm = fullName.Trim().ToLower();
        var users = await _context.Users
            .Where(u => u.FullName.ToLower().Contains(searchTerm) && u.IsDeleted == false && u.IsActive == true)
            .ToListAsync();
        return users;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string resetToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string?> GetUserNameByIdAsync(object id)
    {
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
        {
            return null;
        }
        return user.FullName;
    }

    public Task<string> GetUserEmailByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetAllUserAsync(string? search = null)
    {
        IQueryable<User> query = _context.Users;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return await query.ToListAsync();
    }

    public async Task<PagedResult<User>> GetPageUserAsync(int pageNumber, int pageSize, string? search = null)
    {
        IQueryable<User> query = _context.Users;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var query = await ActiveEntities.FirstOrDefaultAsync(u => u.Email == email);
        return query;
    }

    public async Task<User?> CreateUser(User user, string password)
    {
        try
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var result = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> AddToRoleAsync(User user, Guid roleId)
    {
        try
        {
            UserRole userRole = new UserRole { UserId = user.Id, RoleId = roleId, CreatedBy = "system", LastUpdatedBy = "system" };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        try
        {
            return await Task.Run(() => VerifyPasswordHash(password, passwordHash, passwordSalt));
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    public async Task<List<Role>?> GetRolesAsync(User user)
    {
        try
        {
            var roleIds = await _context.UserRoles
                .Where(u => u.UserId == user.Id)
                .Where(u => u.IsActive)
                .Where(u => !u.IsDeleted)
                .Select(u => u.RoleId)
                .ToListAsync();

            if (!roleIds.Any())
            {
                return new List<Role>();
            }

            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            return roles;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<bool> AddToRoleAsync(User user, string roleName)
    {
        try
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                throw CustomExceptionFactory.CreateBadRequestError("Role chưa tồn tại");
            }

            UserRole userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                CreatedBy = "system",
                LastUpdatedBy = "system"
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var query = await ActiveEntities.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            return query;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        try
        {
            var roleIds = await _context.UserRoles
                .Where(u => u.UserId == userId)
                .Where(u => u.IsActive)
                .Where(u => !u.IsDeleted)
                .Select(u => u.RoleId)
                .ToListAsync();

            if (!roleIds.Any())
            {
                return new List<Role>();
            }

            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            return roles;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }

    public async Task<List<User>> GetUsersByRoleAsync(string roleName)
    {
        try
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Join(_context.UserRoles,
                      u => u.Id,
                      ur => ur.UserId,
                      (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                      u_ur => u_ur.UserRole.RoleId,
                      r => r.Id,
                      (u_ur, r) => new { User = u_ur.User, Role = r })
                .Where(u_r => u_r.Role.Name.ToLower().Equals(roleName.ToLower()))
                .Select(u_r => u_r.User)
                .Distinct()
                .ToListAsync();

            return users;
        }
        catch (Exception ex)
        {
            throw CustomExceptionFactory.CreateInternalServerError(ex.Message);
        }
    }
}
