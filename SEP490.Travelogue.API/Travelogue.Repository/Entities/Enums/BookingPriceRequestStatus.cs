using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum BookingPriceRequestStatus
{
    [Display(Name = "Chờ xác nhận")]
    Pending = 1,
    [Display(Name = "Đã chấp nhận")]
    Approved = 2,
    [Display(Name = "Đã từ chối")]
    Rejected = 3
}