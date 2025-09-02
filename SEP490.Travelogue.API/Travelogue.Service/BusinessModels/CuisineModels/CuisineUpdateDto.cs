using System.ComponentModel.DataAnnotations;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.CuisineModels;

public class CuisineUpdateDto
{
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? Address { get; set; }

    public double MinPrice { get; set; } = 0;
    public double MaxPrice { get; set; } = 0;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public Guid? DistrictId { get; set; }


    // Cuisine properties
    public string? CuisineType { get; set; } // Loại âm thực 
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public Guid? LocationId { get; set; }

    public string? SignatureProduct { get; set; }
    public string? CookingMethod { get; set; }

    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
