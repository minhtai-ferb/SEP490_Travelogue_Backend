using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;
public enum HeritageRank
{
    [Display(Name = "Không xác định")]
    KhongXacDinh = 0,

    [Display(Name = "Di tích cấp tỉnh")]
    CapTinh = 1,

    [Display(Name = "Di tích cấp quốc gia")]
    CapQuocGia = 2,

    [Display(Name = "Di tích cấp quốc gia đặc biệt")]
    CapQuocGiaDacBiet = 3,
}
