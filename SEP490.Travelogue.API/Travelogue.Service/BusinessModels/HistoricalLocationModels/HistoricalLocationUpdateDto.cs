using System.ComponentModel.DataAnnotations;
using Travelogue.Repository.Entities.Enums;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.BusinessModels.HistoricalLocationModels;

public class HistoricalLocationUpdateDto
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

    // Craft Village specific properties
    public HeritageRank HeritageRank { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public Guid LocationId { get; set; }
    public TypeHistoricalLocation? TypeHistoricalLocation { get; set; }
    public List<MediaDto> MediaDtos { get; set; } = new List<MediaDto>();
}
