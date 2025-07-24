namespace Travelogue.Service.BusinessModels.UserModels.Requests;
public class UserRoleUpdateModel
{
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}
