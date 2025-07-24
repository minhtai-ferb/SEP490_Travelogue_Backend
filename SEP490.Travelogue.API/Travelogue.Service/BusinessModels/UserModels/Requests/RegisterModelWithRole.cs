namespace Travelogue.Service.BusinessModels.UserModels.Requests;
public class RegisterModelWithRole
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public Guid? RoleId { get; set; }
}
