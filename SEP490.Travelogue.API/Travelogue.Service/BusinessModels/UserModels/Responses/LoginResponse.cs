namespace Travelogue.Service.BusinessModels.UserModels.Responses;
public class LoginResponse
{
    public string VerificationToken { get; set; }
    public string RefreshTokens { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public List<string> Roles { get; set; }
}
