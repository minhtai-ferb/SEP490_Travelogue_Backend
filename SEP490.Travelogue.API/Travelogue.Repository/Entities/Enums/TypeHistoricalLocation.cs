using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TypeHistoricalLocation
{
    [Display(Name = "Di tích Quốc gia Đặc biệt")]
    SpecialNationalMonument = 1,

    [Display(Name = "Di tích cấp quốc gia")]
    NationalMonument = 2,

    [Display(Name = "Di tích cấp tỉnh")]
    ProvincialMonument = 3
}
