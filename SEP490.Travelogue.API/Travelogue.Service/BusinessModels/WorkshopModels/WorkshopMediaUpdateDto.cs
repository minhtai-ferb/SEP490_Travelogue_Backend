namespace Travelogue.Service.BusinessModels.WorkshopModels;

public class WorkshopMediaUpdateDto
{
    public string? MediaUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public float? SizeInBytes { get; set; }
    public bool? IsThumbnail { get; set; }
}
