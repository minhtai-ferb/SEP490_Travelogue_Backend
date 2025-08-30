using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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