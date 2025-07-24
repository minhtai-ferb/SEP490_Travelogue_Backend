using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Travelogue.Repository.Bases.Exceptions;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.Commons.Interfaces;

namespace Travelogue.Service.Commons.Implementations;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Http context is null. Please Login.");
        }

        var user = httpContext.User;
        if (user == null || user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, ResponseMessages.LOGIN_REQUIRED);
        }

        var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "User ID claim is not found");
        }
        return currentUserId;
    }

    public string? TryGetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext.User.FindFirstValue("sub");
    }

    public Task<List<string>> GetCurrentUserRolesAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Http context is null. Please Login.");
        }

        var user = httpContext.User;
        if (user == null || user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, ResponseMessages.LOGIN_REQUIRED);
        }

        // Lấy tất cả các claims có type là Role
        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role") // hỗ trợ cả 2 dạng thường gặp
            .Select(c => c.Value)
            .Distinct()
            .ToList();

        return Task.FromResult(roles);
    }

    public bool HasRole(params string[] roles)
    {
        var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return userRoles != null && roles.Any(r => userRoles.Contains(r));
    }

    public bool HasRole(string role)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Http context is null. Please Login.");
        }

        var user = httpContext.User;
        if (user == null || user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, ResponseMessages.LOGIN_REQUIRED);
        }

        return user.IsInRole(role) || user.Claims.Any(c => (c.Type == ClaimTypes.Role || c.Type == "role") && c.Value.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasAnyRole(params string[] roles)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Http context is null. Please Login.");
        }

        var user = httpContext.User;
        if (user == null || user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, ResponseMessages.LOGIN_REQUIRED);
        }

        return roles.Any(role =>
            user.IsInRole(role) ||
            user.Claims.Any(c =>
                (c.Type == ClaimTypes.Role || c.Type == "role") &&
                c.Value.Equals(role, StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    public bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.IsAuthenticated == true;
    }

    public string GetUserToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var authorizationHeader = httpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Token không hợp lệ hoặc không tồn tại");
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        return token;
    }
}
