using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum Interest
{
    [Display(Name = "Di sản văn hóa")]
    CulturalHeritage = 1,

    [Display(Name = "Thiên nhiên")]
    Nature = 2,

    [Display(Name = "Địa điểm tôn giáo")]
    ReligiousSite = 3,

    [Display(Name = "Nghệ thuật truyền thống")]
    TraditionalArt = 4,

    [Display(Name = "Ăn uống")]
    Cuisine = 5,

    [Display(Name = "Di tích lịch sử")]
    HistoricalSite = 6
}