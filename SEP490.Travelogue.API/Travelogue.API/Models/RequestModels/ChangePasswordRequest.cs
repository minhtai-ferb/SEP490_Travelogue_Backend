namespace Travelogue.API.Models.RequestModels;

public class ChangePasswordRequest
{
    public required string IdToken { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
