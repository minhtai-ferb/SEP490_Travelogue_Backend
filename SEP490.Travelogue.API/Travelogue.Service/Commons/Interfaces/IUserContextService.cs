namespace Travelogue.Service.Commons.Interfaces;

public interface IUserContextService
{
    string GetCurrentUserId();
    string GetUserToken();
    string? TryGetCurrentUserId();
}
