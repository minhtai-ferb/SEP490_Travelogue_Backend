namespace Travelogue.Service.BusinessModels.RoleModels.Responses;
public class RoleResponseModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
}
