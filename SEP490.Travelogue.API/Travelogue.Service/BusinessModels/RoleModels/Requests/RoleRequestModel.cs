using System.ComponentModel.DataAnnotations;

namespace Travelogue.Service.BusinessModels.RoleModels.Requests;

public class RoleRequestModel
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
}
