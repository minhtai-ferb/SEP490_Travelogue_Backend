using System.ComponentModel.DataAnnotations;

namespace Travelogue.Repository.Entities.Enums;
public enum FileType
{
    [Display(Name = "Ảnh")]
    Image = 1,

    [Display(Name = "Video")]
    Video = 2,

    [Display(Name = "Âm thanh")]
    Audio = 3,

    [Display(Name = "Tài liệu")]
    Document = 4
}
