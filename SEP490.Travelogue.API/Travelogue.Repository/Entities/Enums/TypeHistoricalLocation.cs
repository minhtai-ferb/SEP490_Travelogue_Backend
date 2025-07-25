using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;

public enum TypeHistoricalLocation
{
    [Display(Name = "Di tích Quốc gia Đặc biệt")]
    SpecialNationalMonument,

    [Display(Name = "Di tích cấp quốc gia")]
    NationalMonument,

    [Display(Name = "Di tích cấp tỉnh")]
    ProvincialMonument
}