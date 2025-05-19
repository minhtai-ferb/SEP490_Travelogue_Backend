namespace Travelogue.Service.BusinessModels.UserModels.Requests;
public class LoginRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}

public class LoginResponseGoogle
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}
