namespace Travelogue.Service.BusinessModels.TourModels;

public class TourMediaUpdateDto
{
    public string? MediaUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public float? SizeInBytes { get; set; }
    public bool? IsThumbnail { get; set; }
}