using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.TourModels;

public class UpdateTourDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? Content { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalDays { get; set; }

    public TourType? TourType { get; set; }
    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
