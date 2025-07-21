using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum CraftVillageRequestStatus
{
    [Display(Name = "Chờ xác nhận")]
    Pending = 1,
    [Display(Name = "Đã xác nhận")]
    Approved = 2,
    [Display(Name = "Từ chối")]
    Rejected = 3
}