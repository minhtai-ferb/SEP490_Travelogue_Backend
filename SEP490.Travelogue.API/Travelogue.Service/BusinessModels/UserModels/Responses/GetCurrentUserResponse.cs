namespace Travelogue.Service.BusinessModels.UserModels.Responses;

public class GetCurrentUserResponse
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal UserWalletAmount { get; set; }
    public List<string> Roles { get; set; }
}
