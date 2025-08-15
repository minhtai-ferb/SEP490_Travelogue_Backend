namespace Travelogue.Service.Commons.Interfaces;

public interface IUserContextService
{
    string GetCurrentUserId();
    string GetUserToken();
    string? TryGetCurrentUserId();
    Task<List<string>> GetCurrentUserRolesAsync();
    bool HasRole(string roleName);
    bool HasRole(params string[] roles);
    bool HasAnyRole(params string[] roles);
    bool HasAnyRoleOrAnonymous(params string[] roles);
    bool IsAuthenticated();
}
