using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.LocationModels;

public class LocationUpdateDto
{
    [Required(ErrorMessage = "Tên địa điểm là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên địa điểm không được vượt quá 200 ký tự")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Mô tả là bắt buộc")]
    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Nội dung là bắt buộc")]
    public string? Content { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Vĩ độ là bắt buộc")]
    [Range(-90, 90, ErrorMessage = "Vĩ độ phải nằm trong khoảng -90 đến 90")]
    public double Latitude { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá tối thiểu không hợp lệ")]
    public double MinPrice { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa không hợp lệ")]
    public double MaxPrice { get; set; } = 0;

    [Required(ErrorMessage = "Kinh độ là bắt buộc")]
    [Range(-180, 180, ErrorMessage = "Kinh độ phải nằm trong khoảng -180 đến 180")]
    public double Longitude { get; set; }

    public TimeSpan? OpenTime { get; set; }

    public TimeSpan? CloseTime { get; set; }

    public Guid? DistrictId { get; set; }

    [Required(ErrorMessage = "Loại địa điểm là bắt buộc")]
    public LocationType LocationType { get; set; }

    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}