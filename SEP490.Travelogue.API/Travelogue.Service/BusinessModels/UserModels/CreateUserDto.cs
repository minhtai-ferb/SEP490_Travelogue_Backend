namespace Travelogue.Service.BusinessModels.UserModels;

public class CreateUserDto
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public Guid RoleId { get; set; }
}

public class UpdateUserRoleDto
{
    public Guid RoleId { get; set; }
}

public class UserResponseDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public List<string> Roles { get; set; }
}