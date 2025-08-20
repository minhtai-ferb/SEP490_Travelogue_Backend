using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.UserModels.Requests;

public class RegisterModel
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public Gender? Sex { get; set; }
    public required string ConfirmPassword { get; set; }
}
