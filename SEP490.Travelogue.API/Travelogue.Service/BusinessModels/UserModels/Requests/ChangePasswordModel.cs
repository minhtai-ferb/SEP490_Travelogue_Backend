namespace Travelogue.Service.BusinessModels.UserModels.Requests;

public class ChangePasswordModel
{
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
