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

    [Display(Name = "Khách sạn")]
    Hotel = 4,

    [Display(Name = "Danh lam thắng cảnh")]
    ScenicSpot = 5,

    [Display(Name = "Khác")]
    Other = 6
}
