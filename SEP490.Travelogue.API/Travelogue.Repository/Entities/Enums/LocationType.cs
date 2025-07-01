using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum LocationType
{
    [Display(Name = "Làng nghề")]
    CraftVillage,

    [Display(Name = "Địa điểm lịch sử")]
    HistoricalSite,

    [Display(Name = "Ẩm thực")]
    Cuisine,

    [Display(Name = "Khách sạn")]
    Hotel,

    [Display(Name = "Khác")]
    Other = 5
}