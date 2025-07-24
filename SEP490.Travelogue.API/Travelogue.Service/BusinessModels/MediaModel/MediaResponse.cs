namespace Travelogue.Service.BusinessModels.MediaModel;

public class MediaResponse
{
    public string MediaUrl { get; set; }
    public string FileName { get; set; }
    public string? FileType { get; set; }
    public bool IsThumbnail { get; set; }
    public float SizeInBytes { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
}
