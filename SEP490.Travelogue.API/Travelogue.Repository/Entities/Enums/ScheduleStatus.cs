using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum ScheduleStatus
{
    [Display(Name = "Hoạt động")]
    Active = 1,
    [Display(Name = "Đóng cửa")]
    Closed = 2,
    [Display(Name = "Dã hủy")]
    Cancelled = 3,
}