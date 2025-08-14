using Travelogue.Repository.Entities.Enums;

namespace Travelogue.Service.BusinessModels.UserModels.Responses;

public class UserResponseModel : BaseDataModel
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public bool? EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberConfirmed { get; set; }
    public string FullName { get; set; } = string.Empty;
    //public string? ProfilePictureUrl { get; set; }

    public string? AvatarUrl { get; set; }
    public List<string> Roles { get; set; }
    public decimal UserWalletAmount { get; set; }

    public Gender Sex { get; set; }
    public string? Address { get; set; }
    public bool? IsEmailVerified { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
