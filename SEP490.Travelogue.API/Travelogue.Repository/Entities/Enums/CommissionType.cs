using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum CommissionType
{
    [Display(Name = "Hoa hồng tour guide")]
    TourGuideCommission = 1,

    [Display(Name = "Hoa hồng làng nghề")]
    CraftVillageCommission = 2
}