using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.TourModels;

public class CreateTourDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    public string? TransportType { get; set; }
    [MaxLength(300)]
    public string? PickupAddress { get; set; }
    [MaxLength(500)]
    public string? StayInfo { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    public TourType TourType { get; set; }
    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
