namespace Travelogue.Service.BusinessModels.UserModels.Requests;
public class ResetTokenModel
{
    public required string Token { get; set; }
    public required string Email { get; set; }
}
