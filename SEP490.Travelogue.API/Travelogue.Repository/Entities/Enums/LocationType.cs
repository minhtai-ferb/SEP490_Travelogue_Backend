using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum LocationType
{
    [Display(Name = "Làng nghề")]
    CraftVillage = 1,

    [Display(Name = "Địa điểm lịch sử")]
    HistoricalSite = 2,

    [Display(Name = "Ẩm thực")]
    Cuisine = 3,

    [Display(Name = "Danh lam thắng cảnh")]
    ScenicSpot = 4,

    [Display(Name = "Khác")]
    Other = 0
}
