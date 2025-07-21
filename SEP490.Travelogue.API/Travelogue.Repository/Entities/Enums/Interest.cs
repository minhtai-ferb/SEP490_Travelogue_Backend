using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum Interest
{
    [Display(Name = "Di sản văn hóa")]
    CulturalHeritage,

    [Display(Name = "Thiên nhiên")]
    Nature,

    [Display(Name = "Địa điểm tôn giáo")]
    ReligiousSite,

    [Display(Name = "Nghệ thuật truyền thống")]
    TraditionalArt,

    [Display(Name = "Ăn uống")]
    Cuisine,

    [Display(Name = "Di tích lịch sử")]
    HistoricalSite
}